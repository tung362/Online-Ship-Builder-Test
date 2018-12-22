using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderSaveCam : MonoBehaviour
{
	void Update ()
    {
        GetComponent<Camera>().targetTexture = null;
        Destroy(gameObject);
	}
}
