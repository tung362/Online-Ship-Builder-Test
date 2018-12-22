using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Camera controller for lobby small ship box
public class LobbySmallBoxCamera : MonoBehaviour
{
    /*Settings*/
    public GameObject FocusTarget;
    public Vector2 MoveLimit = new Vector2(3, 3);
    public float CameraRotationSpeed = 100;
    public float CameraMaxZoom = 1;
    public float CameraMinZoom = 2.5f;
    public float CameraZoomSpeed = 200;

    /*Data*/
    public bool IsSelected = false;
    public bool CanMove = false;

    /*Required Components*/
    private MouseShow TheMouseShow;

    void Start()
    {
        TheMouseShow = FindObjectOfType<MouseShow>();
    }

    void LateUpdate()
    {
        UpdateInput();
        UpdateZoom();
        UpdateRotation();
    }

    public void UpdateInput()
    {
        //Start
        if (Input.GetMouseButtonDown(0) && IsSelected) CanMove = true;
        //End
        if (Input.GetMouseButtonUp(0)) CanMove = false;
    }

    void UpdateZoom()
    {
        if (!IsSelected) return;

        float distance = Vector3.Distance(FocusTarget.transform.position, transform.position);
        float MouseZoom = Input.GetAxis("Mouse ScrollWheel");

        //Limit zoom
        if (distance > CameraMaxZoom && distance < CameraMinZoom) transform.position += transform.forward * MouseZoom * CameraZoomSpeed * Time.deltaTime;
        else if ((distance < CameraMaxZoom && MouseZoom < 0) || distance > CameraMinZoom && MouseZoom > 0) transform.position += transform.forward * MouseZoom * CameraZoomSpeed * Time.deltaTime;
    }

    void UpdateRotation()
    {
        if (!CanMove) return;

        //Hides and lock mouse
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        TheMouseShow.DisableUpdateVisibility(true);

        float MouseX = -Input.GetAxis("Mouse X");
        float MouseY = -Input.GetAxis("Mouse Y");

        transform.RotateAround(FocusTarget.transform.position, transform.right, MouseY * CameraRotationSpeed * Time.deltaTime);
        transform.RotateAround(FocusTarget.transform.position, FocusTarget.transform.up, MouseX * CameraRotationSpeed * Time.deltaTime);
    }

    public void SetIsSelected(bool TrueFalse)
    {
        IsSelected = TrueFalse;
    }
}
