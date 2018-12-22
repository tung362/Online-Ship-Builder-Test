using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public BlockStats TheBlockStats;
    public float TransitionSpeed = 0.3f;
    public Vector3 StarttingSize = new Vector3(31, 31, 31);

	void Start ()
    {
        transform.localScale = Vector3.zero;
    }
	
	void Update ()
    {
        if(transform.root.GetComponent<ShipSchematic>() != null)
        {
            if (!TheBlockStats.ShieldIsDown)
            {
                if (transform.localScale != StarttingSize) transform.localScale = Vector3.MoveTowards(transform.localScale, StarttingSize, TransitionSpeed * Time.deltaTime);
            }
            else
            {
                if (transform.localScale != Vector3.zero) transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, TransitionSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (transform.localScale != Vector3.zero) transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, TransitionSpeed * Time.deltaTime);
        }
	}
}
