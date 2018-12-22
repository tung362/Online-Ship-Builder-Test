using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectanglePostProcess : MonoBehaviour
{
    /*Settings*/
    //Color of the line
    public Material BorderMaterial;

    /*Data*/
    private Vector3 StartingMousePosition = -Vector3.one;
    private Vector3 EndingMousePosition = -Vector3.one;
    private Vector3 StartingHitPostion = -Vector3.one;
    private Vector3 EndingHitPostion = -Vector3.one;

    private bool ShowSelectionBox = false;

    /*Required Components*/
    private Camera Cam;

    void Start()
    {
        Cam = GetComponent<Camera>();
    }

    void Update()
    {
        UpdateRectangle();
    }

    void UpdateRectangle()
    {
        //Start
        if (Input.GetMouseButtonDown(0))
        {
            ShowSelectionBox = true;
            StartingHitPostion = RaycastMousePosition();
        }
        //Held
        if (Input.GetMouseButton(0))
        {
            EndingHitPostion = RaycastMousePosition();
            CalculateMousePosition();
        }
        //Reset
        if (Input.GetMouseButtonUp(0))
        {
            StartingMousePosition = -Vector3.one;
            ShowSelectionBox = false;
        }
    }

    void CalculateMousePosition()
    {
        Vector3 startingScreenPoint = Cam.WorldToScreenPoint(StartingHitPostion);
        Vector3 endingScreenPoint = Cam.WorldToScreenPoint(EndingHitPostion);
        StartingMousePosition = new Vector3(startingScreenPoint.x / Screen.width, startingScreenPoint.y / Screen.height, 0);
        EndingMousePosition = new Vector3(endingScreenPoint.x / Screen.width, endingScreenPoint.y / Screen.height, 0);
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

    public void OnPostRender()
    {
        if (!ShowSelectionBox) return;
        GL.PushMatrix();
        //Apply material
        if (BorderMaterial.SetPass(0))
        {
            //Othographic mode
            GL.LoadOrtho();
            //Start drawing lines
            GL.Begin(GL.LINES);
            {
                //Top
                GL.Vertex(StartingMousePosition);
                GL.Vertex(new Vector3(EndingMousePosition.x, StartingMousePosition.y, 0));

                //Right
                GL.Vertex(new Vector3(EndingMousePosition.x, StartingMousePosition.y, 0));
                GL.Vertex(EndingMousePosition);

                //Bottom
                GL.Vertex(EndingMousePosition);
                GL.Vertex(new Vector3(StartingMousePosition.x, EndingMousePosition.y, 0));

                //Left
                GL.Vertex(new Vector3(StartingMousePosition.x, EndingMousePosition.y, 0));
                GL.Vertex(StartingMousePosition);
            }
            GL.End();
        }
        GL.PopMatrix();
    }
}
