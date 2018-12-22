using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float ScrollSpeed = 5;
    public float ZoomSpeed = 3;
    public float ZoomRate = 0.5f;
    public float ZoomMin = 0.79f;
    public float ZoomMax = 10.79f;
    private float newMouseY = 0;


    void Start()
    {
        newMouseY = transform.position.y;
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow)) transform.position = transform.position + (new Vector3(0, 0, 1) * ScrollSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.DownArrow)) transform.position = transform.position + (new Vector3(0, 0, -1) * ScrollSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.LeftArrow)) transform.position = transform.position + (new Vector3(-1, 0, 0) * ScrollSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.RightArrow)) transform.position = transform.position + (new Vector3(1, 0, 0) * ScrollSpeed * Time.deltaTime);

        //Zoom
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            newMouseY += ZoomRate;
            if (newMouseY > ZoomMax) newMouseY = ZoomMax;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            newMouseY -= ZoomRate;
            if (newMouseY < ZoomMin) newMouseY = ZoomMin;
        }

        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, newMouseY, transform.position.z), ScrollSpeed * Time.deltaTime);
    }
}
