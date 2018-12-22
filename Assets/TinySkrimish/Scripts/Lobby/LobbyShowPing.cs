using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LobbyShowPing : MonoBehaviour
{
    /*Required Components*/
    private ManagerTracker Tracker;
    private NetworkManager TheNetworkManager;
    private Text TheText;

    void Start()
    {
        Tracker = GameObject.FindGameObjectWithTag("ManagerTracker").GetComponent<ManagerTracker>();
        TheNetworkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
        TheText = GetComponent<Text>();
    }

    void Update()
    {
        UpdatePing();
    }

    void UpdatePing()
    {
        if (!Tracker.IsFullyReady) return;
        if (!TheNetworkManager.IsClientConnected()) return;
        if (Tracker.HostPlayer.TheResourceManager.Pings.Count - 1 < Tracker.ThePlayerControlPanel.ID) return;

        TheText.text = "Ping: " + Tracker.HostPlayer.TheResourceManager.Pings[Tracker.ThePlayerControlPanel.ID];
    }
}
