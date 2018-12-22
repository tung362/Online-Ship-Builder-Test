using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderSaveShipRoot : MonoBehaviour
{
    public List<GameObject> AllChilds = new List<GameObject>();

    //Deletes all blocks, used when loading a save
    public void ClearAllBlocks()
    {
        for (int i = 0; i < AllChilds.Count; i++)
        {
            Destroy(AllChilds[i]);
            AllChilds[i] = null;
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

    void Update()
    {
        Destroy(gameObject);
    }
}
