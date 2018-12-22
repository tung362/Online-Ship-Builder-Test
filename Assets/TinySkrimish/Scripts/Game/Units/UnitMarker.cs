using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMarker : MonoBehaviour
{
    public GameObject Sprite;

    void Update()
    {
        UpdateVisibility();
    }

    void UpdateVisibility()
    {
        if(transform.root.GetComponent<UnitStats>().IsSelected) Sprite.SetActive(true);
        else Sprite.SetActive(false);
    }
}
