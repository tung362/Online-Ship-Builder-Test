using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShipChangeColor : MonoBehaviour
{
    /*Data*/
    public List<MeshRenderer> Meshes = new List<MeshRenderer>();
    private int PreviousID = -2;

    /*Required components*/
    private ManagerTracker Tracker;

    void Awake()
    {
        Tracker = GameObject.FindGameObjectWithTag("ManagerTracker").GetComponent<ManagerTracker>();

        MeshRenderer theMeshRenderer = GetComponent<MeshRenderer>();
        if (theMeshRenderer != null) Meshes.Add(theMeshRenderer);
        UpdateColor(-1);
    }

    public void UpdateColor(int ID)
    {
        if (PreviousID == ID) return;
        if (!Tracker.IsFullyReady) return;
        for (int i = 0; i < Meshes.Count; i++)
        {
            for (int j = 0; j < Meshes[i].materials.Length; j++)
            {
                if (Meshes[i].materials[j].name.Contains("Base"))
                {
                    if(ID != -1)
                    {
                        Meshes[i].materials[j].color = Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].BaseBaseColor;
                        Meshes[i].materials[j].SetColor("_EmissionColor", Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].BaseEmissionColor);
                    }
                    else
                    {
                        Meshes[i].materials[j].color = Tracker.TheBuilderColorTracker.NeutralBaseBaseColor;
                        Meshes[i].materials[j].SetColor("_EmissionColor", Tracker.TheBuilderColorTracker.NeutralBaseEmissionColor);
                    }
                }
                else if (Meshes[i].materials[j].name.Contains("Trim"))
                {
                    if (ID != -1)
                    {
                        Meshes[i].materials[j].color = Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].TrimBaseColor;
                        Meshes[i].materials[j].SetColor("_EmissionColor", Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].TrimEmissionColor);
                    }
                    else
                    {
                        Meshes[i].materials[j].color = Tracker.TheBuilderColorTracker.NeutralTrimBaseColor;
                        Meshes[i].materials[j].SetColor("_EmissionColor", Tracker.TheBuilderColorTracker.NeutralTrimEmissionColor);
                    }
                }
                else if (Meshes[i].materials[j].name.Contains("Engine"))
                {
                    if (ID != -1)
                    {
                        Meshes[i].materials[j].color = Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].EngineBaseColor;
                        Meshes[i].materials[j].SetColor("_EmissionColor", Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].EngineEmissionColor);
                    }
                    else
                    {
                        Meshes[i].materials[j].color = Tracker.TheBuilderColorTracker.NeutralEngineBaseColor;
                        Meshes[i].materials[j].SetColor("_EmissionColor", Tracker.TheBuilderColorTracker.NeutralEngineEmissionColor);
                    }
                }
                else if (Meshes[i].materials[j].name.Contains("Shield"))
                {
                    if (ID != -1) Meshes[i].materials[j].SetColor("_SurfaceColor", Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].ShieldColor);
                    else Meshes[i].materials[j].SetColor("_SurfaceColor", Tracker.TheBuilderColorTracker.NeutralShieldColor);
                }
            }
        }

        PreviousID = ID;
        this.enabled = false;
    }
}
