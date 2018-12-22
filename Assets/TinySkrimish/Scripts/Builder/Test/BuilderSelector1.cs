using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles moving and selection blocks
public class BuilderSelector1 : MonoBehaviour
{
    /*Settings*/

    /*Data*/
    [HideInInspector]
    public GameObject SelectedObject;

	void Start ()
    {
		
	}

	void Update ()
    {
        UpdateInput();
    }

    void UpdateInput()
    {
        //Start
        if (Input.GetMouseButtonDown(0)) Select();
        //Held
        if (Input.GetMouseButton(0)) Drag();
        //Reset
        if (Input.GetMouseButtonUp(0)) ResetSelection();
        //Rotate
        if (Input.GetKeyDown(KeyCode.Q)) RotateSelectedObject(true);
        if (Input.GetKeyDown(KeyCode.E)) RotateSelectedObject(false);
    }

    void Select()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] mouseHits = Physics.RaycastAll(mouseRay);

        //Loop through each mouse hit
        for(int i = 0; i < mouseHits.Length; i++)
        {
            BuilderBlock1 theBuilderBlock = mouseHits[i].collider.GetComponent<BuilderBlock1>();
            BuilderIgnoreSelection theBuilderIgnoreSelection = mouseHits[i].collider.GetComponent<BuilderIgnoreSelection>();

            if (theBuilderIgnoreSelection == null && theBuilderBlock != null)
            {
                SelectedObject = mouseHits[i].collider.gameObject;
                SelectedObject.GetComponent<Rigidbody>().isKinematic = false;
                SelectedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
                SelectedObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                FixRotation();
                SelectedObject.GetComponent<BuilderBlock1>().Detatch();
                break;
            }
        }
    }

    void Drag()
    {
        if (SelectedObject == null) return;

        SelectedObject.GetComponent<BuilderBlock1>().MoveBlock();
        SelectedObject.GetComponent<BuilderBlock1>().UpdateSnapcalCalculation();
    }

    void ResetSelection()
    {
        if (SelectedObject == null) return;
        //Remove later
        SelectedObject.GetComponent<Rigidbody>().isKinematic = true;
        SelectedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        SelectedObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        SelectedObject.GetComponent<BuilderBlock1>().Place();
        SelectedObject = null;
    }

    void FixRotation()
    {
        if (SelectedObject == null) return;
        float roundedAngle = Mathf.Round(SelectedObject.transform.eulerAngles.y / 90.0f);
        SelectedObject.transform.eulerAngles = new Vector3(0, roundedAngle * 90.0f, 0);

    }

    void RotateSelectedObject(bool Left)
    {
        if (SelectedObject == null) return;

        if(Left) SelectedObject.transform.eulerAngles -= new Vector3(0, 90, 0);
        else SelectedObject.transform.eulerAngles += new Vector3(0, 90, 0);
    }
}
