using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Splines.Interpolators;

public class TimeOfDayColor : MonoBehaviour
{
    Light sun;
    float sunTemp;
    float UdotL;
    float dotLerp;
    public float zenithTemperature;
    public float horizonTemperature;

    void Start()
    {
        sun = GetComponent<Light>();
        sunTemp = sun.colorTemperature;
        UdotL = Vector3.Dot(gameObject.transform.forward, Vector3.up);
        dotLerp = Mathf.SmoothStep(0f, 1f, -UdotL);
        Debug.Log(UdotL.ToString());
        Debug.Log(dotLerp.ToString());
        sun.colorTemperature = Mathf.Lerp(horizonTemperature, zenithTemperature, dotLerp);
    }

    public void SetSunTemperature()
    {
        sun = GetComponent<Light>();
        sunTemp = sun.colorTemperature;
        UdotL = Vector3.Dot(gameObject.transform.forward, Vector3.up);
        dotLerp = Mathf.SmoothStep(0f, 1f, -UdotL);
        Debug.Log(UdotL.ToString());
        Debug.Log(dotLerp.ToString());
        sun.colorTemperature = Mathf.Lerp(horizonTemperature, zenithTemperature, dotLerp);
    }
   
    // Update is called once per frame
    void Update()
    {
    }
}
