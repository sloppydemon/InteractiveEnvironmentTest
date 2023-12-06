using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
    public AudioSource snd;
    Vector3 destination;
    bool followingPlayer;
    Game game;
    public Camera speechCam;
    public TextMeshPro speechText;
    public float speechPause;
    public string[] speechGreet;
    public string[] speechRandom;
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

        StartCoroutine(Greetings());
    }

    void flap()
    {
        GameObject mag = GameObject.FindGameObjectWithTag("Magpie");
        Rigidbody magrb = mag.GetComponent<Rigidbody>();
        magrb.AddForce(-50*mag.transform.forward);
        magrb.AddForce(50*mag.transform.up);
    }

    void stepL()
    {
        int clipNo = Random.Range(0, audioFeet.Length - 1);
        snd.pitch = 1 + Random.Range(-0.1f, 0.1f);
        snd.PlayOneShot(audioFeet[clipNo], 0.5f);
    }

    void stepR()
    {
        int clipNo = Random.Range(0, audioFeet.Length - 1);
        snd.pitch = 1 + Random.Range(-0.1f, 0.1f);
        snd.PlayOneShot(audioFeet[clipNo], 0.5f);
    }

    void Update()
    {
        if (followingPlayer)
        {
            destination = player.position;
        }
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
            int j = Random.Range(0, audioComment.Length-1);
            snd.pitch = 1 + Random.Range(-0.1f, 0.1f);
            snd.PlayOneShot(audioComment[j]);
            i++;
            yield return new WaitForSeconds(speechPause);
            Destroy(bubble);
            aiAnim.SetTrigger("Caw");
        }
    }

    IEnumerator RandomSpeech()
    {
        int k = Random.Range(0, audioComment.Length - 1);
        int l = Random.Range(0, speechRandom.Length - 1);
        speechText.text = speechRandom[l];
        speechCam.Render();
        GameObject bubble = Instantiate(speechPrefab, transform);
        aiAnim.SetTrigger("Caw");
        snd.pitch = 1 + Random.Range(-0.1f, 0.1f);
        snd.PlayOneShot(audioComment[k]);
        yield return new WaitForSeconds(speechPause);
        Destroy(bubble);
        aiAnim.ResetTrigger("Caw");
    }
}
