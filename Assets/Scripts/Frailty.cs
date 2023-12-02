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
    public Vector3 messageScaleDeadFallA;
    public Vector3 messageScaleDeadFallB;
    public Vector3 messageScalePressAnyKey;
    public string messageDeadKillA;
    public string messageDeadKillB;
    public string messageDeadKillC;
    public Vector3 messageScaleDeadKillA;
    public Vector3 messageScaleDeadKillC;
    public string messagePressAnyKey;
    public GameObject messageGO;
    public GameObject messageBox;
    public GameObject narrationCamera;
    public GameObject narrationText;

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
    bool dying;
    public bool dead;
    Camera cam;
    [SerializeField]
    string deathType;

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
        killingObject = null;
        messageGO.SetActive(false);
        deathType = "None";
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > killingImpulse)
        {
            Debug.Log(collision.relativeVelocity);
            Debug.Log(collision.impulse.magnitude);
            Debug.Log(collision.gameObject.name);
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
                    deathType = "fall";
                    pc.enabled = false;
                    control.enabled = false;
                    rb.isKinematic = false;
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

    IEnumerator Death(string type)
    {
        string firstStr = "";
        string secondStr = "";
        Vector3 firstScale = Vector3.zero;
        Vector3 secondScale = Vector3.zero;
        Vector3 scaleLerp = Vector3.zero;
        Vector3 rotLerp = Vector3.zero;
        float ninetyInRads = 90 * Mathf.Deg2Rad; 
        float oneEightyInRads = 180 * Mathf.Deg2Rad;
        float twoSeventyInRads = 270 * Mathf.Deg2Rad;
        float threeSixtyInRads = 360 * Mathf.Deg2Rad;

        if (type == "fall")
        {
            firstStr = messageDeadFallA;
            secondStr = messageDeadFallB;
            firstScale = messageScaleDeadFallA;
            secondScale = messageScaleDeadFallB;
        }
        else if (type == "kill")
        {
            firstStr = new string($"{messageDeadKillA} {killingObject.name} {messageDeadKillB}");
            secondStr = messageDeadKillC;
            firstScale = messageScaleDeadKillA;
            secondScale = messageScaleDeadKillC;
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
            Debug.Log(Vector3.Distance(messageGO.transform.position, cam.transform.position + (cam.transform.forward * messagePositionClose.z)));
            yield return null;
        }

        yield return new WaitForSeconds(3);

        lerp = 0f;

        while (lerp < 1f)
        {
            lerp += 0.5f * Time.fixedDeltaTime;
            if (lerp > 1f)
            {
                lerp = 1f;
            }
            rotLerp = Vector3.Lerp(new Vector3(0,0,0), new Vector3(ninetyInRads, 0,0), lerp);
            scaleLerp = Vector3.Lerp(firstScale, secondScale, lerp);
            Debug.Log(rotLerp);
            messageBox.transform.localScale.Set(scaleLerp.x, scaleLerp.y, scaleLerp.z);
            messageBox.transform.localEulerAngles.Set(rotLerp.x, rotLerp.y, rotLerp.z);
            yield return null;
        }

        narrationText.GetComponent<TextMeshPro>().text = secondStr;
        Debug.Log(secondStr);
        narrationCamera.GetComponent<Camera>().Render();
        yield return null;

        lerp = 0f;

        while (lerp < 1f)
        {
            lerp += 0.5f * Time.fixedDeltaTime;
            if (lerp > 1f)
            {
                lerp = 1f;
            }
            rotLerp = Vector3.Lerp(new Vector3(ninetyInRads, 0, 0), new Vector3(oneEightyInRads, 0, 0), lerp);
            Debug.Log(rotLerp);
            messageBox.transform.localEulerAngles.Set(rotLerp.x, rotLerp.y, rotLerp.z);
            yield return null;
        }

        yield return new WaitForSeconds(3);

        lerp = 0f;

        while (lerp < 1f)
        {
            lerp += 0.5f * Time.fixedDeltaTime;
            if (lerp > 1f)
            {
                lerp = 1f;
            }
            rotLerp = Vector3.Lerp(new Vector3(oneEightyInRads, 0, 0), new Vector3(twoSeventyInRads, 0, 0), lerp);
            Debug.Log(rotLerp);
            scaleLerp = Vector3.Lerp(secondScale, messageScalePressAnyKey, lerp);
            messageBox.transform.localScale.Set(scaleLerp.x, scaleLerp.y, scaleLerp.z);
            messageBox.transform.localEulerAngles.Set(rotLerp.x, rotLerp.y, rotLerp.z);
            yield return null;
        }

        narrationText.GetComponent<TextMeshPro>().text = messagePressAnyKey;
        Debug.Log(messagePressAnyKey);
        narrationCamera.GetComponent<Camera>().Render();
        yield return null;

        lerp = 0f;

        while (lerp < 1f)
        {
            lerp += 0.5f * Time.fixedDeltaTime;
            if (lerp > 1f)
            {
                lerp = 1f;
            }
            rotLerp = Vector3.Lerp(new Vector3(oneEightyInRads, 0, 0), new Vector3(twoSeventyInRads, 0, 0), lerp);
            Debug.Log(rotLerp);
            messageBox.transform.localEulerAngles.Set(rotLerp.x, rotLerp.y, rotLerp.z);
            yield return null;
        }
    }
}
