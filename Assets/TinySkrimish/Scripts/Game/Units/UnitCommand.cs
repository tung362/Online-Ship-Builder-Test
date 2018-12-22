using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles the commands of units such as attacking base, attacking units, moving units
public class UnitCommand : MonoBehaviour
{
    /*Settings*/
    public LayerMask Mask;

    /*Required Components*/
    private ManagerTracker Tracker;
    private SelectionManager TheSelectionManager;

    void Start()
    {
        Tracker = GameObject.FindGameObjectWithTag("ManagerTracker").GetComponent<ManagerTracker>();
        TheSelectionManager = GetComponent<SelectionManager>();
    }

    void Update()
    {
        UpdateInput();
    }

    void UpdateInput()
    {
        //Right click
        if (Input.GetMouseButtonUp(1)) UpdateCommand();
    }

    void UpdateCommand()
    {
        if (!Tracker.IsFullyReady) return;
        if (TheSelectionManager.SelectedBuildings.Count != 0) return;
        if (TheSelectionManager.SelectedUnits.Count == 0) return;

        Ray mouseRay = TheSelectionManager.Cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit mouseHit;
        if (Physics.Raycast(mouseRay, out mouseHit, Mathf.Infinity, Mask.value))
        {
            //Attack base
            if (mouseHit.transform.root.tag == "ShipBase")
            {
                if (mouseHit.transform.root.GetComponent<UnitID>().ID != Tracker.ThePlayerControlPanel.ID)
                {
                    for (int i = 0; i < TheSelectionManager.SelectedUnits.Count; i++)
                    {
                        if (TheSelectionManager.SelectedUnits[i] != null) Tracker.ThePlayerControlPanel.TheCommandManager.CmdSetUnitAttackBuilding(TheSelectionManager.SelectedUnits[i], mouseHit.transform.root.gameObject);
                    }
                }
                //Walk if clicked on an allied base
                else MoveUnitsInCircleFormation(mouseHit.point);
            }
            //Attack unit
            else if (mouseHit.transform.root.tag == "Unit")
            {
                if (mouseHit.transform.root.GetComponent<UnitID>().ID != Tracker.ThePlayerControlPanel.ID)
                {
                    for (int i = 0; i < TheSelectionManager.SelectedUnits.Count; i++)
                    {
                        if(TheSelectionManager.SelectedUnits[i] != null) Tracker.ThePlayerControlPanel.TheCommandManager.CmdSetUnitAttackUnit(TheSelectionManager.SelectedUnits[i], mouseHit.transform.root.gameObject);
                    }
                }
                //Walk if clicked on an allied base
                else MoveUnitsInCircleFormation(mouseHit.point);
            }
            //Move to point
            else if (mouseHit.transform.root.tag == "Ground") MoveUnitsInCircleFormation(mouseHit.point);
        }
    }

    Vector3 GeneratePoint(Vector3 point, int Count, float AgentSize)
    {
        return point + (Random.insideUnitSphere * (Count * AgentSize * 0.4f));
    }

    //Moves all selected units in a circle packer formation
    void MoveUnitsInCircleFormation(Vector3 Position)
    {
        //Radius of the unit
        float Radius = 0.7f;
        float CurrentRingCount = 0;
        float CurrentCount = 0;

        for (int i = 0; i < TheSelectionManager.SelectedUnits.Count; i++)
        {
            //Center garenteed
            if (i == 0)
            {
                if (TheSelectionManager.SelectedUnits[i] != null) Tracker.ThePlayerControlPanel.TheCommandManager.CmdSetUnitMove(TheSelectionManager.SelectedUnits[i], new Vector3(Position.x, TheSelectionManager.SelectedUnits[i].transform.position.y, Position.z));
            }
            else
            {
                if (CurrentRingCount == 0)
                {
                    if (CurrentCount % 6 == 0)
                    {
                        CurrentRingCount += 1;
                        CurrentCount = 0;
                    }
                }
                else
                {
                    if (CurrentCount % (6 * CurrentRingCount) == 0)
                    {
                        CurrentRingCount += 1;
                        CurrentCount = 0;
                    }
                }

                float slice = 2 * Mathf.PI / (6 * CurrentRingCount);
                float angle = slice * (float)(CurrentCount % (6 * CurrentRingCount));
                float newX = (Position.x + ((Radius * 2) * CurrentRingCount) * Mathf.Cos(angle));
                float newZ = (Position.z + ((Radius * 2) * CurrentRingCount) * Mathf.Sin(angle));
                if (TheSelectionManager.SelectedUnits[i] != null) Tracker.ThePlayerControlPanel.TheCommandManager.CmdSetUnitMove(TheSelectionManager.SelectedUnits[i], new Vector3(newX, TheSelectionManager.SelectedUnits[i].transform.position.y, newZ));
                CurrentCount += 1;
            }
        }
    }

}
