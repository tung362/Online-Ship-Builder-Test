using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderColorChange : TungDoesMathForYou
{
    /*Settings*/
    public bool RunOnStart = true;

    /*Required Components*/
    private BuilderColorTracker TheBuilderColorTracker;
    private BuilderBlock TheBuilderBlock;
    private BuilderShipMeshCombine TheBuilderShipMeshCombine;

    void Start()
    {
        TheBuilderColorTracker = GameObject.FindGameObjectWithTag("BuilderColorTracker").GetComponent<BuilderColorTracker>();
        TheBuilderBlock = GetComponent<BuilderBlock>();
        TheBuilderShipMeshCombine = GetComponent<BuilderShipMeshCombine>();
        if (RunOnStart) UpdateColor();
    }

    public void UpdateColor()
    {
        //Change color right away if there is no mesh combiner, if there is a mesh combiner, change color first before combining
        if (TheBuilderShipMeshCombine == null)
        {
            //Gets all mesh renderers
            List<MeshRenderer> Meshes = new List<MeshRenderer>();
            MeshRenderer theMeshRenderer = GetComponent<MeshRenderer>();
            if (theMeshRenderer != null) Meshes.Add(theMeshRenderer);
            Meshes.AddRange(FindAllMeshes(transform));

            for (int i = 0; i < Meshes.Count; i++)
            {
                for (int j = 0; j < Meshes[i].materials.Length; j++)
                {
                    if (Meshes[i].materials[j].name.Contains("Base"))
                    {
                        Meshes[i].materials[j].color = TheBuilderColorTracker.BaseBaseColor;
                        Meshes[i].materials[j].SetColor("_EmissionColor", TheBuilderColorTracker.BaseEmissionColor);
                    }
                    else if (Meshes[i].materials[j].name.Contains("Trim"))
                    {
                        Meshes[i].materials[j].color = TheBuilderColorTracker.TrimBaseColor;
                        Meshes[i].materials[j].SetColor("_EmissionColor", TheBuilderColorTracker.TrimEmissionColor);
                    }
                    else if (Meshes[i].materials[j].name.Contains("Engine"))
                    {
                        Meshes[i].materials[j].color = TheBuilderColorTracker.EngineBaseColor;
                        Meshes[i].materials[j].SetColor("_EmissionColor", TheBuilderColorTracker.EngineEmissionColor);
                    }
                    else if (Meshes[i].materials[j].name.Contains("Shield"))
                    {
                        Meshes[i].materials[j].SetColor("_SurfaceColor", TheBuilderColorTracker.ShieldColor);
                    }
                }
            }

            if (TheBuilderBlock != null) TheBuilderBlock.UpdateStartingColor();
        }
        else
        {
            for (int i = 0; i < TheBuilderShipMeshCombine.materials.Count; i++)
            {
                Material theMaterial = new Material(TheBuilderShipMeshCombine.materials[i]);
                if (TheBuilderShipMeshCombine.materials[i].name.Contains("Base"))
                {
                    theMaterial.color = TheBuilderColorTracker.BaseBaseColor;
                    theMaterial.SetColor("_EmissionColor", TheBuilderColorTracker.BaseEmissionColor);
                }
                else if (TheBuilderShipMeshCombine.materials[i].name.Contains("Trim"))
                {
                    theMaterial.color = TheBuilderColorTracker.TrimBaseColor;
                    theMaterial.SetColor("_EmissionColor", TheBuilderColorTracker.TrimEmissionColor);
                }
                else if (TheBuilderShipMeshCombine.materials[i].name.Contains("Engine"))
                {
                    theMaterial.color = TheBuilderColorTracker.EngineBaseColor;
                    theMaterial.SetColor("_EmissionColor", TheBuilderColorTracker.EngineEmissionColor);
                }
                else if (TheBuilderShipMeshCombine.materials[i].name.Contains("Shield")) theMaterial.SetColor("_SurfaceColor", TheBuilderColorTracker.ShieldColor);

                TheBuilderShipMeshCombine.materials[i] = theMaterial;
            }
            TheBuilderShipMeshCombine.CombineMeshes();
        }
    }
}
