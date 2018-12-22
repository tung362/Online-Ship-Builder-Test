using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderBlock : TungDoesMathForYou
{
    /*Settings*/
    public GameObject SpawnObject;
    public bool Ghost = false;
    public bool SnapToAllSides = false;

    /*Data*/
    private bool CanBuild = true;
    private bool CanBePlaced = true;
    //Used to reset rotation to fit new angle incase an object is tilted and needs to be aligned
    private Vector3 PreviousNormal = Vector3.zero;
    [HideInInspector]
    public bool RaycastCheckRightSide = false;
    [HideInInspector]
    public bool RaycastCheckLeftSide = false;
    //Amount of raycast checks that passed
    private int ContactCount = 0;
    private List<MeshRenderer> Meshes = new List<MeshRenderer>();
    private List<Material> OriginalMaterials = new List<Material>();
    private Vector3 PreviousPosition = Vector3.zero;
    private Quaternion PreviousRotation;

    /*Required Components*/
    private BuilderSettings TheBuilderSettings;
    private BuilderRoot TheRoot;
    private BoxCollider TheBoxCollider;
    private BuilderAdjacentCheck TheBuilderAdjacentCheck;

    void Start()
    {
        TheBuilderSettings = FindObjectOfType<BuilderSettings>();
        TheRoot = FindObjectOfType<BuilderRoot>();
        TheBoxCollider = GetComponent<BoxCollider>();
        TheBuilderAdjacentCheck = GetComponent<BuilderAdjacentCheck>();

        UpdateStartingColor();
    }

    public void UpdateStartingColor()
    {
        Meshes.Clear();
        OriginalMaterials.Clear();

        MeshRenderer theMeshRenderer = GetComponent<MeshRenderer>();
        if (theMeshRenderer != null) Meshes.Add(theMeshRenderer);
        Meshes.AddRange(FindAllMeshes(transform));

        //Get original color
        if (!Ghost)
        {
            for (int i = 0; i < Meshes.Count; i++)
            {
                for (int j = 0; j < Meshes[i].materials.Length; j++)
                {
                    Material NewColor = new Material(Meshes[i].materials[j]);
                    OriginalMaterials.Add(NewColor);
                }
            }
        }
    }

    //Snaps block in place
    public void UpdatePlacement()
    {
        if (TheBuilderSettings.GetComponent<BuilderSelector>().BuildCamera == null) return;

        Ray mouseRay = TheBuilderSettings.GetComponent<BuilderSelector>().BuildCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] mouseHits = Physics.RaycastAll(mouseRay);

        //Find closest hit point
        float ClosestDistance = int.MaxValue;
        RaycastHit hit = new RaycastHit();
        for(int i = 0; i < mouseHits.Length; i++)
        {
            if((mouseHits[i].collider.gameObject.layer == LayerMask.NameToLayer("NoPlace") || mouseHits[i].collider.gameObject.layer == LayerMask.NameToLayer("ForceBuild") || mouseHits[i].collider.gameObject.layer == LayerMask.NameToLayer("Build")) && mouseHits[i].collider.tag != "Ghost")
            {
                if (mouseHits[i].collider.GetComponent<BuilderColliderRoot>().Root != gameObject)
                {
                    float distance = Vector3.Distance(TheBuilderSettings.GetComponent<BuilderSelector>().BuildCamera.transform.position, mouseHits[i].point);
                    if (ClosestDistance > distance)
                    {
                        ClosestDistance = distance;
                        hit = mouseHits[i];
                    }
                }
            }
        }

        //Placement
        if (hit.collider != null)
        {
            //Prevent placement outside of placement zone
            if ((hit.point.x > TheBuilderSettings.BuildLimit.x || hit.point.x < -TheBuilderSettings.BuildLimit.x) || (hit.point.z > TheBuilderSettings.BuildLimit.y || hit.point.z < -TheBuilderSettings.BuildLimit.y)) return;

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Build"))
            {
                if (SnapToAllSides)
                {
                    if (PreviousNormal != hit.normal && hit.normal.y == 0)
                    {
                        transform.rotation = Quaternion.LookRotation(hit.normal);
                        PreviousNormal = hit.normal;
                    }

                    //Adjust position
                    float angle = Vector3.Angle(transform.forward, hit.normal);
                    Vector3 cross = Vector3.Cross(transform.forward, hit.normal);
                    Vector3 RightPosition = new Vector3(TheBoxCollider.size.x / 2, 0, TheBoxCollider.size.z / 2);

                    if (cross.y < 0) angle = -angle;
                    //Left
                    if (angle > 45 && angle < 135)
                    {
                        RaycastCheckRightSide = false;
                        RaycastCheckLeftSide = true;
                        transform.position = new Vector3(hit.point.x, hit.collider.transform.position.y, hit.point.z) + (transform.right * TheBoxCollider.size.x / 2);
                    }
                    //Right
                    else if (angle < -45 && angle > -135)
                    {
                        RaycastCheckRightSide = true;
                        RaycastCheckLeftSide = false;
                        transform.position = new Vector3(hit.point.x, hit.collider.transform.position.y, hit.point.z) - (transform.right * TheBoxCollider.size.x / 2);
                    }
                    //Default
                    else
                    {
                        RaycastCheckRightSide = false;
                        RaycastCheckLeftSide = false;
                        transform.position = new Vector3(hit.point.x, hit.collider.transform.position.y, hit.point.z);
                    }
                }
                else
                {
                    transform.position = new Vector3(hit.point.x, hit.collider.transform.position.y, hit.point.z);
                    transform.rotation = Quaternion.LookRotation(hit.normal);
                    RaycastCheckRightSide = false;
                    RaycastCheckLeftSide = false;
                }

                if (Ghost) GetComponent<BuilderGhostBlockSelect>().UpdateGhostBlock(true);
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("ForceBuild"))
            {
                transform.position = hit.collider.transform.position;
                transform.rotation = hit.collider.transform.rotation;
                if (Ghost) GetComponent<BuilderGhostBlockSelect>().UpdateGhostBlock(true);
                RaycastCheckRightSide = false;
                RaycastCheckLeftSide = false;
            }
            else
            {
                RaycastCheckRightSide = false;
                RaycastCheckLeftSide = false;
                if (Ghost) GetComponent<BuilderGhostBlockSelect>().UpdateGhostBlock(false);
            }
        }
        else
        {
            RaycastCheckRightSide = false;
            RaycastCheckLeftSide = false;
            if (Ghost) GetComponent<BuilderGhostBlockSelect>().UpdateGhostBlock(false);
        }

    }

    public void UpdateMirror(GameObject MirroringTarget)
    {
        RaycastCheckRightSide = !MirroringTarget.GetComponent<BuilderBlock>().RaycastCheckRightSide;
        RaycastCheckLeftSide = !MirroringTarget.GetComponent<BuilderBlock>().RaycastCheckLeftSide;

        if(RaycastCheckRightSide && RaycastCheckLeftSide)
        {
            RaycastCheckRightSide = false;
            RaycastCheckLeftSide = false;
        }

        //Reverse mirror's transform
        transform.position = new Vector3(-MirroringTarget.transform.position.x, MirroringTarget.transform.position.y, MirroringTarget.transform.position.z);
        transform.rotation = Quaternion.Inverse(MirroringTarget.transform.rotation);
    }

    //Determine if block can be placed
    public void UpdatePlacementColor()
    {
        if(TheBuilderAdjacentCheck != null)
        {
            if (!TheBuilderAdjacentCheck.Connected && !Ghost) return;
        }

        //Dimensions
        Vector3 CenterPosition = TheBoxCollider.center;
        Vector3 BackLeftPosition = new Vector3(TheBoxCollider.center.x - TheBoxCollider.size.x / 2.5f, TheBoxCollider.center.y, TheBoxCollider.center.z);
        Vector3 BackRightPosition = new Vector3(TheBoxCollider.center.x + TheBoxCollider.size.x / 2.5f, TheBoxCollider.center.y, TheBoxCollider.center.z);
        Vector3 SideLeftPosition = new Vector3(TheBoxCollider.center.x, TheBoxCollider.center.y, TheBoxCollider.center.z - TheBoxCollider.size.z / 2.5f);
        Vector3 SideRightPosition = new Vector3(TheBoxCollider.center.x, TheBoxCollider.center.y, TheBoxCollider.center.z + TheBoxCollider.size.z / 2.5f);

        ContactCount = 0;

        if (!RaycastCheckRightSide && !RaycastCheckLeftSide)
        {
            ContactCount += RaycastPlacementCheck(transform.TransformPoint(CenterPosition), -transform.forward, TheBoxCollider.size.z / 2);
            ContactCount += RaycastPlacementCheck(transform.TransformPoint(BackLeftPosition), -transform.forward, TheBoxCollider.size.z / 2);
            ContactCount += RaycastPlacementCheck(transform.TransformPoint(BackRightPosition), -transform.forward, TheBoxCollider.size.z / 2);

            if (ContactCount >= 3)
            {
                CanBePlaced = true;
                ChangeMaterialColor(true);
            }
            else
            {
                CanBePlaced = false;
                ChangeMaterialColor(false);
            }
        }
        else if (RaycastCheckRightSide && !RaycastCheckLeftSide)
        {
            ContactCount += RaycastPlacementCheck(transform.TransformPoint(CenterPosition), transform.right, TheBoxCollider.size.x / 2);
            ContactCount += RaycastPlacementCheck(transform.TransformPoint(SideLeftPosition), transform.right, TheBoxCollider.size.x / 2);
            ContactCount += RaycastPlacementCheck(transform.TransformPoint(SideRightPosition), transform.right, TheBoxCollider.size.x / 2);

            if (ContactCount >= 2)
            {
                CanBePlaced = true;
                ChangeMaterialColor(true);
            }
            else
            {
                CanBePlaced = false;
                ChangeMaterialColor(false);
            }
        }
        else if (!RaycastCheckRightSide && RaycastCheckLeftSide)
        {
            ContactCount += RaycastPlacementCheck(transform.TransformPoint(CenterPosition), -transform.right, TheBoxCollider.size.x / 2);
            ContactCount += RaycastPlacementCheck(transform.TransformPoint(SideLeftPosition), -transform.right, TheBoxCollider.size.x / 2);
            ContactCount += RaycastPlacementCheck(transform.TransformPoint(SideRightPosition), -transform.right, TheBoxCollider.size.x / 2);

            if (ContactCount >= 2)
            {
                CanBePlaced = true;
                ChangeMaterialColor(true);
            }
            else
            {
                CanBePlaced = false;
                ChangeMaterialColor(false);
            }
        }
    }

    int RaycastPlacementCheck(Vector3 Start, Vector3 Direction, float Length)
    {
        int retval = 0;
        RaycastHit[] hits = Physics.RaycastAll(Start, Direction);

        //Debug.DrawRay(Start, Direction, Color.red);

        //Find closest hit point
        float ClosestDistance = int.MaxValue;
        RaycastHit hit = new RaycastHit();
        for (int i = 0; i < hits.Length; i++)
        {
            if((hits[i].collider.gameObject.layer == LayerMask.NameToLayer("Build") || hits[i].collider.gameObject.layer == LayerMask.NameToLayer("NoPlace")) && hits[i].collider.tag != "Ghost")
            {
                if (hits[i].collider.GetComponent<BuilderColliderRoot>().Root != gameObject)
                {
                    float distance = Vector3.Distance(Start, hits[i].point);
                    if(ClosestDistance > distance && distance <= Length + 0.1f)
                    {
                        ClosestDistance = distance;
                        hit = hits[i];
                    }
                }
            }
        }

        //Determine if properly placed
        if(hit.collider != null)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Build") && CanBuild) retval = 1;
        }
        return retval;
    }

    //If its a physical block and just recently got unselected check if it's new location is suitable if no then go back to original spot
    public void UnselectCheck()
    {
        if (!CanBePlaced)
        {
            transform.position = PreviousPosition;
            transform.rotation = PreviousRotation;
        }
        else SetPreviousPositionAndRotation();
    }

    //Places the block
    public void Place()
    {
        if(CanBePlaced)
        {
            GameObject spawnedBlock = Instantiate(SpawnObject, transform.position, transform.rotation);
            spawnedBlock.name = SpawnObject.name;
            spawnedBlock.GetComponent<BuilderBlock>().SetPreviousPositionAndRotation();
            spawnedBlock.transform.parent = TheRoot.transform;
            TheRoot.AllChilds.Add(spawnedBlock);
        }
    }

    //Change block color
    public void ChangeMaterialColor(bool TrueFalse)
    {
        if(TrueFalse)
        {
            int materialID = 0;
            for (int i = 0; i < Meshes.Count; i++)
            {
                for (int j = 0; j < Meshes[i].materials.Length; j++)
                {
                    if (Meshes[i].materials[j].name.Contains("Trim") || Meshes[i].materials[j].name.Contains("Base") || Meshes[i].materials[j].name.Contains("Engine") || Meshes[i].materials[j].name.Contains("RestrictBase"))
                    {
                        if (Ghost) Meshes[i].materials[j].SetColor("_EmissionColor", TheBuilderSettings.PlaceMaterial.GetColor("_EmissionColor"));
                        else
                        {
                            Meshes[i].materials[j].color = OriginalMaterials[materialID].color;
                            Meshes[i].materials[j].SetColor("_EmissionColor", OriginalMaterials[materialID].GetColor("_EmissionColor"));
                        }
                    }
                    else if (Meshes[i].materials[j].name.Contains("Shield") || Meshes[i].materials[j].name.Contains("RestrictShield"))
                    {
                        if (Ghost) Meshes[i].materials[j].SetColor("_SurfaceColor", TheBuilderSettings.PlaceShieldMaterial.GetColor("_SurfaceColor"));
                        else Meshes[i].materials[j].SetColor("_SurfaceColor", OriginalMaterials[materialID].GetColor("_SurfaceColor"));
                    }
                    materialID += 1;
                }
            }
        }
        else
        {
            for (int i = 0; i < Meshes.Count; i++)
            {
                for (int j = 0; j < Meshes[i].materials.Length; j++)
                {
                    if (Meshes[i].materials[j].name.Contains("Trim") || Meshes[i].materials[j].name.Contains("Base") || Meshes[i].materials[j].name.Contains("Engine") || Meshes[i].materials[j].name.Contains("RestrictBase"))
                    {
                        if (Ghost) Meshes[i].materials[j].SetColor("_EmissionColor", TheBuilderSettings.NoPlaceMaterial.GetColor("_EmissionColor"));
                        else
                        {
                            Meshes[i].materials[j].color = TheBuilderSettings.SolidNoPlaceMaterial.color;
                            Meshes[i].materials[j].SetColor("_EmissionColor", TheBuilderSettings.SolidNoPlaceMaterial.GetColor("_EmissionColor"));
                        }
                    }
                    else if (Meshes[i].materials[j].name.Contains("Shield") || Meshes[i].materials[j].name.Contains("RestrictShield"))
                    {
                        if (Ghost) Meshes[i].materials[j].SetColor("_SurfaceColor", TheBuilderSettings.NoPlaceShieldMaterial.GetColor("_SurfaceColor"));
                        else Meshes[i].materials[j].SetColor("_SurfaceColor", TheBuilderSettings.SolidNoPlaceShieldMaterial.GetColor("_SurfaceColor"));
                    }
                }
            }
        }
    }

    public void SetPreviousPositionAndRotation()
    {
        PreviousPosition = transform.position;
        PreviousRotation = transform.rotation;
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.gameObject.layer == LayerMask.NameToLayer("NoBuild") && other.gameObject.tag != "Ghost")
        {
            if (other.gameObject.GetComponent<BuilderColliderRoot>().Root != gameObject) CanBuild = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.gameObject.layer == LayerMask.NameToLayer("NoBuild") && other.gameObject.tag != "Ghost")
        {
            if (other.gameObject.GetComponent<BuilderColliderRoot>().Root != gameObject) CanBuild = true;
        }
    }
}
