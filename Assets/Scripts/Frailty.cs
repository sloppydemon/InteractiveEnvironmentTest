using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Rendering.Fullscreen.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Frailty : MonoBehaviour
{
    Rigidbody body;
    CapsuleCollider coll;
    FirstPersonController pc;
    CharacterController control;
    public float injuriousFallHeight;
    public float killingImpulse;
    public float glideBreakingImpulse;
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
    public string messageDeadGlideA;
    public string messageDeadGlideB;
    public string messageDeadGlideC;
    public Vector3 messageScaleDeadGlide;
    public string messageDeadCrashA;
    public string messageDeadCrashB;
    public Vector3 messageScaleDeadCrash;
    public string messagePressAnyKey;
    public GameObject messageGO;
    public GameObject messageBox;
    public GameObject narrationCamera;
    public GameObject narrationText;
    public GameObject magpie;
    public AudioSource snd;
    public AudioClip[] impactSnds;
    public AudioClip flyingSnd;
    public AudioClip dyingSnd;
    public AudioClip deathSnd;
    public GameObject sun;
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
    public bool paperPlane;
    public bool paperGliding;
    public GameObject paperPlaneGO;
    [SerializeField]
    bool magpieRiding;
    bool deathDone;

    float UdotD;
    float UdotF;
    float FdotV;
    float DdotV;
    Vector3 lift;
    Vector3 glide;
    float pitch;
    float yaw;
    float roll;

    void Start()
    {
        cam = Camera.main;
        pc = GetComponent<FirstPersonController>();
        body = GetComponent<Rigidbody>();
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
        deathDone = false;
        UdotD = 0f;
        UdotF = 0f;
        FdotV = 0f;
        lift = Vector3.zero;
        glide = Vector3.zero;
        pitch = 0f;
        yaw = 0f;
        roll = 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.impulse.magnitude);
        if (collision.impulse.magnitude > killingImpulse)
        {
            if (!dead && !dying && collision.gameObject.tag != "Player" && collision.gameObject != paperPlaneGO)
            {
                snd.clip = dyingSnd;
                snd.Play();
                snd.pitch = 1;
                snd.volume = 0.75f;

                pc.enabled = false;
                control.enabled = false;
                body.isKinematic = false;
                dying = true;
                if (paperGliding || paperPlane)
                {
                    paperGliding = false;
                    paperPlane = false;
                    paperPlaneGO.GetComponent<PaperPlaneControl>().Detach();
                    deathType = "glide";
                }
                else
                {
                    deathType = "kill";
                }
                body.AddForceAtPosition(-collision.impulse * 200, collision.GetContact(0).point);
                body.AddForce(new Vector3(0, collision.impulse.magnitude * 20f, 0));

                int clipNo = Random.Range(0, impactSnds.Length - 1);
                snd.pitch = 0.5f + Random.Range(-0.1f, 0.1f) - (collision.impulse.magnitude * 0.1f);
                snd.PlayOneShot(impactSnds[clipNo], 0.5f + (collision.impulse.magnitude * 0.1f));

                body.angularDrag = 0.5f;
                Time.timeScale = 0.1f;
                Time.fixedDeltaTime = 0.002f;
                killingObject = collision.gameObject;
                sun.GetComponent<SunControl>().DyingStart();
            }
        }

        if (paperGliding)
        {
            if (Vector3.Dot(collision.GetContact(0).normal, Vector3.up) > 0.75f)
            {
                if (Vector3.Dot(transform.up, Vector3.up) > 0.8f)
                {
                    snd.clip = null;
                    snd.pitch = 1;
                    snd.volume = 1;

                    lastHeight = transform.position.y;
                    paperGliding = false;
                    paperPlane = false;
                    paperPlaneGO.GetComponent<PaperPlaneControl>().Detach();
                    control.enabled = true;
                    pc.enabled = true;
                    body.isKinematic = true;
                    transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                }
                else
                {
                    snd.clip = dyingSnd;
                    snd.Play();
                    snd.pitch = 1;
                    snd.volume = 0.75f;

                    paperGliding = false;
                    paperPlane = false;
                    paperPlaneGO.GetComponent<PaperPlaneControl>().Detach();
                    dying = true;
                    deathType = "crash";
                    body.AddForceAtPosition(-collision.impulse * 200, collision.GetContact(0).point);
                    body.AddForce(new Vector3(0, collision.impulse.magnitude * 20f, 0));

                    int clipNo = Random.Range(0, impactSnds.Length - 1);
                    snd.pitch = 0.5f + Random.Range(-0.1f, 0.1f) - (collision.impulse.magnitude * 0.1f);
                    snd.PlayOneShot(impactSnds[clipNo], 0.5f + (collision.impulse.magnitude * 0.1f));

                    body.angularDrag = 0.5f;
                    Time.timeScale = 0.1f;
                    Time.fixedDeltaTime = 0.002f;
                    sun.GetComponent<SunControl>().DyingStart();
                }
            }
            else
            {
                if (collision.impulse.magnitude > glideBreakingImpulse)
                {
                    snd.clip = dyingSnd;
                    snd.Play();
                    snd.pitch = 1;
                    snd.volume = 0.75f;

                    paperGliding = false;
                    paperPlane = false;
                    paperPlaneGO.GetComponent<PaperPlaneControl>().Detach();
                    dying = true;
                    deathType = "glide";
                    body.AddForceAtPosition(-collision.impulse * 200, collision.GetContact(0).point);
                    body.AddForce(new Vector3(0, collision.impulse.magnitude * 20f, 0));

                    int clipNo = Random.Range(0, impactSnds.Length - 1);
                    snd.pitch = 0.5f + Random.Range(-0.1f, 0.1f) - (collision.impulse.magnitude * 0.1f);
                    snd.PlayOneShot(impactSnds[clipNo], 0.5f + (collision.impulse.magnitude * 0.1f));

                    body.angularDrag = 0.5f;
                    Time.timeScale = 0.1f;
                    Time.fixedDeltaTime = 0.002f;
                    killingObject = collision.gameObject;
                    sun.GetComponent<SunControl>().DyingStart();
                }
            }
        }

        if (injuriousFall)
        {
            body.angularDrag = 0.5f;
            if (!dead)
            {
                snd.clip = dyingSnd;
                snd.Play();
                snd.pitch = 1;
                snd.volume = 0.75f;

                Time.timeScale = 0.1f;
                Time.fixedDeltaTime = 0.002f;
                body.AddForceAtPosition(-collision.impulse * 5, collision.GetContact(0).point);
                body.AddForce(new Vector3(0, collision.impulse.magnitude, 0));
                body.AddTorque(new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)));

                int clipNo = Random.Range(0, impactSnds.Length - 1);
                snd.pitch = 0.5f + Random.Range(-0.1f, 0.1f) - (collision.impulse.magnitude * 0.1f);
                snd.PlayOneShot(impactSnds[clipNo], 0.5f + (collision.impulse.magnitude * 0.1f));

                sun.GetComponent<SunControl>().DyingStart();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dying)
        {
            if(!dead)
            {
                if (other.gameObject.tag != "Player" && other.gameObject != killingObject && other.gameObject != paperPlaneGO)
                {
                    snd.clip = null;
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
                    
                    snd.clip = flyingSnd;
                    snd.Play();
                    sun.GetComponent<SunControl>().DeadStart();
                    StartCoroutine(Death(deathType));


                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) 
        {
            gameObject.GetComponent<BasicRigidBodyPush>().canPush = true;
        }

        if (Input.GetKeyUp(KeyCode.V))
        {
            gameObject.GetComponent<BasicRigidBodyPush>().canPush = false;
        }

        if (!paperGliding && !magpieRiding)
        {
            if (!injuriousFall)
            {
                if (!pc.Grounded && !paperPlane && !dying)
                {
                    fallDifference = transform.position.y - lastHeight;
                    if (-fallDifference > 0f)
                    {
                        fallHeight -= fallDifference;
                    }

                    lastVelocity = control.velocity;
                    lastPosition = body.position;

                    if (fallHeight > injuriousFallHeight)
                    {
                        lastFall = fallHeight;
                        injuriousFall = true;
                        deathType = "fall";
                        pc.enabled = false;
                        control.enabled = false;
                        body.isKinematic = false;
                        body.velocity = lastVelocity;
                        dying = true;
                        snd.clip = flyingSnd;
                        snd.Play();
                    }
                    lastHeight = transform.position.y;
                }
                else if (!pc.Grounded && paperPlane)
                {
                    lastVelocity = Vector3.Max(control.velocity, body.velocity);
                    paperGliding = true;
                    pc.enabled = false;
                    control.enabled = false;
                    body.isKinematic = false;
                    body.velocity = lastVelocity;
                    snd.clip = flyingSnd;
                    snd.Play();
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

        if (paperGliding)
        {
            UdotD = Vector3.Dot(transform.up, Vector3.up);
            FdotV = Vector3.Dot(body.velocity.normalized, transform.forward);
            UdotF = Vector3.Dot(transform.forward, Vector3.up);
            DdotV = Vector3.Dot(body.velocity.normalized, transform.up);

            lift = -Physics.gravity * Mathf.Clamp(FdotV, 0f, 1f) * body.velocity.magnitude * 6f * /*Mathf.Abs(UdotD) **/ Time.deltaTime;
            glide = transform.forward * -UdotF * 20f * Time.deltaTime;
            pitch = Input.GetAxis("Mouse Y") * 100f * Time.deltaTime;
            roll = -Input.GetAxis("Mouse X") * 100f * Time.deltaTime;
            yaw = Input.GetAxis("Mouse X") * 100f * Time.deltaTime;

            body.velocity = Vector3.RotateTowards(body.velocity, transform.forward * body.velocity.magnitude, 0.1f, Time.deltaTime);
            body.transform.Rotate(pitch, yaw, roll);
            body.AddForce(lift + glide);
            snd.pitch = 1f * (body.velocity.magnitude * 0.5f);
            snd.volume = 1f * (body.velocity.magnitude * 0.2f);
        }

        if (dead)
        {
            if (deathDone)
            {
                if (Input.anyKeyDown)
                {

                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
                }
            }

            snd.pitch = 0.2f + (body.velocity.magnitude * 0.001f);
            snd.volume = 0.01f + (body.velocity.magnitude * 0.005f);
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
            firstStr = new string($"{messageDeadKillA}{killingObject.GetComponent<Keywords>().killerName}{messageDeadKillB}");
            secondStr = messageDeadKillC;
            firstScale = messageScaleDeadKill;
        }
        else if (type == "glide")
        {
            firstStr = new string($"{messageDeadGlideA}{killingObject.GetComponent<Keywords>().killerName}{messageDeadGlideB}");
            secondStr = messageDeadGlideC;
            firstScale = messageScaleDeadGlide;
        }
        else if (type == "crash")
        {
            firstStr = messageDeadCrashA;
            secondStr = messageDeadCrashB;
            firstScale = messageScaleDeadCrash;
        }

        narrationText.GetComponent<TextMeshPro> ().text = firstStr;
        narrationCamera.GetComponent<Camera>().Render();
        messageGO.transform.position = cam.transform.position + messagePositionFar;
        messageBox.transform.localScale = firstScale;
        messageGO.SetActive(true);
        AudioSource AudioB = gameObject.AddComponent<AudioSource>();
        AudioB.PlayOneShot(deathSnd, 0.75f);
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

        deathDone = true;

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
