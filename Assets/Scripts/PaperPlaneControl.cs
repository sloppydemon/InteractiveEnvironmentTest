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
    JointSpring jointSpring;
    Rigidbody body;
    HingeJoint joint;
    GameObject player;
    Vector3 initPos;
    Quaternion initRot;
    public Vector3 attachPos;
    Camera cam;


    void Start()
    {
        attached = false;
        attachedTo = null;
        controlled = false;
        body = gameObject.GetComponent<Rigidbody>();
        joint = null;
        player = GameObject.FindGameObjectWithTag("Player");
        jointSpring.spring = springSpring;
        jointSpring.damper = springDamping;
        jointSpring.targetPosition = springTarget;
        initPos = Vector3.zero;
        cam = Camera.main;
    }
    private void OnMouseDown()
    {
        if (!attached)
        {
            StartCoroutine(Attach());
            //joint = gameObject.AddComponent<HingeJoint>();
            //joint.connectedBody = player.GetComponent<Rigidbody>();
            //joint.anchor = jointAnchor;
            //joint.axis = jointAxis;
            //joint.autoConfigureConnectedAnchor = false;
            //joint.connectedAnchor = attachAnchor;
            //joint.useSpring = true;
            //joint.spring = jointSpring;
        }
    }

    private void OnMouseEnter()
    {

    }

    void Update()
    {
        
    }

    IEnumerator Attach()
    {
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
        yield return null;
    }
}
