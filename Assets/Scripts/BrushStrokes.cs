using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushStrokes : MonoBehaviour
{
    public int numPointsOnMaxAxis;
    private float numPointsOnMinAxis;
    [SerializeField]
    float resolutionFactor;
    [SerializeField]
    float xPoints;
    [SerializeField]
    float yPoints;
    [SerializeField]
    float xInterval;
    [SerializeField]
    float yInterval;

    // Start is called before the first frame update
    void Start()
    {
        float maxAxis = Mathf.Max(Screen.width, Screen.height);
        resolutionFactor = 1/(maxAxis/numPointsOnMaxAxis);
        numPointsOnMinAxis = Mathf.Round(Mathf.Min(Screen.width, Screen.height)*resolutionFactor);
        xPoints = Mathf.Round(Screen.width * resolutionFactor);
        yPoints = Mathf.Round(Screen.height * resolutionFactor);
        xInterval = Screen.width/xPoints;
        yInterval = Screen.height/yPoints;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
