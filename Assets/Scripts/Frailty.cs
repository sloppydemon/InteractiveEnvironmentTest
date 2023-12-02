using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public float killingImpulse;
    public Vector3 messagePositionClose;
    public Vector3 messagePositionFar;
    public string messageDeadFallA;
    public string messageDeadFallB;
    public Vector3 messageScaleDeadFall;
    public Vector3 messageScalePressAnyKey;
    public string messageDeadKillA;
    public string messageDeadKillB;
    public string messageDeadKillC;
    public Vector3 messageScaleDeadKill;
    public string messagePressAnyKey;
    public GameObject messageGO;
    public GameObject messageBox;
    public GameObject narrationCamera;
    public GameObject narrationText;
    public GameObject magpie;
    Camera cam;

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
    [SerializeField]
    GameObject killingObject;
    [SerializeField]
    bool dying;
    [SerializeField]
    public bool dead;
    [SerializeField]
    string deathType;
    [SerializeField]
    bool paperPlane;
    [SerializeField]
    bool paperGliding;
    [SerializeField]
    GameObject paperPlaneGO;
    [SerializeField]
    bool magpieRiding;

    void Start()
    {
        cam = Camera.main;
        pc = GetComponent<FirstPersonController>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
        control = GetComponent<CharacterController>();
        fallHeight = 0f;
        fallDifference = 0f;
        lastHeight = 0f;
        lastFall = 0f;
        injuriousFall = false;
        dying = false;
        dead = false;
        killingObject = null;
        messageGO.SetActive(false);
        deathType = "None";
        paperGliding = false;
        paperPlane = false;
        paperPlaneGO = null;
        magpieRiding = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > killingImpulse)
        {
            if (!dead && !dying)
            {
                pc.enabled = false;
                control.enabled = false;
                rb.isKinematic = false;
                dying = true;
                rb.AddForceAtPosition(-collision.impulse * 200, collision.GetContact(0).point);
                rb.AddForce(new Vector3(0, collision.impulse.magnitude * 20f, 0));
                rb.angularDrag = 0.5f;
                Time.timeScale = 0.1f;
                Time.fixedDeltaTime = 0.002f;
                killingObject = collision.gameObject;
                deathType = "kill";
            }
        }

        if (paperGliding)
        {
            if (collision.gameObject.tag == "Floor")
            {
                pc.enabled = true;
                control.enabled = true;
                rb.isKinematic = true;
                paperGliding = false;

            }
        }

        if (injuriousFall)
        {
            rb.angularDrag = 0.5f;
            if (!dead)
            {
                Time.timeScale = 0.1f;
                Time.fixedDeltaTime = 0.002f;
                rb.AddForceAtPosition(-collision.impulse * 5, collision.GetContact(0).point);
                rb.AddForce(new Vector3(0, collision.impulse.magnitude, 0));
                rb.AddTorque(new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dying)
        {
            if(!dead)
            {
                if (other.gameObject.tag != "Player" && other.gameObject != killingObject)
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
                    StartCoroutine(Death(deathType));


                }
            }
        }
    }

    void Update()
    {
        if (!paperGliding && !magpieRiding)
        {
            if (!injuriousFall)
            {
                if (!pc.Grounded && !paperPlane)
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
                        deathType = "fall";
                        pc.enabled = false;
                        control.enabled = false;
                        rb.isKinematic = false;
                        rb.velocity = lastVelocity;
                        dying = true;
                    }
                    lastHeight = transform.position.y;
                }
                else if (!pc.Grounded && paperPlane)
                {
                    lastVelocity = control.velocity;
                    paperGliding = true;
                    pc.enabled = false;
                    control.enabled = false;
                    rb.isKinematic = false;
                    rb.velocity = lastVelocity;
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

    IEnumerator Death(string type)
    {
        string firstStr = "";
        string secondStr = "";
        Vector3 firstScale = Vector3.zero;

        if (type == "fall")
        {
            firstStr = messageDeadFallA;
            secondStr = messageDeadFallB;
            firstScale = messageScaleDeadFall;
        }
        else if (type == "kill")
        {
            firstStr = new string($"{messageDeadKillA} {killingObject.name} {messageDeadKillB}");
            secondStr = messageDeadKillC;
            firstScale = messageScaleDeadKill;
        }

        narrationText.GetComponent<TextMeshPro> ().text = firstStr;
        narrationCamera.GetComponent<Camera>().Render();
        messageGO.transform.position = cam.transform.position + messagePositionFar;
        messageBox.transform.localScale = firstScale;
        messageGO.SetActive(true);
        yield return null;

        float lerp = 0f;

        while (Vector3.Distance(messageGO.transform.position, cam.transform.position + (cam.transform.forward * messagePositionClose.z)) > 0f)
        {
            lerp += 0.5f * Time.fixedDeltaTime;
            if (lerp > 1f)
            {
                lerp = 1f;
            }
            messageGO.transform.position = Vector3.Lerp(cam.transform.position + (cam.transform.forward * messagePositionFar.z), cam.transform.position + (cam.transform.forward * messagePositionClose.z), lerp);
            yield return null;
        }

        yield return new WaitForSeconds(5);

        lerp = 0f;

        while (lerp < 1f)
        {
            lerp += 5f * Time.fixedDeltaTime;
            if (lerp > 1f)
            {
                lerp = 1f;
            }
            messageGO.transform.position = Vector3.Lerp(cam.transform.position + (cam.transform.forward * messagePositionClose.z), cam.transform.position + (cam.transform.forward * messagePositionFar.z), lerp);
            yield return null;
        }

        narrationText.GetComponent<TextMeshPro>().text = secondStr;
        narrationCamera.GetComponent<Camera>().Render();
        yield return null;

        lerp = 0f;

        while (lerp < 1f)
        {
            lerp += 5f * Time.fixedDeltaTime;
            if (lerp > 1f)
            {
                lerp = 1f;
            }
            messageGO.transform.position = Vector3.Lerp(cam.transform.position + (cam.transform.forward * messagePositionFar.z), cam.transform.position + (cam.transform.forward * messagePositionClose.z), lerp);
            yield return null;
        }

        yield return new WaitForSeconds(5);

        lerp = 0f;

        while (lerp < 1f)
        {
            lerp += 5f * Time.fixedDeltaTime;
            if (lerp > 1f)
            {
                lerp = 1f;
            }
            messageGO.transform.position = Vector3.Lerp(cam.transform.position + (cam.transform.forward * messagePositionClose.z), cam.transform.position + (cam.transform.forward * messagePositionFar.z), lerp);
            yield return null;
        }

        narrationText.GetComponent<TextMeshPro>().text = messagePressAnyKey;
        narrationCamera.GetComponent<Camera>().Render();
        yield return null;

        lerp = 0f;

        while (lerp < 1f)
        {
            lerp += 5f * Time.fixedDeltaTime;
            if (lerp > 1f)
            {
                lerp = 1f;
            }
            messageGO.transform.position = Vector3.Lerp(cam.transform.position + (cam.transform.forward * messagePositionFar.z), cam.transform.position + (cam.transform.forward * messagePositionClose.z), lerp);
            yield return null;
        }
    }
}
