using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderStatbar : MonoBehaviour
{
    public bool Horizontal = true;
    //If stat bar should transition instantly or slowly
    public bool UseStaticBarChange = false;
    public bool ReverseStaticOnIncrease = false;
    private Vector3 StartingScale;

    private float FakeProgress = 1;
    private float FakeVelo = 0;
    public float Smoothness = 0.3f;

    //Used if ReverseStaticOnIncrease is true, tracks if stat values is increasing or decreasing
    private float CurrentChanges = 1;
    private bool RunOnce = true;

    //Max and Current
    private float Current = 1;
    private float Max = 1;

    public bool DisplayThrust = false;
    public bool DisplayWeight = false;
    public bool DisplayArmor = false;
    public bool DisplayShield = false;
    public bool DisplayHullDamage = false;
    public bool DisplayShieldDamage = false;
    public bool DisplayFireRate = false;
    public bool DisplayVelocity = false;
    private BuilderSelector Tracker;

    void Start()
    {
        Tracker = FindObjectOfType<BuilderSelector>();
        StartingScale = transform.localScale;
        if (Tracker.GhostBlock == null) return;
        else if(DisplayThrust) CurrentChanges = Tracker.GhostBlock.GetComponent<BuilderBlock>().SpawnObject.GetComponent<BlockInfo>().Thrust;
        else if(DisplayWeight) CurrentChanges = Tracker.GhostBlock.GetComponent<BuilderBlock>().SpawnObject.GetComponent<BlockInfo>().Weight;
        else if(DisplayArmor) CurrentChanges = Tracker.GhostBlock.GetComponent<BuilderBlock>().SpawnObject.GetComponent<BlockInfo>().Armor;
        else if(DisplayShield) CurrentChanges = Tracker.GhostBlock.GetComponent<BuilderBlock>().SpawnObject.GetComponent<BlockInfo>().Shield;
        else if(DisplayHullDamage) CurrentChanges = Tracker.GhostBlock.GetComponent<BuilderBlock>().SpawnObject.GetComponent<BlockInfo>().HullDamage;
        else if(DisplayShieldDamage) CurrentChanges = Tracker.GhostBlock.GetComponent<BuilderBlock>().SpawnObject.GetComponent<BlockInfo>().ShieldDamage;
        else if(DisplayFireRate) CurrentChanges = Tracker.GhostBlock.GetComponent<BuilderBlock>().SpawnObject.GetComponent<BlockInfo>().FireRate;
        else if(DisplayVelocity) CurrentChanges = Tracker.GhostBlock.GetComponent<BuilderBlock>().SpawnObject.GetComponent<BlockInfo>().Velocity;
    }

    void FixedUpdate()
    {
        if (DisplayThrust && Tracker.GhostBlock != null)
        {
            Current = Tracker.GhostBlock.GetComponent<BuilderBlock>().SpawnObject.GetComponent<BlockInfo>().Thrust;
            Max = Tracker.GetComponent<BuilderSettings>().MaxThrustValue;
        }
        else if (DisplayWeight && Tracker.GhostBlock != null)
        {
            Current = Tracker.GhostBlock.GetComponent<BuilderBlock>().SpawnObject.GetComponent<BlockInfo>().Weight;
            Max = Tracker.GetComponent<BuilderSettings>().MaxWeightValue;
        }
        else if (DisplayArmor && Tracker.GhostBlock != null)
        {
            Current = Tracker.GhostBlock.GetComponent<BuilderBlock>().SpawnObject.GetComponent<BlockInfo>().Armor;
            Max = Tracker.GetComponent<BuilderSettings>().MaxArmor;
        }
        else if (DisplayShield && Tracker.GhostBlock != null)
        {
            Current = Tracker.GhostBlock.GetComponent<BuilderBlock>().SpawnObject.GetComponent<BlockInfo>().Shield;
            Max = Tracker.GetComponent<BuilderSettings>().MaxShield;
        }
        else if (DisplayHullDamage && Tracker.GhostBlock != null)
        {
            Current = Tracker.GhostBlock.GetComponent<BuilderBlock>().SpawnObject.GetComponent<BlockInfo>().HullDamage;
            Max = Tracker.GetComponent<BuilderSettings>().MaxHullDamage;
        }
        else if (DisplayShieldDamage && Tracker.GhostBlock != null)
        {
            Current = Tracker.GhostBlock.GetComponent<BuilderBlock>().SpawnObject.GetComponent<BlockInfo>().ShieldDamage;
            Max = Tracker.GetComponent<BuilderSettings>().MaxShieldDamage;
        }
        else if (DisplayFireRate && Tracker.GhostBlock != null)
        {
            Current = Tracker.GhostBlock.GetComponent<BuilderBlock>().SpawnObject.GetComponent<BlockInfo>().FireRate;
            Max = Tracker.GetComponent<BuilderSettings>().MaxFireRate;
        }
        else if (DisplayVelocity && Tracker.GhostBlock != null)
        {
            Current = Tracker.GhostBlock.GetComponent<BuilderBlock>().SpawnObject.GetComponent<BlockInfo>().Velocity;
            Max = Tracker.GetComponent<BuilderSettings>().maxVelocity;
        }

        //Reverse static
        if (ReverseStaticOnIncrease == true)
        {
            //print(FakeVelo);
            if (CurrentChanges < Current && RunOnce == true)
            {
                if (UseStaticBarChange == true) UseStaticBarChange = false;
                else UseStaticBarChange = true;
                RunOnce = false;
            }
            else
            {
                if (RunOnce == false)
                {
                    if (UseStaticBarChange == true)
                    {
                        if (UseStaticBarChange == true) UseStaticBarChange = false;
                        else UseStaticBarChange = true;
                        RunOnce = true;
                    }
                    else if (ReverseStaticOnIncrease == true)
                    {
                        //print(FakeProgress + "/" + (Realprogress - Mathf.Epsilon));
                        if (FakeProgress >= (Current / Max) - Mathf.Epsilon)
                        {
                            if (RunOnce == false)
                            {
                                if (UseStaticBarChange == true) UseStaticBarChange = false;
                                else UseStaticBarChange = true;
                                RunOnce = true;
                            }
                        }
                    }
                }
            }
        }

        if (UseStaticBarChange == true)
        {
            //Get a percentage of bar value left
            float progress = (Current / Max);
            FakeProgress = progress;
            Vector3 modifiedScale = StartingScale;
            if (Horizontal == true) modifiedScale = new Vector3(StartingScale.x * FakeProgress, StartingScale.y, StartingScale.z);
            else modifiedScale = new Vector3(StartingScale.x, StartingScale.y * FakeProgress, StartingScale.z);
            transform.localScale = new Vector3(Mathf.Clamp(modifiedScale.x, 0.0001f, StartingScale.x), Mathf.Clamp(modifiedScale.y, 0.0001f, StartingScale.y), Mathf.Clamp(modifiedScale.z, 0.0001f, StartingScale.z));
        }
        else
        {
            float progress = (Current / Max);
            //Get a percentage of bar value left
            FakeProgress = Mathf.SmoothDamp(FakeProgress, progress, ref FakeVelo, Smoothness);
            Vector3 modifiedScale = StartingScale;
            if (Horizontal == true) modifiedScale = new Vector3(StartingScale.x * FakeProgress, StartingScale.y, StartingScale.z);
            else modifiedScale = new Vector3(StartingScale.x, StartingScale.y * FakeProgress, StartingScale.z);
            transform.localScale = new Vector3(Mathf.Clamp(modifiedScale.x, 0.0001f, StartingScale.x), Mathf.Clamp(modifiedScale.y, 0.0001f, StartingScale.y), Mathf.Clamp(modifiedScale.z, 0.0001f, StartingScale.z));
        }
        CurrentChanges = Current;
    }
}
