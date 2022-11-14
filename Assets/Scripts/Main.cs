using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Multiplayer.Tools;
using Unity.Netcode.Transports.UTP;
using System.Net;
using AddressFamily = System.Net.Sockets.AddressFamily;
using System.IO;
using System.Text;


public class Main : NetworkBehaviour {

    public Button btnHost;
    public Button btnClient;
    public TMPro.TMP_Text txtStatus;
    public TMPro.TMP_InputField inputIp;
    public TMPro.TMP_InputField inputPort;

    public void Start() {
        btnHost.onClick.AddListener(OnHostClicked);
        btnClient.onClick.AddListener(OnClientClicked);
        NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnect;
        Application.targetFrameRate = 30;

        DebugStart();

    }
    
    private void DebugStart(){
        if(GameData.dbgRun.startMode == DebugRunner.StartModes.HOST){
            string startMsg = $"Starting as {GameData.dbgRun.startMode} with scene{GameData.dbgRun.startScene}";
            StartHost(GameData.dbgRun.startScene, startMsg);
        }else if(GameData.dbgRun.startMode == DebugRunner.StartModes.CLIENT){
            StartClient();
        }else{
            if(GameData.cmdArgs.startMode == DebugRunner.StartModes.HOST){
                StartHost(GameData.cmdArgs.startScene);
            }else if(GameData.cmdArgs.startMode == DebugRunner.StartModes.CLIENT){
                StartClient();
            }
        }
        GameData.cmdArgs.startMode = DebugRunner.StartModes.CHOOSE;
        GameData.dbgRun.startMode = DebugRunner.StartModes.CHOOSE;
    }

    private void StartHost(string sceneName = "Lobby", string startMessage = "Starting Host") {
        bool validSettings = ValidateSettings();
        if(!validSettings){
            return;
        }

        txtStatus.text = startMessage;

        NetworkManager.Singleton.StartHost();
        NetworkManager.SceneManager.LoadScene(
            sceneName,
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    private void StartClient(string startMessage = "Starting Client"){
        bool validSettings = ValidateSettings();
        if(!validSettings){
            return;
        }

        txtStatus.text = startMessage;

        NetworkManager.Singleton.StartClient();
        txtStatus.text = "Waiting on Host";
    }

    private bool ValidateSettings() {
        IPAddress ip;
        bool isValidIp = IPAddress.TryParse(inputIp.text, out ip);
        if(!isValidIp){
            txtStatus.text = "Invaild IP";
            return false;
        }

        bool isValidPort = ushort.TryParse(inputPort.text, out ushort port);
        if(!isValidPort){
            txtStatus.text = "Invaild Port";
            return false;
        }

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            ip.ToString(), port
        );

        btnHost.gameObject.SetActive(false);
        btnClient.gameObject.SetActive(false);
        inputIp.enabled = false;
        inputPort.enabled = false;

        return true;
    }

    private void OnHostClicked() {
        btnClient.gameObject.SetActive(false);
        btnHost.gameObject.SetActive(false);
        txtStatus.text = "Starting Host";
        StartHost();
    }

    private void OnClientClicked() {
        btnClient.gameObject.SetActive(false);
        btnHost.gameObject.SetActive(false);
        txtStatus.text = "Waiting on Host";
        NetworkManager.Singleton.StartClient();
    }

    private void OnDisconnect(ulong clientId) {
        txtStatus.text = "Failed to connect to server";

        btnHost.gameObject.SetActive(true);
        btnClient.gameObject.SetActive(true);
        inputIp.enabled = true;
        inputPort.enabled = true;
    }
}
