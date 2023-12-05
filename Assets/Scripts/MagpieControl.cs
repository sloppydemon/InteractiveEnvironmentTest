using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MagpieControl : MonoBehaviour
{
    Rigidbody rb;
    public NavMeshAgent ai;
    public Transform player;
    public Animator aiAnim;
    Vector3 destination;
    bool followingPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        followingPlayer = true;
    }

    void flap()
    {
        GameObject mag = GameObject.FindGameObjectWithTag("Magpie");
        Rigidbody magrb = mag.GetComponent<Rigidbody>();
        magrb.AddForce(-50*mag.transform.forward);
        magrb.AddForce(50*mag.transform.up);
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
}
