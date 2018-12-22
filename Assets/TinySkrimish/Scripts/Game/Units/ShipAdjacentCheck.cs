using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShipAdjacentCheck : TungDoesNetworkingForyou
{
    /*Settings*/
    public LineRenderer Tracer;
    public GameObject RepairParticlePrefab;

    /*Data*/
    public bool Connected = false;
    //Is this block being grabbed by a nearby block?
    public bool BeingGrabbed = false;
    //List of schematic block ids thats missing
    public List<int> MissingPartsID = new List<int>(); //Note to self: look through the schematic ids for the names
    //The object that will be grabbed towards the block
    [SyncVar]
    public GameObject GrabTarget;
    public int GrabTargetPartID = -1;
    private List<GameObject> RepairParticles = new List<GameObject>();

    /*Required components*/
    private ManagerTracker Tracker;

    void Start()
    {
        Tracker = GameObject.FindGameObjectWithTag("ManagerTracker").GetComponent<ManagerTracker>();
    }

    //To do: add line renderer
    void Update()
    {
        if (transform.root.GetComponent<ShipSchematic>() != null)
        {
            //Checks if any functions are even being used if not disable component (for optimizing)
            int NumberOfFunctionCalls = 0;
            if (isServer)
            {
                NumberOfFunctionCalls += LookForMissingPiece();
                NumberOfFunctionCalls += MoveGrabTarget();
                NumberOfFunctionCalls += UpdateRegenTimers();
            }

            NumberOfFunctionCalls += UpdateVisuals();

            if (NumberOfFunctionCalls == 0) this.enabled = false;
        }
        else this.enabled = false;
    }

    int LookForMissingPiece()
    {
        int retVal = 0;
        if (MissingPartsID.Count == 0) return retVal;
        if (GrabTarget != null) return retVal;
        retVal = 1;

        //Get all near objects
        Collider[] others = Physics.OverlapSphere(transform.position, 8, 1 << LayerMask.NameToLayer("Unattached"));
        //float closestDistance = int.MaxValue;
        GameObject closestObject = null;
        //Loop through missing block ids
        for (int i = 0; i < MissingPartsID.Count; i++)
        {
            //Loop through each hit object
            for (int j = 0; j < others.Length; j++)
            {
                //Only look for blocks
                if (others[j].tag == "Block")
                {
                    string partName = transform.root.GetComponent<ShipSchematic>().GetSchematic().Parts[MissingPartsID[i]].Name;
                    partName = partName.Replace(others[j].name, "");

                    //If the nearby block's name matches the missing block's name
                    if (partName == "Builder" &&
                        !others[j].GetComponent<ShipAdjacentCheck>().BeingGrabbed &&
                        others[j].transform.root.GetComponent<ShipSchematic>() == null)
                    {
                        ////Find closest matching block to grab
                        //float Distance = Vector3.Distance(transform.position, others[j].transform.position);
                        //if (closestDistance > Distance)
                        //{
                        //    closestDistance = Distance;
                        //    closestObject = others[j].gameObject;
                        //    GrabTargetPartID = i;
                        //}
                        closestObject = others[j].gameObject;
                        GrabTargetPartID = i;
                        break;
                    }
                }
            }
        }
        if (closestObject != null)
        {
            GrabTarget = closestObject;
            GrabTarget.GetComponent<ShipAdjacentCheck>().BeingGrabbed = true;
            GrabTarget.GetComponent<BoxCollider>().isTrigger = true;
            RpcGrabbed(GrabTarget);
        }

        return retVal;
    }

    int MoveGrabTarget()
    {
        int retVal = 0;
        if (GrabTarget == null) return retVal;
        retVal = 1;

        GrabTarget.transform.position = new Vector3(GrabTarget.transform.position.x, transform.root.position.y, GrabTarget.transform.position.z);
        Vector3 direction = (transform.position - GrabTarget.transform.position).normalized;
        GrabTarget.GetComponent<Rigidbody>().velocity = direction * 100 * Time.deltaTime;

        return retVal;
    }

    int UpdateVisuals()
    {
        int retVal = 0;
        if (GrabTarget != null)
        {
            retVal = 1;

            Tracer.SetPosition(0, transform.position);
            Tracer.SetPosition(1, GrabTarget.transform.position);
        }
        else
        {
            Tracer.SetPosition(0, Vector3.zero);
            Tracer.SetPosition(1, Vector3.zero);

        }

        if (isServer)
        {
            //Dynamic
            for (int i = 0; i < 1; ++i)
            {
                //Add to list to fit connection count
                if (RepairParticles.Count < MissingPartsID.Count)
                {
                    GameObject spawnedRepairParticle = Instantiate(RepairParticlePrefab, Vector3.zero, Quaternion.identity);
                    ParticleSystem.MainModule main = spawnedRepairParticle.GetComponent<ParticleSystem>().main;
                    main.startColor = Tracker.ThePlayerControlPanel.TheResourceManager.PlayerColor[transform.root.GetComponent<UnitID>().ID].TrimEmissionColor;
                    spawnedRepairParticle.transform.parent = transform.root;
                    NetworkServer.Spawn(spawnedRepairParticle);
                    RepairParticles.Add(spawnedRepairParticle);
                    RpcRepairParticleColor(spawnedRepairParticle, Tracker.ThePlayerControlPanel.TheResourceManager.PlayerColor[transform.root.GetComponent<UnitID>().ID].TrimEmissionColor);
                    //Recheck
                    i -= 1;
                }
                //remove from list to fit connection count
                if (RepairParticles.Count > MissingPartsID.Count)
                {
                    Destroy(RepairParticles[RepairParticles.Count - 1]);
                    RepairParticles.RemoveAt(RepairParticles.Count - 1);
                    //Recheck
                    i -= 1;
                }
            }

            for (int i = 0; i < RepairParticles.Count; ++i)
            {
                retVal = 1;
                ShipSchematic theShipSchematic = transform.root.GetComponent<ShipSchematic>();
                //RpcRepairParticle(RepairParticles[i], transform.root.gameObject, new Vector3(theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Position[0], theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Position[1], theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Position[2]));
                if(RepairParticles[i] != null) RepairParticles[i].transform.localPosition = new Vector3(theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Position[0], theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Position[1], theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Position[2]);
                //RepairParticles[i].transform.localPosition = Vector3.zero;
            }
        }

        return retVal;
    }

    int UpdateRegenTimers()
    {
        int retVal = 0;
        if (transform.root.GetComponent<ShipSchematic>().StartRegeneratingTimer < 5) return retVal;
        //Regenerates missing parts if its not grabbing a nearby block
        for (int i = 0; i < MissingPartsID.Count; i++)
        {
            retVal = 1;

            ShipSchematic theShipSchematic = transform.root.GetComponent<ShipSchematic>();
            //if (GrabTargetPartID != i || GrabTarget == null)
            //{
            //}
            //else theShipSchematic.TrackedBlockRegenTimers[MissingPartsID[i]] = 0;
            theShipSchematic.TrackedBlockRegenTimers[MissingPartsID[i]] += Time.deltaTime;
            if (theShipSchematic.TrackedBlockRegenTimers[MissingPartsID[i]] >= 4)
            {
                bool checkAdjacent = false;
                string partName = theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Name;
                for (int j = 0; j < Tracker.NetworkBlocks.Count; j++)
                {
                    partName = partName.Replace(Tracker.NetworkBlocks[j].name, "");
                    if (partName == "Builder")
                    {
                        GameObject spawnedBlock = Instantiate(Tracker.NetworkBlocks[j], Vector3.zero, Quaternion.identity);
                        spawnedBlock.name = Tracker.NetworkBlocks[j].name;
                        NetworkServer.Spawn(spawnedBlock);
                        spawnedBlock.transform.parent = transform.root;
                        spawnedBlock.transform.localPosition = new Vector3(theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Position[0], theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Position[1], theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Position[2]);
                        spawnedBlock.transform.localEulerAngles = new Vector3(theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Rotation[0], theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Rotation[1], theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Rotation[2]);
                        theShipSchematic.TrackedBlocks[MissingPartsID[i]] = spawnedBlock;
                        spawnedBlock.GetComponent<ShipAdjacentCheck>().BeingGrabbed = false;

                        //Physics
                        spawnedBlock.layer = LayerMask.NameToLayer("Attached");
                        ApplyLayerToChilds(spawnedBlock.transform, "Attached");
                        spawnedBlock.GetComponent<Rigidbody>().isKinematic = true;
                        spawnedBlock.GetComponent<BoxCollider>().isTrigger = false;
                        spawnedBlock.GetComponent<NetworkTransform>().enabled = false;

                        //Rpc info
                        GameObject rpcGrabbedObject = spawnedBlock;
                        GameObject rpcRoot = transform.root.gameObject;
                        Vector3 rpcPosition = new Vector3(theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Position[0], theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Position[1], theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Position[2]);
                        Vector3 rpcRotation = new Vector3(theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Rotation[0], theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Rotation[1], theShipSchematic.GetSchematic().Parts[MissingPartsID[i]].Rotation[2]);

                        if(GrabTarget != null) GrabTarget.GetComponent<ShipAdjacentCheck>().BeingGrabbed = false;

                        //Loop
                        checkAdjacent = true;

                        //Cancel
                        theShipSchematic.TrackedBlockRegenTimers[MissingPartsID[i]] = 0;
                        MissingPartsID.RemoveAt(i);
                        GrabTargetPartID = -1;
                        GrabTarget = null;
                        checkAdjacent = true;
                        i -= 1;
                        RpcRegen(rpcGrabbedObject, rpcRoot, rpcPosition, rpcRotation);
                        break;
                    }
                }
                if (checkAdjacent)
                {
                    transform.root.GetComponent<ShipMeshCombine>().RequestCombine();
                    theShipSchematic.RequestAdjacentCheck();
                }
            }
        }
        return retVal;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;
        if (GrabTargetPartID == -1) return;
        if (GrabTarget == null) return;

        if (other.tag == "Block")
        {
            if (other.gameObject == GrabTarget)
            {
                ShipSchematic theShipSchematic = transform.root.GetComponent<ShipSchematic>();
                GrabTarget.transform.parent = transform.root;
                GrabTarget.transform.localPosition = new Vector3(theShipSchematic.GetSchematic().Parts[MissingPartsID[GrabTargetPartID]].Position[0], theShipSchematic.GetSchematic().Parts[MissingPartsID[GrabTargetPartID]].Position[1], theShipSchematic.GetSchematic().Parts[MissingPartsID[GrabTargetPartID]].Position[2]);
                GrabTarget.transform.localEulerAngles = new Vector3(theShipSchematic.GetSchematic().Parts[MissingPartsID[GrabTargetPartID]].Rotation[0], theShipSchematic.GetSchematic().Parts[MissingPartsID[GrabTargetPartID]].Rotation[1], theShipSchematic.GetSchematic().Parts[MissingPartsID[GrabTargetPartID]].Rotation[2]);
                theShipSchematic.TrackedBlocks[MissingPartsID[GrabTargetPartID]] = GrabTarget;

                //Physics
                GrabTarget.layer = LayerMask.NameToLayer("Attached");
                ApplyLayerToChilds(GrabTarget.transform, "Attached");
                GrabTarget.GetComponent<Rigidbody>().isKinematic = true;
                GrabTarget.GetComponent<BoxCollider>().isTrigger = false;
                GrabTarget.GetComponent<NetworkTransform>().enabled = false;

                //Rpc info
                GameObject rpcGrabbedObject = GrabTarget;
                GameObject rpcRoot = transform.root.gameObject;
                Vector3 rpcPosition = new Vector3(theShipSchematic.GetSchematic().Parts[MissingPartsID[GrabTargetPartID]].Position[0], theShipSchematic.GetSchematic().Parts[MissingPartsID[GrabTargetPartID]].Position[1], theShipSchematic.GetSchematic().Parts[MissingPartsID[GrabTargetPartID]].Position[2]);
                Vector3 rpcRotation = new Vector3(theShipSchematic.GetSchematic().Parts[MissingPartsID[GrabTargetPartID]].Rotation[0], theShipSchematic.GetSchematic().Parts[MissingPartsID[GrabTargetPartID]].Rotation[1], theShipSchematic.GetSchematic().Parts[MissingPartsID[GrabTargetPartID]].Rotation[2]);

                //Cancel
                GrabTarget.GetComponent<ShipAdjacentCheck>().BeingGrabbed = false;
                MissingPartsID.RemoveAt(GrabTargetPartID);
                GrabTargetPartID = -1;
                GrabTarget = null;

                //Loop
                theShipSchematic.RequestAdjacentCheck();
                transform.root.GetComponent<ShipMeshCombine>().RequestCombine();

                RpcGrabCollide(rpcGrabbedObject, rpcRoot, rpcPosition, rpcRotation);
            }
        }
    }

    public void ResetGrabbed()
    {
        if (GrabTarget == null) return;
        GrabTarget.transform.parent = null;
        GrabTarget.GetComponent<Rigidbody>().velocity = Vector3.zero;
        GrabTarget.GetComponent<Rigidbody>().isKinematic = false;
        GrabTarget.GetComponent<ShipAdjacentCheck>().BeingGrabbed = false;
        GrabTargetPartID = -1;
        GrabTarget = null;
    }

    private void OnDisable()
    {
        GrabTargetPartID = -1;
        GrabTarget = null;
        BeingGrabbed = false;
        Tracer.SetPosition(0, Vector3.zero);
        Tracer.SetPosition(1, Vector3.zero);
    }

    [ClientRpc]
    void RpcGrabbed(GameObject GrabbedObject)
    {
        if (isServer) return;
        if (GrabbedObject == null) return;
        GrabbedObject.GetComponent<BoxCollider>().isTrigger = true;
    }

    [ClientRpc]
    void RpcRegen(GameObject GrabbedObject, GameObject Root, Vector3 Position, Vector3 RotationEuler)
    {
        if (isServer) return;
        if (GrabbedObject == null) return;
        if (Root == null) return;

        GrabbedObject.transform.parent = Root.transform;
        GrabbedObject.transform.localPosition = Position;
        GrabbedObject.transform.localEulerAngles = RotationEuler;

        GrabbedObject.layer = LayerMask.NameToLayer("Attached");
        ApplyLayerToChilds(GrabbedObject.transform, "Attached");
        GrabbedObject.GetComponent<Rigidbody>().isKinematic = true;
        GrabbedObject.GetComponent<BoxCollider>().isTrigger = false;
        GrabbedObject.GetComponent<NetworkTransform>().enabled = false;
        Root.GetComponent<ShipMeshCombine>().RequestCombine();
    }

    [ClientRpc]
    void RpcGrabCollide(GameObject GrabbedObject, GameObject Root, Vector3 Position, Vector3 RotationEuler)
    {
        if (isServer) return;
        if (GrabbedObject == null) return;
        if (Root == null) return;

        GrabbedObject.transform.parent = Root.transform;
        GrabbedObject.transform.localPosition = Position;
        GrabbedObject.transform.localEulerAngles = RotationEuler;

        GrabbedObject.layer = LayerMask.NameToLayer("Attached");
        ApplyLayerToChilds(GrabbedObject.transform, "Attached");
        GrabbedObject.GetComponent<Rigidbody>().isKinematic = true;
        GrabbedObject.GetComponent<BoxCollider>().isTrigger = false;
        GrabbedObject.GetComponent<NetworkTransform>().enabled = false;
        Root.GetComponent<ShipMeshCombine>().RequestCombine();
    }

    [ClientRpc]
    void RpcRepairParticleColor(GameObject RepairParticle, Color StartingColor)
    {
        if (isServer) return;
        if (RepairParticle == null) return;
        ParticleSystem.MainModule main = RepairParticle.GetComponent<ParticleSystem>().main;
        main.startColor = StartingColor;
    }

    [ClientRpc]
    void RpcRepairParticle(GameObject RepairParticle, GameObject Root, Vector3 Position)
    {
        if (isServer) return;
        if (RepairParticle == null) return;
        if (Root == null) return;
        RepairParticle.transform.parent = Root.transform;
        RepairParticle.transform.localPosition = Position;
    }
}
