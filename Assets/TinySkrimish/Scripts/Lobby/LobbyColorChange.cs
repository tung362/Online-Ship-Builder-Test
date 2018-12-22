using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyColorChange : MonoBehaviour
{
    /*Settings*/
    public int ID = 0;

    /*Required components*/
    private ManagerTracker Tracker;
    private MeshRenderer TheMeshRenderer;
    private NetworkManager TheNetworkManager;

    void Start()
    {
        Tracker = GameObject.FindGameObjectWithTag("ManagerTracker").GetComponent<ManagerTracker>();
        TheMeshRenderer = GetComponent<MeshRenderer>();
        TheNetworkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
    }

    void Update()
    {
        UpdateColor();
    }

    public void UpdateColor()
    {
        if (!Tracker.IsFullyReady) return;
        if (!TheNetworkManager.IsClientConnected()) return;
        if (Tracker.HostPlayer.TheResourceManager.PlayerColor.Count - 1 < ID) return;

        for (int j = 0; j < TheMeshRenderer.materials.Length; j++)
        {
            if (TheMeshRenderer.materials[j].name.Contains("Base"))
            {
                TheMeshRenderer.materials[j].color = Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].BaseBaseColor;
                TheMeshRenderer.materials[j].SetColor("_EmissionColor", Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].BaseEmissionColor);
            }
            else if (TheMeshRenderer.materials[j].name.Contains("Trim"))
            {
                TheMeshRenderer.materials[j].color = Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].TrimBaseColor;
                TheMeshRenderer.materials[j].SetColor("_EmissionColor", Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].TrimEmissionColor);
            }
            else if (TheMeshRenderer.materials[j].name.Contains("Engine"))
            {
                TheMeshRenderer.materials[j].color = Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].EngineBaseColor;
                TheMeshRenderer.materials[j].SetColor("_EmissionColor", Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].EngineEmissionColor);
            }
            else if (TheMeshRenderer.materials[j].name.Contains("Shield"))
            {
                TheMeshRenderer.materials[j].SetColor("_SurfaceColor", Tracker.HostPlayer.TheResourceManager.PlayerColor[ID].ShieldColor);
            }
        }
    }
}
