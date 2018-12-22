using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyDisplayReadys : MonoBehaviour
{
    /*Settings*/
    public List<Text> ReadyTexts = new List<Text>();
    public Color NotReadyColor = Color.red;
    public Color ReadyColor = Color.green;

    /*Required components*/
    private ManagerTracker Tracker;

    void Start()
    {
        Tracker = GameObject.FindGameObjectWithTag("ManagerTracker").GetComponent<ManagerTracker>();
    }

    void Update()
    {
        UpdateReady();
    }

    void UpdateReady()
    {
        if (!Tracker.IsFullyReady) return;
        for (int i = 0; i < Tracker.HostPlayer.TheResourceManager.Readys.Count; i++)
        {
            if(Tracker.HostPlayer.TheResourceManager.ConnectStatus[i])
            {
                //Ready
                if (Tracker.HostPlayer.TheResourceManager.Readys[i])
                {
                    ReadyTexts[i].color = ReadyColor;
                    ReadyTexts[i].text = "Ready";
                }
                //Not ready
                else
                {
                    ReadyTexts[i].color = NotReadyColor;
                    ReadyTexts[i].text = "Unready";
                }
            }
            //Disconnected
            else
            {
                ReadyTexts[i].color = NotReadyColor;
                ReadyTexts[i].text = "Closed";
            }
        }
    }
}
