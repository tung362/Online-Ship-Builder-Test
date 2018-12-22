using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using System.Linq;

//Handles all unit ai states
public class UnitAI : NetworkBehaviour
{
    /*Settings*/
    //How close to the destination before ship stops moving
    public float SleepThreshhold = 0.1f;
    //How close to the target vefore ship stops moving
    public float PursuitSleepThreshold = 10;
    //How far away the ship can chase the target before retreating back to it's orginal position
    public float MaxPursuitDistance = 15;

    /*Data*/
    //0 = stay still, 1 = defensive, 2 = aggressive
    private int AgressiveLevel = 1;
    private bool Move = false;
    private Vector3 MovePosition = Vector3.zero;
    private GameObject BuildingTarget;
    private GameObject UnitTarget;
    private GameObject TemperaryTarget;
    private Vector3 PersuitStartingPosition;
    private int CurrentPathCorner = 1;

    /*Required Componenets*/
    private NavMeshPath Path;
    private Rigidbody TheRigidbody;

    void Start ()
    {
        Path = new NavMeshPath();
        TheRigidbody = GetComponent<Rigidbody>();
        PersuitStartingPosition = transform.position;
    }
	
	void FixedUpdate()
    {
        if (!isServer) return;
        CheckForNearbyUnits();
        UpdateAI();
    }

    void CheckForNearbyUnits()
    {
        GameObject target = null;
        float closestDistance = int.MaxValue;
        Collider[] others = Physics.OverlapSphere(transform.position, MaxPursuitDistance, 1 << LayerMask.NameToLayer("ShipTracker"));
        for (int i = 0; i < others.Length; i++)
        {
            if (others[i].transform.root.tag == "Unit")
            {
                if (others[i].transform.root.GetComponent<UnitID>().ID != GetComponent<UnitID>().ID)
                {
                    float distance = Vector3.Distance(transform.position, others[i].transform.root.position);
                    if (distance < closestDistance)
                    {
                        target = others[i].transform.root.gameObject;
                        closestDistance = distance;
                    }
                }
            }
        }
        if (target != null) TemperaryTarget = target;

        //GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
        //GameObject[] bases = GameObject.FindGameObjectsWithTag("ShipBase");
        //GameObject[] allUnits = units.Concat(bases).ToArray();
        //GameObject target = null;
        //float closestDistance = int.MaxValue;
        //for (int i = 0; i < allUnits.Length; i++)
        //{
        //    if (allUnits[i].GetComponent<UnitID>().ID != GetComponent<UnitID>().ID)
        //    {
        //        float distance = Vector3.Distance(transform.position, allUnits[i].transform.position);
        //        if (distance < closestDistance)
        //        {
        //            target = allUnits[i].transform.gameObject;
        //            closestDistance = distance;
        //        }
        //    }
        //}
        //if (target != null) TemperaryTarget = target;
    }

    void UpdateAI()
    {
        //Move
        if (Move)
        {
            float distance = Vector3.Distance(transform.position, MovePosition);
            //Move
            if(distance > SleepThreshhold) MoveShip(true, Vector3.zero);
            //Stop
            else
            {
                TheRigidbody.velocity = Vector3.zero;
                TheRigidbody.angularVelocity = Vector3.zero;
                PersuitStartingPosition = transform.position;
                Move = false;
            }
        }
        //Attack Building
        else if(BuildingTarget != null)
        {
            GetComponent<UnitID>().Target = BuildingTarget;
            float distance = Vector3.Distance(transform.position, BuildingTarget.transform.position);
            if (distance > PursuitSleepThreshold) MoveShip(false, BuildingTarget.transform.position);
            else
            {
                Vector3 direction = (new Vector3(BuildingTarget.transform.position.x, transform.position.y, BuildingTarget.transform.position.z) - transform.position).normalized;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5 * Time.fixedDeltaTime);
                TheRigidbody.velocity = Vector3.zero;
                TheRigidbody.angularVelocity = Vector3.zero;
            }

            PersuitStartingPosition = transform.position;
        }
        //Attack unit
        else if(UnitTarget != null)
        {
            GetComponent<UnitID>().Target = UnitTarget;
            float distance = Vector3.Distance(transform.position, UnitTarget.transform.position);
            if (distance > PursuitSleepThreshold) MoveShip(false, UnitTarget.transform.position);
            else
            {
                Vector3 direction = (new Vector3(UnitTarget.transform.position.x, transform.position.y, UnitTarget.transform.position.z) - transform.position).normalized;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5 * Time.fixedDeltaTime);
                TheRigidbody.velocity = Vector3.zero;
                TheRigidbody.angularVelocity = Vector3.zero;
            }

            PersuitStartingPosition = transform.position;
        }
        //Attack temperary units
        else
        {
            float distanceFromStart = Vector3.Distance(transform.position, PersuitStartingPosition);
            if (TemperaryTarget != null)
            {
                GetComponent<UnitID>().Target = TemperaryTarget;
                float distance = Vector3.Distance(transform.position, TemperaryTarget.transform.position);
                float distanceFromTargetToStart = Vector3.Distance(PersuitStartingPosition, TemperaryTarget.transform.position);

                //Stay still
                if (AgressiveLevel == 0)
                {
                    if (distanceFromTargetToStart <= MaxPursuitDistance)
                    {
                        Vector3 direction = (new Vector3(TemperaryTarget.transform.position.x, transform.position.y, TemperaryTarget.transform.position.z) - transform.position).normalized;
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5 * Time.fixedDeltaTime);
                    }
                }
                //Defensive
                else if(AgressiveLevel == 1)
                {
                    //Chase
                    if (distanceFromTargetToStart <= MaxPursuitDistance)
                    {
                        if (distance > PursuitSleepThreshold) MoveShip(false, TemperaryTarget.transform.position);
                        else
                        {
                            Vector3 direction = (new Vector3(TemperaryTarget.transform.position.x, transform.position.y, TemperaryTarget.transform.position.z) - transform.position).normalized;
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5 * Time.fixedDeltaTime);
                            TheRigidbody.velocity = Vector3.zero;
                            TheRigidbody.angularVelocity = Vector3.zero;
                        }
                    }
                    //Go back if its further than the max pursuit distance
                    else
                    {
                        if (distanceFromStart > SleepThreshhold) MoveShip(false, PersuitStartingPosition);
                        else
                        {
                            TheRigidbody.velocity = Vector3.zero;
                            TheRigidbody.angularVelocity = Vector3.zero;
                        }
                    }
                }
                //Aggressive
                else if(AgressiveLevel == 2)
                {
                    if (distance > PursuitSleepThreshold) MoveShip(false, TemperaryTarget.transform.position);
                    else
                    {
                        Vector3 direction = (new Vector3(TemperaryTarget.transform.position.x, transform.position.y, TemperaryTarget.transform.position.z) - transform.position).normalized;
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5 * Time.fixedDeltaTime);
                        TheRigidbody.velocity = Vector3.zero;
                        TheRigidbody.angularVelocity = Vector3.zero;
                    }
                }
            }
            //Go back if there is no target
            else
            {
                if (distanceFromStart > SleepThreshhold) MoveShip(false, PersuitStartingPosition);
                else
                {
                    TheRigidbody.velocity = Vector3.zero;
                    TheRigidbody.angularVelocity = Vector3.zero;
                }
            }
        }
    }

    void MoveShip(bool FixedPosition, Vector3 TargetPosition)
    {
        if(FixedPosition)
        {
            if (Path.corners.Length > CurrentPathCorner)
            {
                float distance = Vector3.Distance(transform.position, Path.corners[CurrentPathCorner]);
                //Move
                if (distance > SleepThreshhold)
                {
                    Vector3 direction = (new Vector3(Path.corners[CurrentPathCorner].x, transform.position.y, Path.corners[CurrentPathCorner].z) - transform.position).normalized;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5 * Time.fixedDeltaTime);
                    TheRigidbody.velocity = direction * 100 * Time.fixedDeltaTime;
                }
                else CurrentPathCorner += 1;
            }
        }
        else
        {
            //Calculate path every 5 frames to prevent lag
            NavMesh.CalculatePath(transform.position, TargetPosition, NavMesh.AllAreas, Path);

            if (Path.corners.Length > 1)
            {
                Vector3 direction = (new Vector3(Path.corners[1].x, transform.position.y, Path.corners[1].z) - transform.position).normalized;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5 * Time.fixedDeltaTime);
                TheRigidbody.velocity = direction * 100 * Time.fixedDeltaTime;
                //TheRigidbody.velocity = transform.forward * 100 * Time.fixedDeltaTime;
            }
        }
    }

    [ServerCallback]
    public void SetAgressiveLevel(int Level)
    {
        AgressiveLevel = Level;
        PersuitStartingPosition = transform.position;
    }

    [ServerCallback]
    public void SetMove(Vector3 Position)
    {
        Move = true;
        MovePosition = Position;
        NavMesh.CalculatePath(transform.position, MovePosition, NavMesh.AllAreas, Path);
        CurrentPathCorner = 1;
        BuildingTarget = null;
        UnitTarget = null;
    }

    [ServerCallback]
    public void SetAttackBuilding(GameObject Target)
    {
        BuildingTarget = Target;
        Move = false;
        UnitTarget = null;
    }

    [ServerCallback]
    public void SetAttackUnit(GameObject Target)
    {
        UnitTarget = Target;
        Move = false;
        BuildingTarget = null;
    }
}
