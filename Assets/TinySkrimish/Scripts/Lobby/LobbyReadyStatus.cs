using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LobbyReadyStatus : MonoBehaviour
{
    /*Settings*/
    public Color NotReadyColor = Color.red;
    public Color ReadyColor = Color.green;

    /*Required Components*/
    private ManagerTracker Tracker;
    private Text TheText;
    private NetworkManager TheNetworkManager;

    void Start ()
    {
        Tracker = GameObject.FindGameObjectWithTag("ManagerTracker").GetComponent<ManagerTracker>();
        TheText = GetComponent<Text>();
        TheNetworkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateReadyStatus();
    }

    void UpdateReadyStatus()
    {
        if (!Tracker.IsFullyReady) return;
        if (!TheNetworkManager.IsClientConnected()) return;
        if (Tracker.HostPlayer.TheResourceManager.Readys.Count - 1 < Tracker.ThePlayerControlPanel.ID) return;

        if (Tracker.HostPlayer.TheResourceManager.Readys[Tracker.ThePlayerControlPanel.ID])
        {
            TheText.text = "Ready";
            TheText.color = ReadyColor;
        }
        else
        {
            TheText.text = "Unready";
            TheText.color = NotReadyColor;
        }
    }
}
