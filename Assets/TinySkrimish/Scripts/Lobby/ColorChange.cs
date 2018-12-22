using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ColorChange : MonoBehaviour
{
    /*Data*/
    public bool RunOnce = true;

    /*Required Components*/
    private ManagerTracker Tracker;
    private BuilderColorTracker TheBuilderColorTracker;
    private NetworkManager TheNetworkManager;

    void Start()
    {
        Tracker = GameObject.FindGameObjectWithTag("ManagerTracker").GetComponent<ManagerTracker>();
        TheBuilderColorTracker = GameObject.FindGameObjectWithTag("BuilderColorTracker").GetComponent<BuilderColorTracker>();
        TheNetworkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
    }

    void Update()
    {
        if(RunOnce)
        {
            if (Tracker == null) return;
            if (!Tracker.IsFullyReady) return;
            if (!TheNetworkManager.IsClientConnected()) return;
            if (Tracker.HostPlayer.TheResourceManager.PlayerColor.Count - 1 < Tracker.ThePlayerControlPanel.ID) return;

            UpdatePlayerColor();
            RunOnce = false;
        }
    }

    public void UpdatePlayerColor()
    {
        if (Tracker == null) return;
        if (!Tracker.IsFullyReady) return;
        if (!TheNetworkManager.IsClientConnected()) return;
        if (Tracker.HostPlayer.TheResourceManager.PlayerColor.Count - 1 < Tracker.ThePlayerControlPanel.ID) return;


        Tracker.ThePlayerControlPanel.TheCommandManager.CmdChangeShipColor(TheBuilderColorTracker.BaseBaseColor,
            TheBuilderColorTracker.BaseEmissionColor,
            TheBuilderColorTracker.TrimBaseColor,
            TheBuilderColorTracker.TrimEmissionColor,
            TheBuilderColorTracker.EngineBaseColor,
            TheBuilderColorTracker.EngineEmissionColor,
            TheBuilderColorTracker.ShieldColor,
            Tracker.ThePlayerControlPanel.ID);
    }
}
