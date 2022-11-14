using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class DebugRunner{
    public enum StartModes {
        CLIENT,
        SERVER,
        HOST,
        CHOOSE
    }
    public StartModes startMode = StartModes.CHOOSE;
    public string startScene = "Lobby";

    public void StartGameWithSceneIfNotStarted(string overrideSceneName = null) {
        if(GameData.Instance == null){
            if(overrideSceneName == null){
                startScene = SceneManager.GetActiveScene().name;
            }else{
                startScene = overrideSceneName;
            }

            startMode = StartModes.HOST;
            SceneManager.LoadScene(
                "Main",
                UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

    }
}
