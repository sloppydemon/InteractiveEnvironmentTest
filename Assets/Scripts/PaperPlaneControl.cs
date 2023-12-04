using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines.Interpolators;

public class PaperPlaneControl : MonoBehaviour
{
    public float breakingImpulse;
    public bool attached;
    public GameObject attachedTo;
    public bool controlled;
    public Vector3 jointAnchor;
    public Vector3 jointAxis;
    public Vector3 attachAnchor;
    public float springSpring;
    public float springDamping;
    public float springTarget;
    public AudioClip[] audioPaperHit;
    public AudioClip[] audioPaperMove;
    public AudioClip audioPaperFlight;
    public AudioClip audioPaperTake;
    JointSpring jointSpring;
    Rigidbody body;
    HingeJoint joint;
    public GameObject player;
    Vector3 initPos;
    Quaternion initRot;
    public Vector3 attachPos;
    Camera cam;
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
        attached = false;
        attachedTo = null;
        controlled = false;
        body = gameObject.GetComponent<Rigidbody>();
        joint = null;
        jointSpring.spring = springSpring;
        jointSpring.damper = springDamping;
        jointSpring.targetPosition = springTarget;
        initPos = Vector3.zero;
        cam = Camera.main;
        UdotD = 0f;
        UdotF = 0f;
        FdotV = 0f;
        lift = Vector3.zero;
        glide = Vector3.zero;
        pitch = 0f;
        yaw = 0f;
        roll = 0f;
    }
    private void OnMouseDown()
    {
        if (!attached)
        {
            StartCoroutine(Attach());
        }
    }

    private void OnMouseEnter()
    {

    }

    void Update()
    {
        if (body.velocity.magnitude > 0.1f)
        {
            UdotD = Vector3.Dot(transform.up, Vector3.up);
            FdotV = Vector3.Dot(body.velocity.normalized, transform.forward);
            UdotF = Vector3.Dot(transform.forward, Vector3.up);
            DdotV = Vector3.Dot(body.velocity.normalized, transform.up);

            lift = -Physics.gravity * Mathf.Clamp(FdotV, 0f, 1f) * body.velocity.magnitude * 6f * /*Mathf.Abs(UdotD) **/ Time.deltaTime;
            glide = transform.forward * -UdotF * 20f * Time.deltaTime;
            if (attached && controlled)
            {
                pitch = Input.GetAxis("Mouse Y") * 100f * Time.deltaTime;
                roll = -Input.GetAxis("Mouse X") * 100f * Time.deltaTime;
                yaw = Input.GetAxis("Mouse X") * 100f * Time.deltaTime;
            }
            else
            {
                pitch = ((body.velocity.magnitude * Mathf.Clamp(FdotV, 0, 0.5f) * -0.1f) - (DdotV * UdotD * 0.1f) + UdotF) * Time.deltaTime;
            }

            body.velocity = Vector3.RotateTowards(body.velocity, transform.forward * body.velocity.magnitude, 0.1f, Time.deltaTime);
            body.transform.Rotate(pitch, yaw, roll);
            body.AddForce(lift + glide);
        }
    }

    public void Detach()
    {
        Component.Destroy(joint);
        gameObject.GetComponent<MeshCollider>().enabled = true;
        attached = false;
        controlled = false;
        body.velocity = Vector3.zero;
    } 

    IEnumerator Attach()
    {
        player.GetComponent<Frailty>().paperPlane = true;
        player.GetComponent<Frailty>().paperPlaneGO = gameObject;

        initPos = transform.position;
        initRot = transform.rotation;
        Quaternion rot = Quaternion.Euler(Vector3.zero); 

        float lerp = 0f;

        while (lerp < 1f)
        {
            lerp += 10f * Time.fixedDeltaTime;
            rot = Quaternion.Euler(new Vector3(0,player.transform.rotation.eulerAngles.y,0));
            if (lerp > 1f)
            {
                lerp = 1f;
            }
            transform.rotation = Quaternion.Lerp(initRot, rot, lerp);
            transform.position = Vector3.Lerp(initPos, cam.transform.position + attachPos, lerp);
            yield return null;
        }
        
        joint = gameObject.AddComponent<HingeJoint>();
        joint.connectedBody = player.GetComponent<Rigidbody>();
        joint.anchor = jointAnchor;
        joint.axis = jointAxis;
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = attachAnchor;
        joint.useSpring = true;
        joint.spring = jointSpring;
        attachedTo = player;
        attached = true;
        controlled = true;
        gameObject.GetComponent<MeshCollider>().enabled = false;
        yield return null;
    }
}
