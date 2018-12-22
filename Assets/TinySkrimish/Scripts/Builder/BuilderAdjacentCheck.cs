using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderAdjacentCheck : MonoBehaviour
{
    public List<GameObject> AdjacentBlocks = new List<GameObject>();

    //If the command block reached this block through other adjacent blocks
    [HideInInspector]
    public bool Connected = false;

    private BoxCollider TheBoxCollider;

    void Start()
    {
        TheBoxCollider = GetComponent<BoxCollider>();
    }

    //Called on by the command block when theres a check for gaps
    public void AdjacentCheck()
    {
        ClearOutNulls();
        Connected = true;

        for (int i = 0; i < AdjacentBlocks.Count; i++)
        {
            if (AdjacentBlocks[i] != null)
            {
                if (!AdjacentBlocks[i].GetComponent<BuilderAdjacentCheck>().Connected) AdjacentBlocks[i].GetComponent<BuilderAdjacentCheck>().AdjacentCheck();
            }
        }
    }

    //Removes nulls from list
    public void ClearOutNulls()
    {
        for(int i = 0; i < AdjacentBlocks.Count; i++)
        {
            if (AdjacentBlocks[i] == null) AdjacentBlocks.RemoveAt(i);
        }
    }

    //Add to list of adjacent blocks
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.gameObject.layer == LayerMask.NameToLayer("Adjacent"))
        {
            if (other.gameObject.GetComponent<BuilderColliderRoot>().Root != gameObject) AdjacentBlocks.Add(other.gameObject.GetComponent<BuilderColliderRoot>().Root);
        }
    }
    
    //Remove from list of adjacent blocks
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.gameObject.layer == LayerMask.NameToLayer("Adjacent"))
        {
            if (other.gameObject.GetComponent<BuilderColliderRoot>().Root != gameObject)
            {
                for(int i = 0; i < AdjacentBlocks.Count; i++)
                {
                    if (AdjacentBlocks[i].gameObject == other.gameObject.GetComponent<BuilderColliderRoot>().Root) AdjacentBlocks.RemoveAt(i);
                }
            }
        }
    }
}
