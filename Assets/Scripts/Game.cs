using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        QualitySettings.vSyncCount = 10;
        Application.targetFrameRate = 15;
    }

    void Update()
    {
        
    }
}
