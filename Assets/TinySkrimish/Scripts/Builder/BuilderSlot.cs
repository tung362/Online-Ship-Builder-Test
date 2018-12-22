using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderSlot : TungDoesMathForYou
{
    /*Settings*/
    public GameObject SlotSelect;
    public GameObject GhostBlock;
    public GameObject GhostBlockMirror;
    //Use this ghost block instead during mirror mode
    public GameObject MirrorAlternative;
    public int TabID = 1;
    public int SlotID = 1;

    /*Required Components*/
    private BuilderSettings TheBuilderSettings;

    void Start()
    {
        TheBuilderSettings = FindObjectOfType<BuilderSettings>();
    }

    void Update ()
    {
        UpdateSlotSelect();
        UpdateGhostBlock();
    }

    void UpdateSlotSelect()
    {
        if (TheBuilderSettings.CurrentSlot == SlotID) SlotSelect.transform.position = transform.position;
    }

    void UpdateGhostBlock()
    {
        if (TheBuilderSettings.PlacingNewObject && TheBuilderSettings.CurrentTab == TabID && TheBuilderSettings.CurrentSlot == SlotID)
        {
            GhostBlock.GetComponent<BuilderGhostBlockSelect>().UpdateGhostBlock(true);
            if(MirrorAlternative != null) MirrorAlternative.GetComponent<BuilderGhostBlockSelect>().UpdateGhostBlock(true);
            else GhostBlockMirror.GetComponent<BuilderGhostBlockSelect>().UpdateGhostBlock(true);
        }
        else
        {
            GhostBlock.GetComponent<BuilderGhostBlockSelect>().UpdateGhostBlock(false);
            if (MirrorAlternative != null) MirrorAlternative.GetComponent<BuilderGhostBlockSelect>().UpdateGhostBlock(false);
            else GhostBlockMirror.GetComponent<BuilderGhostBlockSelect>().UpdateGhostBlock(false);
        }
    }
}
