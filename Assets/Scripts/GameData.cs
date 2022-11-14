using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Unity.Multiplayer.Tools.NetStatsMonitor;


public class GameData : NetworkBehaviour {
    private static GameData _instance;
    public class CommandLineArgs{
        private const string PRE = "--";
        
        public DebugRunner.StartModes startMode = DebugRunner.StartModes.CHOOSE;
        public string startScene = "Lobby";

        public Dictionary<string, string> cmdArgs;

        public CommandLineArgs(){
            cmdArgs = GetCommandLineArgs();
            PrintCommandLineArgs(cmdArgs);
            ParseArgs(cmdArgs);
        }

        private Dictionary<string, string> GetCommandLineArgs() {
            Dictionary<string, string> argDictionary = new Dictionary<string, string>();

            var args = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; ++i){
                var arg = args[i].ToLower();
                if(arg.StartsWith(PRE)){
                    arg = arg.Substring(PRE.Length);
                    var value = i < args.Length - 1 ? args[i + 1]:null;
                    value = (value?.StartsWith(PRE) ?? false) ? null : value;
                    argDictionary.Add(arg, value);
                }
            }
            return argDictionary;
        }
        
        private void ParseArgs(Dictionary<string, string> args){
            if (!args.TryGetValue("start_scene", out startScene)){
                startScene = "Lobby";
            }

            string cmdStartMode = "";
            if (args.TryGetValue("start_mode", out cmdStartMode)) {
                if (cmdStartMode == "server"){
                    Debug.Log("Server not supported now.");
                }else if (cmdStartMode == "host") {
                    startMode = DebugRunner.StartModes.HOST;
                }else if (cmdStartMode == "client") {
                    startMode = DebugRunner.StartModes.CLIENT;
                }
            }

            Debug.Log($"[cmd] start scene = {startScene}");
            Debug.Log($"[cmd] start mode = {cmdStartMode}");
        }
        
        private void PrintCommandLineArgs(Dictionary<string, string> args){
            Debug.Log($"[cmd] Args found: {args.Keys.Count}");
            foreach (KeyValuePair<string, string> kvp in args) {
                Debug.Log($"{kvp.Key} = {kvp.Value}");
            }
        }
    }

    public static GameData Instance {
        get {
            return _instance;
        }
    }

    public static DebugRunner dbgRun = new DebugRunner();
    public static CommandLineArgs cmdArgs = new CommandLineArgs();



    private int colorIndex = 0;
    private Color[] playerColors = new Color[] {
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
        Color.cyan
    };

    public NetworkList<PlayerInfo> allPlayers;
    public RuntimeNetStatsMonitor netMonitor;


    // --------------------------
    // Initialization
    // --------------------------
    public void Awake() {
        // allPlayers must be initialized even though we might be destroying
        // this instance.  Errors occur if we do not.
        allPlayers = new NetworkList<PlayerInfo>();

        // This isn't working as expected.  If you place another GameData in a
        // later scene, it causes an error.  I suspect this has something to
        // do with the NetworkList but I have not verified that yet.  It causes
        // Network related errors.
        if(_instance == null) {
            _instance = this;
            DontDestroyOnLoad(this);
        } else if(_instance != this) {
            Destroy(this);
        }
    }
    public void Start(){
        netMonitor = NetworkManager.GetComponent<RuntimeNetStatsMonitor>();
    }

    public override void OnNetworkSpawn() {
        if (IsHost) {
            NetworkManager.Singleton.OnClientConnectedCallback += HostOnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HostOnClientDisconnected;
            AddPlayerToList(NetworkManager.LocalClientId);
        }
    }
    public void Update() {
        if(Input.GetKeyDown(KeyCode.N)){
            if (netMonitor){
            netMonitor.Visible = !netMonitor.Visible;
            netMonitor.enabled = netMonitor.Visible;
            }
        }   
    }


    // --------------------------
    // Private
    // --------------------------
    private Color NextColor() {
        Color newColor = playerColors[colorIndex];
        colorIndex += 1;
        if (colorIndex > playerColors.Length - 1) {
            colorIndex = 0;
        }
        return newColor;
    }


    // --------------------------
    // Events
    // --------------------------

    private void HostOnClientConnected(ulong clientId) {
        Debug.Log($"[GameData] Client connected {clientId}");
        AddPlayerToList(clientId);
    }
    private void HostOnClientDisconnected(ulong clientId) {
        Debug.Log($"[GameData] Client disconnected {clientId}");
        RemovePlayerFromList(clientId);
    }


    // --------------------------
    // Public
    // --------------------------
    public void AddPlayerToList(ulong clientId) {
        allPlayers.Add(new PlayerInfo(clientId, NextColor(), true));
    }

    public void RemovePlayerFromList(ulong clientId) {
        int index = FindPlayerIndex(clientId);
        if (index != -1) {
            allPlayers.RemoveAt(index);
        }
    }


    public int FindPlayerIndex(ulong clientId) {
        var idx = 0;
        var found = false;

        while (idx < allPlayers.Count && !found) {
            if (allPlayers[idx].clientId == clientId) {
                found = true;
            } else {
                idx += 1;
            }
        }

        if (!found) {
            idx = -1;
        }

        return idx;
    }
}