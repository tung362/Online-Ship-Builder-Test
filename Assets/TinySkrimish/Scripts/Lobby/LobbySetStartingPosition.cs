using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySetStartingPosition : MonoBehaviour
{
    /*Settings*/
    public List<GameObject> TargetsToMove;
    public GameObject PlayerMarker;
    public List<GameObject> StartingLocations = new List<GameObject>();

    /*Data*/
    private bool RunOnce = true;

    /*Required Components*/
    private ManagerTracker Tracker;

    void Start()
    {
        Tracker = GameObject.FindGameObjectWithTag("ManagerTracker").GetComponent<ManagerTracker>();
    }

    void Update ()
    {
        UpdateStartingPosition();
    }

    void UpdateStartingPosition()
    {
        if (!Tracker.IsFullyReady) return;
        if (!RunOnce) return;

        for(int i = 0; i < StartingLocations.Count; i++)
        {
            if(Tracker.ThePlayerControlPanel.ID == i)
            {
                for(int j = 0; j < TargetsToMove.Count; j++)
                {
                    TargetsToMove[j].transform.position = StartingLocations[i].transform.position;
                    TargetsToMove[j].transform.rotation = StartingLocations[i].transform.rotation;
                }
                PlayerMarker.transform.position = new Vector3(StartingLocations[i].transform.position.x, PlayerMarker.transform.position.y, StartingLocations[i].transform.position.z);
                RunOnce = false;
            }
        }
    }
}
