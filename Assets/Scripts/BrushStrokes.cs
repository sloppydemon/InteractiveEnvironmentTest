using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class BrushStrokes : MonoBehaviour
{
    public Texture2D strokeTexture;
    [SerializeField]
    public Light sun;
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
    Camera cam;
    Sprite brushSprite;
    GameObject[] sprites;
    public GameObject prefab;

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
        cam = Camera.main;
        Rect strokeRect = new Rect(0,0,512,512);
        brushSprite = Sprite.Create(strokeTexture, strokeRect, new Vector2(0.5f, 0.5f));
        sprites = new GameObject[(int)xPoints*(int)yPoints];
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i] = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < xPoints; i++)
        {
            for (int j = 0; j < yPoints; j++)
            {
                Ray ray = new Ray();
                float xRnd = Random.Range(-(xInterval/2), xInterval/2);
                float yRnd = Random.Range(-(yInterval/2), yInterval/2);
                ray = cam.ScreenPointToRay(new Vector2(xInterval * i + xRnd, yInterval * j + yRnd));
                RaycastHit hitinfo = new RaycastHit();
                bool hit = Physics.Raycast(ray.origin, ray.direction, out hitinfo, 100f);
                if (hit)
                {
                    sprites[i + (j * i)].name = $"stroke_{i}-{j}";
                    //GameObject stroke = new GameObject($"stroke_{i}-{j}");
                    sprites[i + (j * i)].transform.position = hitinfo.point;
                    sprites[i + (j * i)].transform.forward = -ray.direction;
                    //stroke.transform.position = hitinfo.point;
                    //SpriteRenderer sr = stroke.AddComponent<SpriteRenderer>();
                    SpriteRenderer sr = sprites[i + (j * i)].GetComponent<SpriteRenderer>();
                    //sr.sprite = brushSprite;
                    float NdotL = Vector3.Dot(sun.transform.forward, hitinfo.normal);
                    if (NdotL < 0)
                    {
                        sr.color = Color.white;
                    }
                    else if (NdotL > 0)
                    {
                        sr.color = Color.black;
                    }
                }
            }
        }
    }
}
