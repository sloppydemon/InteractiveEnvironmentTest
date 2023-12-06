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
    public AudioSource audioSource;
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
        audioSource = gameObject.GetComponent<AudioSource>();

        StartCoroutine(Greetings());
    }

    void flap()
    {
        GameObject mag = GameObject.FindGameObjectWithTag("Magpie");
        Rigidbody magrb = mag.GetComponent<Rigidbody>();
        magrb.AddForce(-50*mag.transform.forward);
        magrb.AddForce(50*mag.transform.up);
    }

    void magpieFeet()
    {
        int clipNo = Random.Range(0, audioFeet.Length - 1);
        audioSource.PlayOneShot(audioFeet[clipNo]);
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
            audioSource.PlayOneShot(audioComment[j]);
            i++;
            yield return new WaitForSeconds(speechPause);
            Destroy(bubble);
            aiAnim.ResetTrigger("Caw");
        }
    }

    IEnumerator RandomSpeech()
    {
        int i = Random.Range(0, speechRandom.Length - 1);
        speechText.text = speechRandom[i];
        speechCam.Render();
        GameObject bubble = Instantiate(speechPrefab, transform);
        aiAnim.SetTrigger("Caw");
        audioSource.PlayOneShot(audioComment[0]);
        yield return new WaitForSeconds(speechPause);
        Destroy(bubble);
        aiAnim.ResetTrigger("Caw");
    }
}
