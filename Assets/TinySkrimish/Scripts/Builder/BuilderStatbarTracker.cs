using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderStatbarTracker : MonoBehaviour
{
    /*Settings*/
    public List<GameObject> HullStatbars;
    public List<GameObject> WeaponStatbars;

    /*Required Component*/
    private BuilderSelector Tracker;

    void Start()
    {
        Tracker = FindObjectOfType<BuilderSelector>();
    }

    void Update()
    {
        UpdateVisiblity();
    }

    void UpdateVisiblity()
    {
        if (Tracker.GhostBlock == null) return;
        if (Tracker.GhostBlock.GetComponent<BuilderBlock>().SpawnObject.GetComponent<BlockInfo>().IsWeapon)
        {
            for(int i = 0; i < WeaponStatbars.Count; i++)
            {
                if (!WeaponStatbars[i].activeSelf) WeaponStatbars[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < WeaponStatbars.Count; i++)
            {
                if (WeaponStatbars[i].activeSelf) WeaponStatbars[i].SetActive(false);
            }
        }
    }
}
