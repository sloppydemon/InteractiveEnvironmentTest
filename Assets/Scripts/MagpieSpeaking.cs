using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MagpieSpeaking : MonoBehaviour
{
    Transform player;
    LookAtConstraint look;
    ConstraintSource target;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target.sourceTransform = player;
        target.weight = 1f;
        look = gameObject.AddComponent<LookAtConstraint>();
        look.AddSource(target);
        look.constraintActive = true;

    }
}
