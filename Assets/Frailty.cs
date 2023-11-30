using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Fullscreen.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.Rendering.Universal;

public class Frailty : MonoBehaviour
{
    Rigidbody rb;
    CapsuleCollider coll;
    FirstPersonController pc;
    CharacterController control;
    public float injuriousFallHeight;
    [SerializeField]
    float fallHeight;
    [SerializeField]
    float fallDifference;
    [SerializeField]
    float lastHeight;
    [SerializeField]
    float lastFall;
    [SerializeField]
    bool injuriousFall;
    [SerializeField]
    Vector3 lastVelocity;
    [SerializeField]
    Vector3 lastPosition;
    bool dying;
    public bool dead;
    Camera cam;

    void Start()
    {
        cam = Camera.main;
        pc = GetComponent<FirstPersonController>();
        Camera camera = cam.GetComponent<Camera>();
        Renderer renderer = camera.GetComponent<Renderer>();
        Debug.Log(renderer.sharedMaterials);
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
        control = GetComponent<CharacterController>();
        fallHeight = 0f;
        fallDifference = 0f;
        lastHeight = 0f;
        lastFall = 0f;
        injuriousFall = false;
        dying = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.relativeVelocity);
        Debug.Log(collision.impulse.magnitude);
        if (injuriousFall)
        {
            //rb.position = lastPosition;
            rb.angularDrag = 0.5f;
            if (!dead)
            {
                Time.timeScale = 0.1f;
                Time.fixedDeltaTime = 0.002f;
                rb.AddForceAtPosition(-collision.impulse * 5, collision.GetContact(0).point);
                rb.AddForce(new Vector3(0, collision.impulse.magnitude, 0));
                rb.AddTorque(new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)));
            }
            
            //rb.AddForce(new Vector3(Random.Range(-10f, 20f), Random.Range(0f, 150f), Random.Range(-10f, 10f)));

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dying)
        {
            if(!dead)
            {
                if (other.gameObject.tag != "Player")
                {
                    GameObject[] destroyList = GameObject.FindGameObjectsWithTag("Floor");
                    for (int i = 0; i < destroyList.Length; i++)
                    {
                        Destroy(destroyList[i]);
                    }

                    Destroy(other.gameObject);
                    GameObject[] list = GameObject.FindGameObjectsWithTag("Movable");
                    for (int i = 0; i < list.Length; i++)
                    {
                        if (list[i].GetComponent<Rigidbody>() != null)
                        {
                            list[i].GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)));
                            list[i].GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-10f, 10f), Random.Range(-20f, -150f), Random.Range(-10f, 10f)));
                        }
                    }
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = 0.02f;
                    dead = true;
                }
            }
            
        }
    }

    void Update()
    {
        if (!injuriousFall)
        {
            if (!pc.Grounded)
            {
                fallDifference = transform.position.y - lastHeight;
                if (-fallDifference > 0f)
                {
                    fallHeight -= fallDifference;
                }

                lastVelocity = control.velocity;
                lastPosition = rb.position;

                if (fallHeight > injuriousFallHeight)
                {
                    lastFall = fallHeight;
                    injuriousFall = true;
                    pc.enabled = false;
                    control.enabled = false;
                    rb.isKinematic = false;
                    //rb.AddTorque(new Vector3(Random.Range(-0.2f, -0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f)));
                    rb.velocity = lastVelocity;
                    dying = true;
                }
                lastHeight = transform.position.y;
            }
            else
            {
                if (fallHeight > 0f)
                {
                    lastFall = fallHeight;
                }
                fallHeight = 0f;
                lastHeight = transform.position.y;
            }
        }
    }
}
