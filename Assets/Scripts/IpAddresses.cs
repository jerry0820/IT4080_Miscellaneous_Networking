using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using System;

public class IpAddresses : MonoBehaviour
{
    public string localIp = "?.?.?.?";
    public string globalIp = "?.?.?.?";
    public string chosenIp = "";

    public GameObject scrollContent;
    public GameObject btnTemplate;
    public Button iPButtonPrefab;

    public event Action<string> IpChosen;

    private void Start(){
        btnTemplate.gameObject.SetActive(false);
        localIp = GetLocalIp();
        globalIp = GetGlobalIp();

        AddButton(localIp, "local");
        AddButton(globalIp, "global");
        AddButton("127.0.0.1", "localhost");
        AddButton("0.0.0.0", "all");
    }

    private void AddButton(string ip, string label){
        GameObject newButton = Instantiate(btnTemplate);
        newButton.transform.SetParent(scrollContent.transform, false);
        newButton.gameObject.SetActive(true);
        Button b = newButton.transform.Find("IpButton").GetComponent<Button>();
        newButton.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = label;
        b.transform.Find("ButtonText").GetComponent<TextMeshProUGUI>().text = ip;
        b.onClick.AddListener(delegate {OnIpButtonClicked(ip);});
    }

    private void OnIpButtonClicked(string ip){
        IpChosen.Invoke(ip);
    }
    public string GetGlobalIp(){
        string toReturn = "?.?.?.?";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.ipify.org");
        request.Method = "GET";
        request.Timeout = 1000;
        try {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK) {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream,Encoding.UTF8);
                toReturn = reader.ReadToEnd();
            }
            else {
                Debug.LogError("Timed out?"+response.StatusDescription);
            }
        } catch (WebException ex) {
            Debug.Log("Likely no internet connection: "+ex.Message);
        }
        return toReturn;
    }

    public string GetLocalIp(){
        string toReturn = "?.?.?.?";

        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in hostEntry.AddressList) {
            if (ip.AddressFamily == AddressFamily.InterNetwork) {
                toReturn = ip.ToString();
                break;
            }
        }
        return toReturn;
    }
}
