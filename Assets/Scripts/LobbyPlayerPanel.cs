using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;



public class LobbyPlayerPanel : MonoBehaviour
{
    [SerializeField] protected TMPro.TMP_Text txtName;
    [SerializeField] protected TMPro.TMP_Text txtReady;
    [SerializeField] protected GameObject pnlColor;
    [SerializeField] public Button btnKick;

    public event Action OnKickPlayer;

    private bool isReady = false;

    public void Start() {
        SetReady(isReady);
        btnKick.onClick.AddListener(OnKickPlayerClicked);
    }

    private void OnKickPlayerClicked(){
        Debug.Log($"Panel for{txtName.text} kick button pressed");
        OnKickPlayer.Invoke();
    }

    public void SetName(string newName) {
        txtName.text = newName;
    }

    public string GetName() {
        return txtName.text;
    }

    public void SetColor(Color c) {
        pnlColor.GetComponent<Image>().color = c;
    }

    public void ShowKick(bool jack){
        btnKick.gameObject.SetActive(jack);
    }
    public void SetReady(bool ready) {
        isReady = ready;
        if (isReady) {
            txtReady.text = "Ready";
        } else {
            txtReady.text = "Not Ready";
        }
    }
}