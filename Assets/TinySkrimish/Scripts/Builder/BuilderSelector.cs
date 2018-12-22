using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles moving and selection blocks
public class BuilderSelector : TungDoesMathForYou
{
    /*Settings*/

    /*Data*/
    //The ghost block that is currently selected
    [HideInInspector]
    public GameObject GhostBlock;
    //The ghost block mirrior that is currently selected
    [HideInInspector]
    public GameObject GhostBlockMirror;
    //[HideInInspector]
    public Camera BuildCamera;

    /*Required Components*/
    private BuilderSettings TheBuilderSettings;
    public BuilderRoot TheRoot;

    void Start()
    {
        TheBuilderSettings = FindObjectOfType<BuilderSettings>();
        TheRoot = FindObjectOfType<BuilderRoot>();
    }

	void Update()
    {
        UpdateInput();
    }

    void UpdateInput()
    {
        if (BuildCamera == null) return;

        //Adjacent check
        TheRoot.AdjacentCheck();

        if (TheBuilderSettings.PlacingNewObject)
        {
            //Rotate
            if (Input.GetKeyDown(KeyCode.Q)) RotateSelectedObject(true);
            if (Input.GetKeyDown(KeyCode.E)) RotateSelectedObject(false);

            UpdateGhost();
            if (Input.GetMouseButtonDown(0)) PlaceGhost();
        }
        else
        {
            //Rotate
            if (Input.GetKeyDown(KeyCode.Q)) RotateSelectedObject(true);
            if (Input.GetKeyDown(KeyCode.E)) RotateSelectedObject(false);

            //Start
            if (Input.GetMouseButtonDown(0)) Select();
            //Held
            if (Input.GetMouseButton(0)) Drag();
            //Reset
            if (Input.GetMouseButtonUp(0)) UnSelect();

            //Delete
            if (Input.GetKeyDown(KeyCode.Delete)) DeleteBlock();
        }
    }

    void UpdateGhost()
    {
        if (GhostBlock == null) return;
        GhostBlock.GetComponent<BuilderBlock>().UpdatePlacement();
        GhostBlock.GetComponent<BuilderBlock>().UpdatePlacementColor();
        if(TheBuilderSettings.Mirror)
        {
            GhostBlockMirror.GetComponent<BuilderBlock>().UpdateMirror(GhostBlock);
            GhostBlockMirror.GetComponent<BuilderBlock>().UpdatePlacementColor();
        }
    }

    void PlaceGhost()
    {
        if (GhostBlock == null) return;
        GhostBlock.GetComponent<BuilderBlock>().Place();
        if (TheBuilderSettings.Mirror)
        {
            float distance = Vector3.Distance(GhostBlock.transform.position, GhostBlockMirror.transform.position);
            if(distance > 0.1f) GhostBlockMirror.GetComponent<BuilderBlock>().Place();

        }
    }

    void Select()
    {
        GhostBlock = null;
        Ray mouseRay = BuildCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] mouseHits = Physics.RaycastAll(mouseRay);

        //Find closest hit point
        float ClosestDistance = int.MaxValue;
        RaycastHit hit = new RaycastHit();
        for(int i = 0; i < mouseHits.Length; i++)
        {
            if(mouseHits[i].collider.gameObject.layer == LayerMask.NameToLayer("Build") && mouseHits[i].collider.tag != "Ghost")
            {
                float distance = Vector3.Distance(BuildCamera.transform.position, mouseHits[i].point);
                if(ClosestDistance > distance)
                {
                    ClosestDistance = distance;
                    hit = mouseHits[i];
                }
            }
        }

        if(hit.collider != null)
        {
            if (hit.collider.GetComponent<BuilderColliderRoot>().Root.GetComponent<BuilderIgnoreSelection>() == null)
            {
                GhostBlock = hit.collider.GetComponent<BuilderColliderRoot>().Root;
                GhostBlock.GetComponent<BuilderBlock>().SetPreviousPositionAndRotation();
            }
            else GhostBlock = null;
        }
    }

    void Drag()
    {
        if (GhostBlock == null) return;
        GhostBlock.GetComponent<BuilderBlock>().UpdatePlacement();
        GhostBlock.GetComponent<BuilderBlock>().UpdatePlacementColor();
    }

    void UnSelect()
    {
        if (GhostBlock == null) return;
        GhostBlock.GetComponent<BuilderBlock>().UnselectCheck();
        GhostBlock = null;
    }

    void FixRotation()
    {
        if (GhostBlock == null) return;
        float roundedAngle = Mathf.Round(GhostBlock.transform.eulerAngles.y / 90.0f);
        GhostBlock.transform.eulerAngles = new Vector3(0, roundedAngle * 90.0f, 0);
    }

    void RotateSelectedObject(bool Left)
    {
        if (GhostBlock == null) return;
        if (Left) GhostBlock.transform.eulerAngles -= new Vector3(0, 90, 0);
        else GhostBlock.transform.eulerAngles += new Vector3(0, 90, 0);
    }

    void DeleteBlock()
    {
        if (GhostBlock == null) return;
        if (!GhostBlock.GetComponent<BuilderBlock>().Ghost) Destroy(GhostBlock);
    }
}
