using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Splines.Interpolators;

public class SunControl : MonoBehaviour
{
    Light sun;
    float sunTemp;
    float UdotL;
    float dotLerp;
    public float zenithTemperature;
    public float horizonTemperature;
    public Vector3 rotationClassroom;
    public float intensityClassroom;
    public UnityEngine.Color filterClassroom;
    public Vector3 rotationCorridor;
    public float intensityCorridor;
    public UnityEngine.Color filterCorridor;
    public Vector3 rotationDying;
    public float intensityDying;
    public UnityEngine.Color filterDying;
    public Vector3 rotationDead;
    public float intensityDead;
    public UnityEngine.Color filterDead;
    public float lerpSpeedDying;
    public float lerpSpeedDead;

    void Start()
    {
        sun = GetComponent<Light>();
        sunTemp = sun.colorTemperature;
        UdotL = Vector3.Dot(gameObject.transform.forward, Vector3.up);
        dotLerp = Mathf.SmoothStep(0f, 1f, -UdotL);
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

    public void DyingStart()
    {
        StartCoroutine(Dying());
    }

    public void DeadStart()
    {
        //StopCoroutine(Dying());
        StartCoroutine(Dead());
    }

    IEnumerator Dying()
    {
        Quaternion initQuatA = transform.rotation;
        Quaternion targetQuatA = Quaternion.Euler(rotationDying);

        UnityEngine.Color initColorA = sun.color;
        Vector3 initHSVA = new Vector3(0, 0, 0);
        UnityEngine.Color.RGBToHSV(initColorA, out initHSVA.x, out initHSVA.y, out initHSVA.z);
        UnityEngine.Color targetColorA = filterDying;
        Vector3 targetHSVA = new Vector3(0, 0, 0);
        UnityEngine.Color.RGBToHSV(targetColorA, out targetHSVA.x, out targetHSVA.y, out targetHSVA.z);
        Vector3 lerpHSVA = new Vector3(0, 0, 0);
        UnityEngine.Color lerpColorA = initColorA;

        float initIntensityA = sun.intensity;


        
        float lerpA = 0f;
        while (lerpA < 1f )
        {
            lerpA += lerpSpeedDying * Time.fixedDeltaTime;
            if (lerpA > 1f)
            {
                lerpA = 1f;
            }
            transform.rotation = Quaternion.Lerp(initQuatA, targetQuatA, lerpA);
            SetSunTemperature();
            sun.intensity = Mathf.Lerp(initIntensityA, intensityDying, lerpA);
            lerpHSVA = Vector3.Lerp(initHSVA, targetHSVA, lerpA);
            lerpColorA = UnityEngine.Color.HSVToRGB(lerpHSVA.x, lerpHSVA.y, lerpHSVA.z);
            sun.color = lerpColorA;
            yield return null;
        }
    }

    IEnumerator Dead()
    {
        Quaternion initQuatB = transform.rotation;
        Quaternion targetQuatB = Quaternion.Euler(rotationDead);

        UnityEngine.Color initColorB = sun.color;
        Vector3 initHSVB = new Vector3(0, 0, 0);
        UnityEngine.Color.RGBToHSV(initColorB, out initHSVB.x, out initHSVB.y, out initHSVB.z);
        UnityEngine.Color targetColorB = filterDead;
        Vector3 targetHSVB = new Vector3(0, 0, 0);
        UnityEngine.Color.RGBToHSV(targetColorB, out targetHSVB.x, out targetHSVB.y, out targetHSVB.z);
        Vector3 lerpHSVB = new Vector3(0, 0, 0);
        UnityEngine.Color lerpColorB = initColorB;

        float initIntensityB = sun.intensity;

        float lerpB = 0f;
        while (lerpB < 1f)
        {
            lerpB += lerpSpeedDead * Time.fixedDeltaTime;
            if (lerpB > 1f)
            {
                lerpB = 1f;
            }
            transform.rotation = Quaternion.Lerp(initQuatB, targetQuatB, lerpB);
            SetSunTemperature();
            sun.intensity = Mathf.Lerp(initIntensityB, intensityDead, lerpB);
            lerpHSVB = Vector3.Lerp(initHSVB, targetHSVB, lerpB);
            lerpColorB = UnityEngine.Color.HSVToRGB(lerpHSVB.x, lerpHSVB.y, lerpHSVB.z);
            sun.color = lerpColorB;
            yield return null;
        }
    }
}
