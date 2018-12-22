using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderRoot : MonoBehaviour
{
    public List<GameObject> AllChilds = new List<GameObject>();
    public GameObject CommandBlock;
    //Calculated Stats
    public float Cost = 0;
    public float Speed = 0;

    public void AdjacentCheck()
    {
        ClearOutNulls();

        //Reset
        Cost = 0;
        Speed = 0;
        for (int i = 0; i < AllChilds.Count; i++) AllChilds[i].GetComponent<BuilderAdjacentCheck>().Connected = false;
        //Start adjacent check chain
        CommandBlock.GetComponent<BuilderAdjacentCheck>().AdjacentCheck();

        //Change Color and assign speed and cost
        for (int i = 0; i < AllChilds.Count; i++)
        {
            if (AllChilds[i].GetComponent<BuilderAdjacentCheck>().Connected)
            {
                AllChilds[i].GetComponent<BuilderBlock>().ChangeMaterialColor(true);
                Cost += AllChilds[i].GetComponent<BlockInfo>().Cost;
                Speed += AllChilds[i].GetComponent<BlockInfo>().Thrust;
                Speed -= AllChilds[i].GetComponent<BlockInfo>().Weight;
            }
            else AllChilds[i].GetComponent<BuilderBlock>().ChangeMaterialColor(false);
        }
    }

    //Deletes any object that isnt connected
    public void ClearAllRedBlocks()
    {
        for(int i = 0; i < AllChilds.Count; i++)
        {
            if (!AllChilds[i].GetComponent<BuilderAdjacentCheck>().Connected) Destroy(AllChilds[i]);
        }
        ClearOutNulls();
    }

    //Deletes all blocks, used when loading a save
    public void ClearAllBlocks()
    {
        for (int i = 0; i < AllChilds.Count; i++)
        {
            if (i != 0) Destroy(AllChilds[i]);
        }
        ClearOutNulls();
    }

    //Removes nulls from list
    public void ClearOutNulls()
    {
        for (int i = 0; i < AllChilds.Count; i++)
        {
            if (AllChilds[i] == null)
            {
                AllChilds.RemoveAt(i);
                i -= 1;
            }
        }
    }
}
