using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagpieControl : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void flap()
    {
        GameObject mag = GameObject.FindGameObjectWithTag("Magpie");
        Rigidbody magrb = mag.GetComponent<Rigidbody>();
        magrb.AddForce(-50*mag.transform.forward);
        magrb.AddForce(50*mag.transform.up);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
