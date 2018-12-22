using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderSettings : MonoBehaviour
{
    /*Camera Settings*/
    public float CameraRotationSpeed = 100;
    public float CameraMoveSpeed = 5;
    public float CameraMaxYRotation = 85;
    public float CameraMinYRotation = -28;
    public float CameraMaxZoom = 2;
    public float CameraMinZoom = 6;
    public float CameraZoomSpeed = 200;

    /*Menu Settings*/
    public string ShipName = "New Ship";
    //1 = Engines, 2 = Hulls, 3 = Shields, 4 = Weapons
    public int CurrentTab = 2;
    //Selection mode
    public bool PlacingNewObject = false;
    public int CurrentSlot = 1;
    public bool Mirror = false;
    public Vector2 BuildLimit = new Vector3(3, 3);
    //Surpass the limit restriction, only used for creating non ships
    public bool OverrideBuildLimit = false;

    /*Placer settings*/
    //Only used by ghost blocks, physical blocks return to their original material
    public Material NoPlaceMaterial;
    public Material NoPlaceShieldMaterial;
    public Material PlaceMaterial;
    public Material PlaceShieldMaterial;
    //Used by physical blocks
    public Material SolidNoPlaceMaterial;
    public Material SolidNoPlaceShieldMaterial;

    /*Load Save*/
    public bool VerifyLoad = false;
    //Used by confirmation windows to load a specific file
    public string VerifyLoadName = "New Ship";
    public GameObject LoadVerifyObject;
    public GameObject Loader;

    /*Block info*/
    public float MaxThrustValue = 20;
    public float MaxWeightValue = 5;
    public float MaxArmor = 200;
    public float MaxShield = 200;
    public float MaxHullDamage = 200;
    public float MaxShieldDamage = 200;
    public float MaxFireRate = 10;
    public float maxVelocity = 100;
}
