using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles the selection box
public class SelectionManager : MonoBehaviour
{
    /*Data*/
    public Camera Cam;
    public List<GameObject> SelectedUnits;
    public List<GameObject> SelectedBuildings;
    public Rect SelectionBox;
    private Vector2 shiptingMousePosition = -Vector2.one;
    private Vector2 EndingMousePosition = -Vector2.one;
    private Vector3 shiptingHitPostion = -Vector3.one;
    private Vector3 EndingHitPostion = -Vector3.one;
    public bool IsOverUI = false;

    /*Required Components*/
    private ManagerTracker Tracker;

    void Start()
    {
        Tracker = GameObject.FindGameObjectWithTag("ManagerTracker").GetComponent<ManagerTracker>();
    }

    void Update()
    {
        if (!Tracker.IsFullyReady) return;
        if (IsOverUI) IsOverUI = false;
        else
        {
            UpdateInput();
            SetRectangle();
        }
    }

    void UpdateInput()
    {
        //shipt
        if (Input.GetMouseButtonDown(0))
        {
            shiptingHitPostion = RaycastMousePosition();
            ResetSelect();
        }

        //Held
        if (Input.GetMouseButton(0))
        {
            //Reset builder select toggle
            if (Input.GetKeyUp(KeyCode.LeftControl)) ResetFake();
            //shipbase toggle
            EndingHitPostion = RaycastMousePosition();
            FakeDragSelectObject();
        }

        //Reset
        if (Input.GetMouseButtonUp(0))
        {
            shiptingMousePosition = -Vector3.one;
            DragSelectObject();
            QuickSelect();
        }
    }

    //Sets the rectangle size
    void SetRectangle()
    {
        Vector3 shiptingScreenPoint = Cam.WorldToScreenPoint(shiptingHitPostion);
        Vector3 endingScreenPoint = Cam.WorldToScreenPoint(EndingHitPostion);
        shiptingMousePosition = new Vector3(shiptingScreenPoint.x, shiptingScreenPoint.y, 0);
        EndingMousePosition = new Vector3(endingScreenPoint.x, endingScreenPoint.y, 0);

        float x = shiptingMousePosition.x;
        float y = shiptingMousePosition.y;
        float width = EndingMousePosition.x - shiptingMousePosition.x;
        float height = EndingMousePosition.y - shiptingMousePosition.y;

        //Flipped
        if (shiptingMousePosition.x > EndingMousePosition.x)
        {
            x = EndingMousePosition.x;
            width = shiptingMousePosition.x - EndingMousePosition.x;
        }
        if (shiptingMousePosition.y > EndingMousePosition.y)
        {
            y = EndingMousePosition.y;
            height = shiptingMousePosition.y - EndingMousePosition.y;
        }

        SelectionBox.Set(x, y, width, height);
    }

    Vector3 RaycastMousePosition()
    {
        float x = 0;
        float z = 0;

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit mouseHit;
        if (Physics.Raycast(mouseRay, out mouseHit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
        {
            x = mouseHit.point.x;
            z = mouseHit.point.z;
        }

        return new Vector3(x, 0, z);
    }

    //Hovered over
    void FakeDragSelectObject()
    {
        //Shipbase select
        if (Input.GetKey(KeyCode.LeftControl))
        {
            GameObject[] ShipBases = GameObject.FindGameObjectsWithTag("ShipBase");
            GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
            foreach (GameObject shipBase in ShipBases)
            {
                if (shipBase != null)
                {
                    if (SelectionBox.Contains(Cam.WorldToScreenPoint(shipBase.transform.position)) && Tracker.ThePlayerControlPanel.ID == shipBase.GetComponent<UnitID>().ID) shipBase.GetComponent<UnitStats>().IsSelected = true;
                    else shipBase.GetComponent<UnitStats>().IsSelected = false;
                }
            }
            foreach (GameObject unit in units)
            {
                if (unit != null) unit.GetComponent<UnitStats>().IsSelected = false;
            }
        }
        //Unit select
        else
        {
            GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
            foreach (GameObject unit in units)
            {
                if (unit != null)
                {
                    if (SelectionBox.Contains(Cam.WorldToScreenPoint(unit.transform.position)) && Tracker.ThePlayerControlPanel.ID == unit.GetComponent<UnitID>().ID) unit.GetComponent<UnitStats>().IsSelected = true;
                    else unit.GetComponent<UnitStats>().IsSelected = false;
                }
            }
        }
    }

    //Add hovered objects to list
    void DragSelectObject()
    {
        //Shipbase select
        if(Input.GetKey(KeyCode.LeftControl))
        {
            GameObject[] shipBases = GameObject.FindGameObjectsWithTag("ShipBase");
            foreach (GameObject shipBase in shipBases)
            {
                if (SelectionBox.Contains(Cam.WorldToScreenPoint(shipBase.transform.position)) && Tracker.ThePlayerControlPanel.ID == shipBase.GetComponent<UnitID>().ID) SelectedBuildings.Add(shipBase);
            }
        }
        //Unit select
        else
        {
            GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
            foreach (GameObject unit in units)
            {
                if (SelectionBox.Contains(Cam.WorldToScreenPoint(unit.transform.position)) && Tracker.ThePlayerControlPanel.ID == unit.GetComponent<UnitID>().ID) SelectedUnits.Add(unit);
            }
        }
    }

    //Add clicked object to list
    void QuickSelect()
    {
        if (SelectedUnits.Count == 0)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;
            if (Physics.Raycast(mouseRay, out mouseHit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Attached")))
            {
                //Unit select
                if (mouseHit.transform.root.tag == "Unit")
                {
                    if (Tracker.ThePlayerControlPanel.ID == mouseHit.transform.root.GetComponent<UnitID>().ID)
                    {
                        SelectedUnits.Add(mouseHit.transform.root.gameObject);
                        mouseHit.transform.root.GetComponent<UnitStats>().IsSelected = true;
                    }
                }
                //Shipbase select
                if (mouseHit.transform.root.tag == "ShipBase")
                {
                    if (Tracker.ThePlayerControlPanel.ID == mouseHit.transform.root.GetComponent<UnitID>().ID)
                    {
                        SelectedUnits.Clear();
                        SelectedBuildings.Add(mouseHit.transform.root.gameObject);
                        mouseHit.transform.root.GetComponent<UnitStats>().IsSelected = true;
                    }
                }
            }
        }
    }

    //Clears list and unapply hover effect
    void ResetSelect()
    {
        foreach (GameObject unit in SelectedUnits)
        {
            if (unit != null) unit.GetComponent<UnitStats>().IsSelected = false;
        }
        foreach (GameObject unit in SelectedBuildings)
        {
            if (unit != null) unit.GetComponent<UnitStats>().IsSelected = false;
        }
        SelectedUnits.Clear();
        SelectedBuildings.Clear();
    }

    //Unselect all
    void ResetFake()
    {
        GameObject[] ShipBases = GameObject.FindGameObjectsWithTag("ShipBase");
        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");

        foreach (GameObject shipBase in ShipBases)
        {
            if (shipBase != null) shipBase.GetComponent<UnitStats>().IsSelected = false;
        }

        foreach (GameObject unit in units)
        {
            if (unit != null) unit.GetComponent<UnitStats>().IsSelected = false;
        }
    }
}
