using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//Changes color of projectiles
public class ProjectileColorChange : NetworkBehaviour
{
    /*Required Components*/
    private ManagerTracker Tracker;
    MeshRenderer TheMeshRenderer;
    [SyncVar]
    public int ID = 0;

    void Start()
    {
        Tracker = GameObject.FindGameObjectWithTag("ManagerTracker").GetComponent<ManagerTracker>();
        TheMeshRenderer = GetComponent<MeshRenderer>();
        UpdateColor();
    }

    public void UpdateColor()
    {
        if (!Tracker.IsFullyReady) return;

        for(int i = 0; i < TheMeshRenderer.materials.Length; i++)
        {
            if (TheMeshRenderer.materials[i].name.Contains("Base"))
            {
                TheMeshRenderer.materials[i].color = Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].BaseBaseColor;
                TheMeshRenderer.materials[i].SetColor("_EmissionColor", Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].BaseEmissionColor);
            }
            else if (TheMeshRenderer.materials[i].name.Contains("Trim"))
            {
                TheMeshRenderer.materials[i].color = Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].TrimBaseColor;
                TheMeshRenderer.materials[i].SetColor("_EmissionColor", Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].TrimEmissionColor);
            }
            else if (TheMeshRenderer.materials[i].name.Contains("Engine"))
            {
                TheMeshRenderer.materials[i].color = Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].EngineBaseColor;
                TheMeshRenderer.materials[i].SetColor("_EmissionColor", Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].EngineEmissionColor);
            }
            else if (TheMeshRenderer.materials[i].name.Contains("Shield")) TheMeshRenderer.materials[i].SetColor("_SurfaceColor", Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].ShieldColor);
        }
    }
}
