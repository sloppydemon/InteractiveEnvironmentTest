using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class MagpieControl : MonoBehaviour
{
    Rigidbody rb;
    public NavMeshAgent ai;
    public Transform player;
    public Animator aiAnim;
    public GameObject speechPrefab;
    public AudioClip[] audioCall;
    public AudioClip[] audioComment;
    public AudioClip[] audioAttention;
    public AudioClip[] audioAnxious;
    public AudioClip[] audioFeet;
    public AudioClip[] audioFlap;
    public AudioSource snd;
    public Vector2 perchWaitTimeMinMax;
    public bool useLateralSpeedSolver;
    public float flightSpeed;
    public float flightHeightAddend;
    public float reqGravity;
    public float initialPerchSearchDistance;
    public float perchSearchDistanceIncrement;
    public float perchChancePer;
    Vector3 destination;
    bool followingPlayer;
    bool idle;
    bool perching;
    bool offPerching;
    bool flight;
    bool introduction;
    Game game;
    public Camera speechCam;
    public TextMeshPro speechText;
    public float speechPause;
    public string[] speechGreet;
    public string[] speechRandom;
    public string[] speechPerch;
    public string[] speechWait;
    public string[] commentsFall;
    public string[] commentsGlide;
    public string[] commentsKill;
    public string[] commentsCrash;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        followingPlayer = true;
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        snd = gameObject.GetComponent<AudioSource>();
        idle = false;
        offPerching = false;
        StartCoroutine(Greetings());
    }

    void flap()
    {
        //GameObject mag = GameObject.FindGameObjectWithTag("Magpie");
        //Rigidbody magrb = mag.GetComponent<Rigidbody>();
        //magrb.AddForce(-50*mag.transform.forward);
        //magrb.AddForce(50*mag.transform.up);
        snd.PlayOneShot(audioFlap[0]);
    }

    void stepL()
    {
        int clipNo = UnityEngine.Random.Range(0, audioFeet.Length - 1);
        snd.pitch = 1 + UnityEngine.Random.Range(-0.1f, 0.1f);
        snd.PlayOneShot(audioFeet[clipNo], 0.5f);
    }

    void stepR()
    {
        int clipNo = UnityEngine.Random.Range(0, audioFeet.Length - 1);
        snd.pitch = 1 + UnityEngine.Random.Range(-0.1f, 0.1f);
        snd.PlayOneShot(audioFeet[clipNo], 0.5f);
    }

    void Update()
    {
        if (followingPlayer)
        {
            destination = player.position;
        }
        
        if (ai.enabled)
        {
            ai.destination = destination;
            if (!ai.pathPending)
            {
                if (ai.remainingDistance <= ai.stoppingDistance)
                {
                    aiAnim.SetFloat("Speed", ai.velocity.magnitude);
                }
                else
                {
                    aiAnim.SetFloat("Speed", ai.velocity.magnitude);
                }
            }
        }

        if (!idle && !perching && !flight && !introduction)
        {
            if (ai.velocity.magnitude < 0.0001f)
            {
                idle = true;
                StartCoroutine(UrgeToPerch());
            }
            else
            {
                idle = false;
            }
        }

        if (idle || perching)
        {
            if (Vector3.Distance(transform.position, player.transform.position) > 2 * initialPerchSearchDistance)
            {
                if (perching)
                {
                    Debug.Log("Player getting too far away.");
                    if (!offPerching)
                    {
                        Transform destination = player.transform;
                        StartFlyingOffPerch(destination, false);
                    }
                }
                else
                {

                }
            }
        }

        if (flight)
        {
            if (useLateralSpeedSolver)
            {
                rb.AddForce((new Vector3(0,reqGravity,0) + Physics.gravity) * -0.7f);
            }
            else
            {
                rb.AddForce(-Physics.gravity * 0.5f);
            }
            float speedY = rb.velocity.y;
            float speedX = Vector3.Magnitude(new Vector3(rb.velocity.x, 0, rb.velocity.z));
            aiAnim.SetFloat("GlideForward", speedX);
            aiAnim.SetFloat("SpeedZ", speedY);
            snd.pitch = 0.5f + (rb.velocity.magnitude * 0.2f);
        }

    }

    void StartFlyingToPerch(Transform perch, bool cylinder)
    {
        StopCoroutine(UrgeToPerch());
        StartCoroutine(PerchSpeech());
        StartCoroutine(FlyToPerch(perch, cylinder, true));
    }

    void StartFlyingOffPerch(Transform target, bool cylinder)
    {
        StopCoroutine(FlyToPerch(target, cylinder, true));
        StartCoroutine(WaitSpeech());
        StartCoroutine(FlyToPerch(target, false, false));
    }

    private void OnMouseDown()
    {
        if (Input.GetKey(KeyCode.V))
        {
            StartCoroutine(RandomSpeech());
        }
    }

    IEnumerator Greetings()
    {
        yield return new WaitForSeconds(speechPause);
        int i = 0;
        while (i < speechGreet.Length)
        {
            speechText.text = speechGreet[i];
            speechCam.Render();
            GameObject bubble = Instantiate(speechPrefab, transform);
            aiAnim.SetTrigger("Caw");
            int j = UnityEngine.Random.Range(0, audioComment.Length-1);
            snd.pitch = 1 + UnityEngine.Random.Range(-0.1f, 0.1f);
            snd.PlayOneShot(audioComment[j]);
            i++;
            yield return new WaitForSeconds(speechPause);
            Destroy(bubble);
            aiAnim.ResetTrigger("Caw");
        }
    }

    IEnumerator RandomSpeech()
    {
        int k = UnityEngine.Random.Range(0, audioComment.Length - 1);
        int l = UnityEngine.Random.Range(0, speechRandom.Length - 1);
        speechText.text = speechRandom[l];
        speechCam.Render();
        GameObject bubble = Instantiate(speechPrefab, transform);
        aiAnim.SetTrigger("Caw");
        snd.pitch = 1 + UnityEngine.Random.Range(-0.1f, 0.1f);
        snd.PlayOneShot(audioComment[k]);
        yield return new WaitForSeconds(speechPause);
        Destroy(bubble);
        aiAnim.ResetTrigger("Caw");
    }

    IEnumerator PerchSpeech()
    {
        int k = UnityEngine.Random.Range(0, audioCall.Length - 1);
        int l = UnityEngine.Random.Range(0, speechPerch.Length - 1);
        speechText.text = speechPerch[l];
        speechCam.Render();
        GameObject bubble = Instantiate(speechPrefab, transform);
        aiAnim.SetTrigger("Caw");
        snd.pitch = 1 + UnityEngine.Random.Range(-0.1f, 0.1f);
        snd.PlayOneShot(audioCall[k]);
        yield return new WaitForSeconds(speechPause);
        Destroy(bubble);
        aiAnim.ResetTrigger("Caw");
    }

    IEnumerator WaitSpeech()
    {
        int k = UnityEngine.Random.Range(0, audioAttention.Length - 1);
        int l = UnityEngine.Random.Range(0, speechWait.Length - 1);
        speechText.text = speechWait[l];
        speechCam.Render();
        GameObject bubble = Instantiate(speechPrefab, transform);
        aiAnim.SetTrigger("Caw");
        snd.pitch = 1 + UnityEngine.Random.Range(-0.1f, 0.1f);
        snd.PlayOneShot(audioAttention[k]);
        yield return new WaitForSeconds(speechPause);
        Destroy(bubble);
        aiAnim.ResetTrigger("Caw");
    }

    IEnumerator UrgeToPerch()
    {
        float perchSearchDistance = initialPerchSearchDistance;
        float latestPlayerDistance = Vector3.Distance(transform.position, player.transform.position);
        float playerDistance = Vector3.Distance(transform.position, player.transform.position);
        while (idle)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(perchWaitTimeMinMax.x, perchWaitTimeMinMax.y));
            if (playerDistance < latestPlayerDistance + perchSearchDistance)
            {
                GameObject[] allPerches = GameObject.FindGameObjectsWithTag("Perch");

                List<GameObject> viablePerches = new List<GameObject>();
                for (int i = 0; i < allPerches.Length; i++)
                {
                    if (allPerches[i].GetComponentInParent<Keywords>().perchableAtTheMoment)
                    {
                        viablePerches.Add(allPerches[i]);
                    }
                }

                List<GameObject> nearPerches = new List<GameObject>();
                for (int j = 0; j < viablePerches.Count; j++)
                {
                    if (Vector3.Distance(viablePerches[j].transform.position, transform.position) < perchSearchDistance)
                    {
                        nearPerches.Add(viablePerches[j]);
                    }
                }

                if (nearPerches.Count > 0)
                {
                    float urge = UnityEngine.Random.Range(11, 100);
                    float urgeThreshold = 100f - ((float)nearPerches.Count * perchChancePer);
                    if (urge > urgeThreshold)
                    {
                        int perchNo = UnityEngine.Random.Range(0, nearPerches.Count - 1);
                        Transform perch = nearPerches[perchNo].transform;
                        bool cylinder = nearPerches[perchNo].GetComponentInParent<Keywords>().perchTypeCylinder;
                        StartFlyingToPerch(perch, cylinder);
                        idle = false;
                    }
                }
                else
                {
                    latestPlayerDistance = playerDistance;
                    playerDistance = Vector3.Distance(transform.position, player.transform.position);
                    perchSearchDistance += perchSearchDistanceIncrement;
                }
            }
            else
            {
                latestPlayerDistance = playerDistance;
                playerDistance = Vector3.Distance(transform.position, player.transform.position);
                perchSearchDistance += perchSearchDistanceIncrement;
            }
            
        }
        yield return null;
    }

    IEnumerator FlyToPerch(Transform perch, bool cylinder, bool toPerch)
    {
        if (toPerch)
        {
            offPerching = false;
        }
        else
        {
            offPerching = true;
        }

        ai.enabled = false;
        aiAnim.SetBool("Flight", true);

        Vector3 targetPosition = Vector3.zero;
        if (toPerch)
        {
            targetPosition = perch.position + new Vector3(0, 0.02f, 0);
        }
        else
        {
            targetPosition = perch.position + (perch.forward * 0.5f);
        }
        
        Vector3 newVel = Vector3.zero;
        Vector3 newVelB = Vector3.zero;
        float initDistance = Vector3.Distance(transform.position, targetPosition);
        Vector3 initPosition = transform.position;
        float updatedFlightSpeed = flightSpeed;
        bool flightPathViable = false;
        float max_height = Mathf.Max(targetPosition.y, transform.position.y) + flightHeightAddend;
        while (!flightPathViable)
        {
            if (useLateralSpeedSolver)
            {
                Ballistics.solve_ballistic_arc_lateral(transform.position, updatedFlightSpeed, targetPosition, max_height, out newVel, out reqGravity);
                if (newVel.magnitude > 0.0001f)
                {
                    if (reqGravity < -Physics.gravity.y && reqGravity > 0.5f)
                    {
                        flightPathViable = true;
                        break;
                    }
                }
            }
            else
            {
                Ballistics.SolveBallisticArc(transform.position, updatedFlightSpeed, targetPosition, -Physics.gravity.y * 0.1f, out newVel, out newVelB);
                if (newVel.magnitude > 0.0001f)
                {
                    flightPathViable = true;
                    break;
                }
            }

            yield return null;

            Debug.Log($"No flight path solution with flight speed {updatedFlightSpeed}");
            updatedFlightSpeed += 0.1f;
        }

        Debug.Log($"Flight path viable at flight speed {updatedFlightSpeed} with a velocity magnitude of {newVel.magnitude}, requiring a gravity of {reqGravity}.");
        flight = true;
        rb.isKinematic = false;
        transform.forward = newVel.normalized;
        rb.velocity = newVel * 2f;

        yield return null;

        while (Vector3.Distance(transform.position, perch.position) > 0.01f)
        {
            if (Vector3.Distance(transform.position, perch.position) < 0.1f)
            {
                float distLerp = 1 - (Vector3.Distance(transform.position, targetPosition) * 5);
                transform.rotation = Quaternion.Lerp(Quaternion.Euler(rb.velocity.normalized), perch.rotation, distLerp);
                rb.velocity *= distLerp;
                aiAnim.SetLayerWeight(2, distLerp);
                aiAnim.SetLayerWeight(3, distLerp);
            }

            yield return null;

            if (Vector3.Distance(transform.position, initPosition) > initDistance + 0.1f)
            {
                if (toPerch)
                {
                    transform.rotation = perch.rotation;
                    transform.position = perch.position;
                    rb.velocity = Vector3.zero;
                    rb.isKinematic = true;
                    flight = false;
                    idle = false;
                    aiAnim.SetBool("Flight", false);
                    perching = true;
                    aiAnim.SetLayerWeight(2, 1);
                    aiAnim.SetLayerWeight(3, 1);
                    if (cylinder)
                    {
                        aiAnim.SetBool("perchedCylinder", true);
                        aiAnim.SetBool("perchedOther", false);
                    }
                    else
                    {
                        aiAnim.SetBool("perchedOther", true);
                        aiAnim.SetBool("perchedCylinder", false);
                    }
                    yield return null;
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
                    transform.position = targetPosition;
                    rb.velocity = Vector3.zero;
                    rb.isKinematic = true;
                    flight = false;
                    aiAnim.SetBool("Flight", false);
                    perching = false;
                    aiAnim.SetLayerWeight(2, 0);
                    aiAnim.SetLayerWeight(3, 0);
                    aiAnim.SetBool("perchedOther", false);
                    aiAnim.SetBool("perchedCylinder", false);
                    ai.enabled = true;
                    yield return null;
                }
                break;
            }
        }

        if (!perching)
        {
            if (toPerch)
            {
                transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
                transform.position = targetPosition;
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
                flight = false;
                idle = false;
                aiAnim.SetBool("Flight", false);
                perching = true;
                aiAnim.SetLayerWeight(2, 1);
                aiAnim.SetLayerWeight(3, 1);
                if (cylinder)
                {
                    aiAnim.SetBool("perchedCylinder", true);
                    aiAnim.SetBool("perchedOther", false);
                }
                else
                {
                    aiAnim.SetBool("perchedOther", true);
                    aiAnim.SetBool("perchedCylinder", false);
                }
                yield return null;
            }
            else
            {
                transform.rotation = perch.rotation;
                transform.position = perch.position;
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
                flight = false;
                aiAnim.SetBool("Flight", false);
                perching = false;
                aiAnim.SetLayerWeight(2, 0);
                aiAnim.SetLayerWeight(3, 0);
                aiAnim.SetBool("perchedOther", false);
                aiAnim.SetBool("perchedCylinder", false);
                ai.enabled = true;
            }
        }
        

        yield return null;
    }
}
