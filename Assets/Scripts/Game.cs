using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Camera idCamera;
    public TextMeshPro idText;
    public Material idMaterial;
    public GameObject idPrefab;
    public GameObject cursorTalk;
    public GameObject cursorPush;
    public GameObject cursorGrab;
    public GameObject cursorTake;
    public GameObject cursorMount;
    public GameObject cursorLook;
    public string[] physicMaterials;
    public SndsPerMat[] sndArrays;


    void Start()
    {
        QualitySettings.vSyncCount = 10;
        Application.targetFrameRate = 15;
        physicMaterials = new string[sndArrays.Length];
        
        for (int i = 0; i < sndArrays.Length; i++)
        {
            physicMaterials[i] = $"{sndArrays[i].physicMaterial.name} (Instance)";
        }
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = 0.01f;

    }

    void Update()
    {
        
    }
}
