using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShipSchematic : TungDoesNetworkingForyou
{
    /*Settings*/
    public GameObject CommandBlock;

    /*Data*/
    //Blueprint of the ship
    private ShipData Schematic;
    //Represents each schematic block's physically game object, if its missing a physical game object then it will be null
    public List<GameObject> TrackedBlocks = new List<GameObject>();
    //Tracks the current timer of when a block regens
    public List<float> TrackedBlockRegenTimers = new List<float>();
    //Prevents regenerating during combat
    public float StartRegeneratingTimer = 5;
    private bool RegenToggle = true;
    private int CommandBlockID = 0;
    private bool RunOnce = false;

    void Start()
    {
        if (!isServer) return;
        if (Schematic == null) return;
        //Match the count of the schematic
        for (int i = 0; i < Schematic.Parts.Length; i++)
        {
            TrackedBlocks.Add(null);
            TrackedBlockRegenTimers.Add(0);
        }
        FindCommandBlock();
        AdjacentCheck();
    }

    void FindCommandBlock()
    {
        if (!isServer) return;
        if (CommandBlock == null) return;
        if (Schematic == null) return;

        for (int i = 0; i < Schematic.Parts.Length; i++)
        {
            //Finds the command block
            if (Schematic.Parts[i].Name.Contains(CommandBlock.name))
            {
                //Assigns the command block as the physical game object of the schematic's command block
                TrackedBlocks[i] = CommandBlock;
                CommandBlockID = i;
            }
        }
    }

    void Update()
    {
        if (RunOnce) AdjacentCheck();
        if (StartRegeneratingTimer < 5)
        {
            StartRegeneratingTimer += Time.deltaTime;
            RegenToggle = true;
        }
        else
        {
            if (RegenToggle)
            {
                AdjacentCheck();
                RegenToggle = false;
            }
        }
    }

    //Checks for missing parts
    public void AdjacentCheck()
    {
        if (!isServer) return;
        if (CommandBlock == null) return;
        if (Schematic == null) return;
        //Reset connections
        for(int i = 0; i < TrackedBlocks.Count; i++)
        {
            if (TrackedBlocks[i] != null)
            {
                if (i != CommandBlockID)
                {
                    RpcEnableAdjacentCheck(TrackedBlocks[i]);
                    TrackedBlocks[i].GetComponent<ShipAdjacentCheck>().Connected = false;
                    TrackedBlocks[i].GetComponent<ShipAdjacentCheck>().ResetGrabbed();
                }
                else
                {
                    RpcEnableAdjacentCheck(gameObject);
                    GetComponent<ShipAdjacentCheck>().Connected = false;
                    GetComponent<ShipAdjacentCheck>().ResetGrabbed();
                }
            }
        }
        CheckBlock(CommandBlockID);

        List<GameObject> SperatingObjects = new List<GameObject>();
        //Split up disconnected blocks
        for (int i = 0; i < TrackedBlocks.Count; i++)
        {
            if(TrackedBlocks[i] != null)
            {
                if (TrackedBlocks[i].GetComponent<ShipAdjacentCheck>() != null)
                {
                    if (!TrackedBlocks[i].GetComponent<ShipAdjacentCheck>().Connected)
                    {
                        TrackedBlocks[i].transform.parent = null;
                        TrackedBlocks[i].layer = LayerMask.NameToLayer("Unattached");
                        ApplyLayerToChilds(TrackedBlocks[i].transform, "Unattached");
                        TrackedBlocks[i].GetComponent<Rigidbody>().isKinematic = false;
                        TrackedBlocks[i].GetComponent<MeshRenderer>().enabled = true;
                        TrackedBlocks[i].GetComponent<NetworkTransform>().enabled = true;
                        SperatingObjects.Add(TrackedBlocks[i]);
                        TrackedBlocks[i] = null;
                        GetComponent<ShipMeshCombine>().RequestCombine();
                    }
                }
            }
        }

        RpcSeperateFragments(SperatingObjects.ToArray());

        RunOnce = false;
    }

    //Loop for adjacent check
    public void CheckBlock(int ID)
    {
        //Clears the missing list of the block thats being checked
        if (ID != CommandBlockID)
        {
            TrackedBlocks[ID].GetComponent<ShipAdjacentCheck>().enabled = true;
            TrackedBlocks[ID].GetComponent<ShipAdjacentCheck>().MissingPartsID.Clear();
            TrackedBlocks[ID].GetComponent<ShipAdjacentCheck>().Connected = true;
        }
        else
        {
            GetComponent<ShipAdjacentCheck>().enabled = true;
            GetComponent<ShipAdjacentCheck>().MissingPartsID.Clear();
            GetComponent<ShipAdjacentCheck>().Connected = true;
        }

        //Loop through specific schematic block's connections
        for (int i = 0; i < Schematic.Parts[ID].Connections.Length; i++)
        {
            //Loop through each schematic block
            for (int j = 0; j < Schematic.Parts.Length; j++)
            {
                //If the connection matches one of the schematic blocks
                if(Schematic.Parts[ID].Connections[i] == Schematic.Parts[j])
                {
                    //If there is not a physical game object for this schematic block
                    if (TrackedBlocks[j] == null)
                    {
                        if (ID != CommandBlockID) TrackedBlocks[ID].GetComponent<ShipAdjacentCheck>().MissingPartsID.Add(j);
                        else GetComponent<ShipAdjacentCheck>().MissingPartsID.Add(j);
                    }
                    //If there is a physical game object of this schematic block then check the connected block for missing connections
                    else
                    {
                        if(j != CommandBlockID) 
                        {
                            if (!TrackedBlocks[j].GetComponent<ShipAdjacentCheck>().Connected) CheckBlock(j);
                        }
                    }
                    break;
                }
            }
        }
    }

    public void SeperateAllBlocks()
    {
        List<GameObject> SperatingObjects = new List<GameObject>();
        for (int i = 0; i < TrackedBlocks.Count; i++)
        {
            if (TrackedBlocks[i] != null && i != CommandBlockID)
            {
                TrackedBlocks[i].transform.parent = null;
                TrackedBlocks[i].layer = LayerMask.NameToLayer("Unattached");
                ApplyLayerToChilds(TrackedBlocks[i].transform, "Unattached");
                TrackedBlocks[i].GetComponent<Rigidbody>().isKinematic = false;
                TrackedBlocks[i].GetComponent<MeshRenderer>().enabled = true;
                TrackedBlocks[i].GetComponent<NetworkTransform>().enabled = true;
                TrackedBlocks[i].GetComponent<ShipAdjacentCheck>().MissingPartsID.Clear();
                TrackedBlocks[i].GetComponent<ShipAdjacentCheck>().GrabTargetPartID = -1;
                TrackedBlocks[i].GetComponent<ShipAdjacentCheck>().BeingGrabbed = false;
                SperatingObjects.Add(TrackedBlocks[i]);
                TrackedBlocks[i] = null;
            }
        }
        RpcSeperateAllBlocks(SperatingObjects.ToArray());
    }

    public void SetSchematic(ShipData NewSchematic)
    {
        Schematic = NewSchematic;
    }

    public ShipData GetSchematic()
    {
        return Schematic;
    }

    public void RequestAdjacentCheck()
    {
        RunOnce = true;
    }

    [ClientRpc]
    void RpcSplitFragment(GameObject ObjectToSplit, GameObject Root, Vector3 Position, Vector3 RotationEuler)
    {
        if (isServer) return;
        if (ObjectToSplit == null) return;
        if (Root == null) return;

        ObjectToSplit.transform.parent = null;
        ObjectToSplit.layer = LayerMask.NameToLayer("Unattached");
        ApplyLayerToChilds(ObjectToSplit.transform, "Unattached");
        ObjectToSplit.GetComponent<Rigidbody>().isKinematic = false;
        ObjectToSplit.GetComponent<MeshRenderer>().enabled = true;
        ObjectToSplit.GetComponent<NetworkTransform>().enabled = true;
        GetComponent<ShipMeshCombine>().RequestCombine();
    }

    [ClientRpc]
    void RpcEnableAdjacentCheck(GameObject ScriptObject)
    {
        if (isServer) return;
        if (ScriptObject != null) ScriptObject.GetComponent<ShipAdjacentCheck>().enabled = true;
    }

    [ClientRpc]
    void RpcSeperateAllBlocks(GameObject[] ObjectsToSeperate)
    {
        if (isServer) return;
        for (int i = 0; i < ObjectsToSeperate.Length; i++)
        {
            ObjectsToSeperate[i].transform.parent = null;
            ObjectsToSeperate[i].layer = LayerMask.NameToLayer("Unattached");
            ApplyLayerToChilds(ObjectsToSeperate[i].transform, "Unattached");
            ObjectsToSeperate[i].GetComponent<Rigidbody>().isKinematic = false;
            ObjectsToSeperate[i].GetComponent<MeshRenderer>().enabled = true;
            ObjectsToSeperate[i].GetComponent<NetworkTransform>().enabled = true;
        }
    }

    [ClientRpc]
    void RpcSeperateFragments(GameObject[] ObjectsToSeperate)
    {
        if (isServer) return;
        for (int i = 0; i < ObjectsToSeperate.Length; i++)
        {
            ObjectsToSeperate[i].transform.parent = null;
            ObjectsToSeperate[i].layer = LayerMask.NameToLayer("Unattached");
            ApplyLayerToChilds(ObjectsToSeperate[i].transform, "Unattached");
            ObjectsToSeperate[i].GetComponent<Rigidbody>().isKinematic = false;
            ObjectsToSeperate[i].GetComponent<MeshRenderer>().enabled = true;
            ObjectsToSeperate[i].GetComponent<NetworkTransform>().enabled = true;
        }
        if (GetComponent<ShipMeshCombine>() != null) GetComponent<ShipMeshCombine>().RequestCombine();
    }
}