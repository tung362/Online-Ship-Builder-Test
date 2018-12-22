using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Camera controller for builder mode
public class BuilderCamera : MonoBehaviour
{
    /*Settings*/
    public GameObject FocusTarget;

    /*Data*/

    /*Required Components*/
    private BuilderSettings TheBuilderSettings;

    void Start()
    {
        TheBuilderSettings = FindObjectOfType<BuilderSettings>();
        TheBuilderSettings.GetComponent<BuilderSelector>().BuildCamera = GetComponent<Camera>();
    }
	
	void LateUpdate ()
    {
        UpdateMovement();
        UpdateZoom();
        UpdateRotation();
    }

    void UpdateMovement()
    {
        Vector3 Movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) Movement += new Vector3(transform.forward.x, 0, transform.forward.z);
        if (Input.GetKey(KeyCode.S)) Movement -= new Vector3(transform.forward.x, 0, transform.forward.z);
        if (Input.GetKey(KeyCode.A)) Movement -= new Vector3(transform.right.x, 0, transform.right.z);
        if (Input.GetKey(KeyCode.D)) Movement += new Vector3(transform.right.x, 0, transform.right.z);

        //Limits movement
        if(FocusTarget.transform.position.z > TheBuilderSettings.BuildLimit.y && Movement.z > 0 || FocusTarget.transform.position.z < -TheBuilderSettings.BuildLimit.y && Movement.z < 0) Movement.z = 0;
        if (FocusTarget.transform.position.x > TheBuilderSettings.BuildLimit.x && Movement.x > 0 || FocusTarget.transform.position.x < -TheBuilderSettings.BuildLimit.x && Movement.x < 0) Movement.x = 0;

        FocusTarget.transform.position += Movement.normalized * TheBuilderSettings.CameraMoveSpeed * Time.deltaTime;
    }

    void UpdateZoom()
    {
        float distance = Vector3.Distance(FocusTarget.transform.position, transform.position);
        float MouseZoom = Input.GetAxis("Mouse ScrollWheel");

        //Limit zoom
        if(distance > TheBuilderSettings.CameraMaxZoom && distance < TheBuilderSettings.CameraMinZoom) transform.position += transform.forward * MouseZoom * TheBuilderSettings.CameraZoomSpeed * Time.deltaTime;
        else if((distance < TheBuilderSettings.CameraMaxZoom && MouseZoom < 0) || distance > TheBuilderSettings.CameraMinZoom && MouseZoom > 0) transform.position += transform.forward * MouseZoom * TheBuilderSettings.CameraZoomSpeed * Time.deltaTime;
    }

    void UpdateRotation()
    {
        //If right clicked
        if (Input.GetMouseButton(1))
        {
            //Hides and lock mouse
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            float MouseX = -Input.GetAxis("Mouse X");
            float MouseY = -Input.GetAxis("Mouse Y");

            //Y angle calculation
            float angleY = Vector3.Angle(new Vector3(transform.forward.x, 0, transform.forward.z), transform.forward);
            float dotY = Vector3.Dot(Vector3.up, transform.forward);
            if (dotY > 0) angleY = -angleY;

            //Limit Y rotation
            if (angleY > TheBuilderSettings.CameraMinYRotation && angleY < TheBuilderSettings.CameraMaxYRotation) transform.RotateAround(FocusTarget.transform.position, transform.right, MouseY * TheBuilderSettings.CameraRotationSpeed * Time.deltaTime);
            else if ((angleY < TheBuilderSettings.CameraMinYRotation && MouseY > 0) || (angleY > TheBuilderSettings.CameraMaxYRotation && MouseY < 0)) transform.RotateAround(FocusTarget.transform.position, transform.right, MouseY * TheBuilderSettings.CameraRotationSpeed * Time.deltaTime);

            transform.RotateAround(FocusTarget.transform.position, FocusTarget.transform.up, MouseX * TheBuilderSettings.CameraRotationSpeed * Time.deltaTime);
        }
        else
        {
            //Shows mouse
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
