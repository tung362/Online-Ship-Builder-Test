using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderBlock1 : MonoBehaviour
{
    /*Settings*/
    public bool IsPlaced = false;
    public bool SnapToAllSides = false;

    /*Data*/
    private Vector3 CurrentMousePosition = Vector3.zero;

    /*Required Components*/
    private BuilderSettings TheBuilderSettings;
    private BoxCollider TheBoxCollider;
    private Rigidbody TheRigidbody;

    void Start()
    {
        TheBuilderSettings = FindObjectOfType<BuilderSettings>();
        TheBoxCollider = GetComponent<BoxCollider>();
        TheRigidbody = GetComponent<Rigidbody>();
    }

    public void MoveBlock()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] mouseHits = Physics.RaycastAll(mouseRay);

        //Loop through each mouse hit
        for (int i = 0; i < mouseHits.Length; i++)
        {
            if (mouseHits[i].collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Vector3 hitPoint = new Vector3(mouseHits[i].point.x, transform.position.y, mouseHits[i].point.z);
                CurrentMousePosition = hitPoint;
            }
        }
    }
    public void UpdateSnapcalCalculation()
    {
        bool normalMove = true;
        if (!SnapToAllSides) normalMove = CalculateSingleSide();
        else normalMove = CalculateAllSides();

        float mouseDistance = Vector3.Distance(transform.TransformPoint(TheBoxCollider.center), CurrentMousePosition);
        if (mouseDistance > 0.6f) normalMove = true;

        if (normalMove) TheRigidbody.MovePosition(Vector3.MoveTowards(transform.position, CurrentMousePosition, 40 * Time.deltaTime));
    }

    bool CalculateSingleSide()
    {
        bool retval = false;
        RaycastHit hit = new RaycastHit();

        //Raycast
        float distanceBackward = RaycastSnapCheck(transform.TransformPoint(TheBoxCollider.center), -transform.forward, ref hit);

        //Ensures that the object is close enough to snap
        if (distanceBackward <= 0.4f)
        {
            if (hit.collider.transform.InverseTransformVector(hit.normal).x >= 0.5f || hit.collider.transform.InverseTransformVector(hit.normal).x <= -0.5f) transform.position = new Vector3(hit.point.x, transform.position.y, CurrentMousePosition.z);
            else if (hit.collider.transform.InverseTransformVector(hit.normal).z >= 0.5f || hit.collider.transform.InverseTransformVector(hit.normal).z <= -0.5f) transform.position = new Vector3(CurrentMousePosition.x, transform.position.y, hit.point.z);
        }
        else retval = true;
        return retval;
    }

    bool CalculateAllSides()
    {
        bool retval = false;
        RaycastHit[] hits = new RaycastHit[4];
        float[] distances = new float[4];

        //Dimension
        Vector3 Center = TheBoxCollider.center;
        Vector3 LeftPosition = new Vector3(TheBoxCollider.center.x - TheBoxCollider.size.x / 2, TheBoxCollider.center.y, TheBoxCollider.center.z);
        Vector3 RightPosition = new Vector3(TheBoxCollider.center.x + TheBoxCollider.size.x / 2, TheBoxCollider.center.y, TheBoxCollider.center.z);
        Vector3 ForwardPosition = new Vector3(TheBoxCollider.center.x, TheBoxCollider.center.y, TheBoxCollider.center.z + TheBoxCollider.size.z / 2);

        //Raycast
        distances[0] = RaycastSnapCheck(transform.TransformPoint(TheBoxCollider.center), -transform.right, ref hits[0]);
        distances[1] = RaycastSnapCheck(transform.TransformPoint(TheBoxCollider.center), transform.right, ref hits[1]);
        distances[2] = RaycastSnapCheck(transform.TransformPoint(TheBoxCollider.center), transform.forward, ref hits[2]);
        distances[3] = RaycastSnapCheck(transform.TransformPoint(TheBoxCollider.center), -transform.forward, ref hits[3]);

        float closestDistance = int.MaxValue;
        int ClosestDistanceIndex = 0;
        for (int i = 0; i < distances.Length; i++)
        {
            if (closestDistance > distances[i])
            {
                closestDistance = distances[i];
                ClosestDistanceIndex = i;
            }
        }

        //Ensures that the object is close enough to snap
        if (distances[ClosestDistanceIndex] <= 0.4f)
        {
            if (ClosestDistanceIndex == 0)
            {
                //transform.position = HitPoints[ClosestDistanceIndex] - LeftPosition;
                if (hits[ClosestDistanceIndex].collider.transform.InverseTransformVector(hits[ClosestDistanceIndex].normal).x >= 0.5f 
                    || hits[ClosestDistanceIndex].collider.transform.InverseTransformVector(hits[ClosestDistanceIndex].normal).x <= -0.5f)
                {
                    transform.position = new Vector3(hits[ClosestDistanceIndex].point.x - LeftPosition.x, transform.position.y, CurrentMousePosition.z);
                }
                else if (hits[ClosestDistanceIndex].collider.transform.InverseTransformVector(hits[ClosestDistanceIndex].normal).z >= 0.5f 
                    || hits[ClosestDistanceIndex].collider.transform.InverseTransformVector(hits[ClosestDistanceIndex].normal).z <= -0.5f)
                {
                    transform.position = new Vector3(CurrentMousePosition.x, transform.position.y, hits[ClosestDistanceIndex].point.z - LeftPosition.z);
                }
            }
            else if (ClosestDistanceIndex == 1)
            {
                //transform.position = HitPoints[ClosestDistanceIndex] - RightPosition;
                if (hits[ClosestDistanceIndex].collider.transform.InverseTransformVector(hits[ClosestDistanceIndex].normal).x >= 0.5f
                    || hits[ClosestDistanceIndex].collider.transform.InverseTransformVector(hits[ClosestDistanceIndex].normal).x <= -0.5f)
                {
                    transform.position = new Vector3(hits[ClosestDistanceIndex].point.x - RightPosition.x, transform.position.y, CurrentMousePosition.z);
                }
                else if (hits[ClosestDistanceIndex].collider.transform.InverseTransformVector(hits[ClosestDistanceIndex].normal).z >= 0.5f
                    || hits[ClosestDistanceIndex].collider.transform.InverseTransformVector(hits[ClosestDistanceIndex].normal).z <= -0.5f)
                {
                    transform.position = new Vector3(CurrentMousePosition.x, transform.position.y, hits[ClosestDistanceIndex].point.z - RightPosition.z);
                }
            }
            else if (ClosestDistanceIndex == 2)
            {
                //transform.position = HitPoints[ClosestDistanceIndex] - ForwardPosition;
                if (hits[ClosestDistanceIndex].collider.transform.InverseTransformVector(hits[ClosestDistanceIndex].normal).x >= 0.5f
                    || hits[ClosestDistanceIndex].collider.transform.InverseTransformVector(hits[ClosestDistanceIndex].normal).x <= -0.5f)
                {
                    transform.position = new Vector3(hits[ClosestDistanceIndex].point.x - ForwardPosition.x, transform.position.y, CurrentMousePosition.z);
                }
                else if (hits[ClosestDistanceIndex].collider.transform.InverseTransformVector(hits[ClosestDistanceIndex].normal).z >= 0.5f
                    || hits[ClosestDistanceIndex].collider.transform.InverseTransformVector(hits[ClosestDistanceIndex].normal).z <= -0.5f)
                {
                    transform.position = new Vector3(CurrentMousePosition.x, transform.position.y, hits[ClosestDistanceIndex].point.z - ForwardPosition.z);
                }
            }
            else if (ClosestDistanceIndex == 3)
            {
                //transform.position = HitPoints[ClosestDistanceIndex];
                if (hits[ClosestDistanceIndex].collider.transform.InverseTransformVector(hits[ClosestDistanceIndex].normal).x >= 0.5f
                    || hits[ClosestDistanceIndex].collider.transform.InverseTransformVector(hits[ClosestDistanceIndex].normal).x <= -0.5f)
                {
                    transform.position = new Vector3(hits[ClosestDistanceIndex].point.x, transform.position.y, CurrentMousePosition.z);
                }
                else if (hits[ClosestDistanceIndex].collider.transform.InverseTransformVector(hits[ClosestDistanceIndex].normal).z >= 0.5f
                    || hits[ClosestDistanceIndex].collider.transform.InverseTransformVector(hits[ClosestDistanceIndex].normal).z <= -0.5f)
                {
                    transform.position = new Vector3(CurrentMousePosition.x, transform.position.y, hits[ClosestDistanceIndex].point.z);
                }
            }
        }
        else retval = true;
        return retval;
    }

    public void Place()
    {
        //IsPlaced = true;
    }

    public void Detatch()
    {
        //IsPlaced = false;
    }

    float RaycastSnapCheck(Vector3 Start, Vector3 Direction, ref RaycastHit Hit)
    {
        RaycastHit[] hits = Physics.RaycastAll(Start, Direction);

        float closestDistance = int.MaxValue;

        //Find closest
        for (int i = 0; i < hits.Length; i++)
        {
            BuilderBlock1 theBuilderBlock = hits[i].collider.GetComponent<BuilderBlock1>();
            if (theBuilderBlock != null)
            {
                if (theBuilderBlock.IsPlaced)
                {
                    float distance = Vector3.Distance(Start, hits[i].point);
                    if (closestDistance > distance)
                    {
                        closestDistance = distance;
                        Hit = hits[i];
                    }
                }
            }
        }
        return closestDistance;
    }
}
