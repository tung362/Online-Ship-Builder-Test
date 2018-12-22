using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderGhostBlockSelect : TungDoesMathForYou
{
    /*Settings*/
    public bool Mirror = false;
    
    /*Required Components*/
    private BuilderSettings TheBuilderSettings;
    private MeshRenderer TheMeshRenderer;

    void Start()
    {
        TheBuilderSettings = FindObjectOfType<BuilderSettings>();
        TheMeshRenderer = GetComponent<MeshRenderer>();

        UpdateGhostBlock(false);
    }

    public void UpdateGhostBlock(bool TrueFalse)
    {
        if (TrueFalse)
        {
            TheMeshRenderer.enabled = true;
            List<MeshRenderer> childs = FindAllMeshes(transform);
            for (int i = 0; i < childs.Count; i++) childs[i].enabled = true;
            //Assigns the ghost block to the selector
            if(Mirror) TheBuilderSettings.GetComponent<BuilderSelector>().GhostBlockMirror = gameObject;
            else TheBuilderSettings.GetComponent<BuilderSelector>().GhostBlock = gameObject;
        }
        else
        {
            TheMeshRenderer.enabled = false;
            List<MeshRenderer> childs = FindAllMeshes(transform);
            for (int i = 0; i < childs.Count; i++) childs[i].enabled = false;
            transform.position = new Vector3(1000, 1000, 1000);
        }
    }
}
