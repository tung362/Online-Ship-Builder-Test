using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Camera controller for lobby
public class LobbyCamera : MonoBehaviour
{
    /*Settings*/
    public GameObject FocusTarget;
    public Vector2 MoveLimit = new Vector2(3, 3);
    public float CameraRotationSpeed = 100;
    public float CameraMoveSpeed = 5;
    public float CameraMaxYRotation = 85;
    public float CameraMinYRotation = -10;
    public float CameraMaxZoom = 2;
    public float CameraMinZoom = 6;
    public float CameraZoomSpeed = 200;

    /*Data*/
    private bool StopUpdatingCamera = false;

    /*Required Components*/
    private MouseShow TheMouseShow;

    void Start()
    {
        TheMouseShow = FindObjectOfType<MouseShow>();
    }

    void LateUpdate()
    {
        UpdateMovement();
        UpdateZoom();
        UpdateRotation();
    }

    void UpdateMovement()
    {
        if (StopUpdatingCamera) return;

        Vector3 Movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) Movement += new Vector3(transform.forward.x, 0, transform.forward.z);
        if (Input.GetKey(KeyCode.S)) Movement -= new Vector3(transform.forward.x, 0, transform.forward.z);
        if (Input.GetKey(KeyCode.A)) Movement -= new Vector3(transform.right.x, 0, transform.right.z);
        if (Input.GetKey(KeyCode.D)) Movement += new Vector3(transform.right.x, 0, transform.right.z);

        //Limits movement
        if (FocusTarget.transform.position.z > MoveLimit.y && Movement.z > 0 || FocusTarget.transform.position.z < -MoveLimit.y && Movement.z < 0) Movement.z = 0;
        if (FocusTarget.transform.position.x > MoveLimit.x && Movement.x > 0 || FocusTarget.transform.position.x < -MoveLimit.x && Movement.x < 0) Movement.x = 0;

        FocusTarget.transform.position += Movement.normalized * CameraMoveSpeed * Time.deltaTime;
    }

    void UpdateZoom()
    {
        if (StopUpdatingCamera) return;

        float distance = Vector3.Distance(FocusTarget.transform.position, transform.position);
        float MouseZoom = Input.GetAxis("Mouse ScrollWheel");

        //Limit zoom
        if (distance > CameraMaxZoom && distance < CameraMinZoom) transform.position += transform.forward * MouseZoom * CameraZoomSpeed * Time.deltaTime;
        else if ((distance < CameraMaxZoom && MouseZoom < 0) || distance > CameraMinZoom && MouseZoom > 0) transform.position += transform.forward * MouseZoom * CameraZoomSpeed * Time.deltaTime;
    }

    void UpdateRotation()
    {
        if (StopUpdatingCamera) return;

        //If right clicked
        if (Input.GetMouseButton(1))
        {
            //Hides and lock mouse
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            TheMouseShow.DisableUpdateVisibility(true);

            float MouseX = -Input.GetAxis("Mouse X");
            float MouseY = -Input.GetAxis("Mouse Y");

            //Y angle calculation
            float angleY = Vector3.Angle(new Vector3(transform.forward.x, 0, transform.forward.z), transform.forward);
            float dotY = Vector3.Dot(Vector3.up, transform.forward);
            if (dotY > 0) angleY = -angleY;

            //Limit Y rotation
            if (angleY > CameraMinYRotation && angleY < CameraMaxYRotation) transform.RotateAround(FocusTarget.transform.position, transform.right, MouseY * CameraRotationSpeed * Time.deltaTime);
            else if ((angleY < CameraMinYRotation && MouseY > 0) || (angleY > CameraMaxYRotation && MouseY < 0)) transform.RotateAround(FocusTarget.transform.position, transform.right, MouseY * CameraRotationSpeed * Time.deltaTime);

            transform.RotateAround(FocusTarget.transform.position, FocusTarget.transform.up, MouseX * CameraRotationSpeed * Time.deltaTime);
        }
    }

    public void DisableUpdateCamera(bool TrueFalse)
    {
        StopUpdatingCamera = TrueFalse;
    }
}
