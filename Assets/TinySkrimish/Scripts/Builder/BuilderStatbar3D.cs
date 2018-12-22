using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderStatbar3D : TungDoesMathForYou
{
    /*Required Components*/
    private BuilderSelector Tracker;
    private List<MeshRenderer> TheMeshRenderers = new List<MeshRenderer>();

    void Start()
    {
        Tracker = FindObjectOfType<BuilderSelector>();
        TheMeshRenderers = FindAllMeshes(transform);
        TheMeshRenderers.Add(GetComponent<MeshRenderer>());
    }

    void Update()
    {
        UpdateVisibility();
    }

    void UpdateVisibility()
    {
        if (Tracker.GhostBlock == null) return;
        if (Tracker.GhostBlock.name.Contains(name))
        {
            for (int i = 0; i < TheMeshRenderers.Count; i++) TheMeshRenderers[i].enabled = true;
        }
        else
        {
            for (int i = 0; i < TheMeshRenderers.Count; i++) TheMeshRenderers[i].enabled = false;
        }
    }
}
