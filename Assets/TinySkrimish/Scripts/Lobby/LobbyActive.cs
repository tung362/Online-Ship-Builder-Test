using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyActive : MonoBehaviour
{
    /*Settings*/
    public List<GameObject> ShipPreviews = new List<GameObject>();
    public List<GameObject> SmallBoxes = new List<GameObject>();

    /*Required Components*/
    private ManagerTracker Tracker;
    private NetworkManager TheNetworkManager;

    void Start()
    {
        Tracker = GameObject.FindGameObjectWithTag("ManagerTracker").GetComponent<ManagerTracker>();
        TheNetworkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
    }

    void Update()
    {
        UpdateActive();
    }

    void UpdateActive()
    {
        if (!Tracker.IsFullyReady) return;
        if (!TheNetworkManager.IsClientConnected()) return;
        if (Tracker.HostPlayer.TheResourceManager.ConnectStatus.Count - 1 < Tracker.ThePlayerControlPanel.ID) return;

        for(int i = 0; i < ShipPreviews.Count; i++)
        {
            if (Tracker.HostPlayer.TheResourceManager.ConnectStatus.Count - 1 < i)
            {
                ShipPreviews[i].SetActive(false);
                SmallBoxes[i].SetActive(false);
            }
            else
            {
                if (!Tracker.HostPlayer.TheResourceManager.ConnectStatus[i])
                {
                    ShipPreviews[i].SetActive(false);
                    SmallBoxes[i].SetActive(false);
                }
                else
                {
                    ShipPreviews[i].SetActive(true);
                    SmallBoxes[i].SetActive(true);
                }
            }
        }
    }
}
