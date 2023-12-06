using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class Keywords : MonoBehaviour
{
    public bool perchable;
    public Dropdown perchType;
    public bool floor;
    public bool canTake;
    public bool canGrab;
    public bool canPush;
    public bool canMount;
    public bool canCommunicate;
    public string displayName;
    public string killerName;
    public Vector3 idScale;
    public Vector3 idOffset;
    public AudioClip[] impactAudioElementA;
    public AudioClip[] impactAudioElementB;
    public PhysicMaterial elementA;
    public PhysicMaterial elementB;
    
    Camera cam;
    Game game;
    GameObject idNote;
    GameObject idCursor;
    bool id;
    Ray ray;
    RaycastHit hitInfo;
    bool hit;

    void Start()
    {
        cam = Camera.main;
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        idNote = null;
        id = false;
        idCursor = null;
    }

    private void OnMouseEnter()
    {
        if (Input.GetKey(KeyCode.V))
        {
            if (!id)
            {
                Identify();
            }
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetKey(KeyCode.V))
        {
            if (id)
            {
                
                if (canPush)
                {
                    idCursor.transform.position = RayCaster() + (0.001f * hitInfo.normal);
                    idCursor.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
                }
                else
                {
                    idCursor.transform.position = RayCaster() + (0.02f * hitInfo.normal);
                    idCursor.transform.rotation = Quaternion.LookRotation(-ray.direction, Vector3.up);
                }
            }
            else
            {
                Identify();
            }
        }
    }

    private void OnMouseExit()
    {
        if (id)
        {
            Destroy(idCursor);
            idCursor = null;
            Destroy(idNote);
            idNote = null;
            id = false;
        }
    }

    void Identify()
    {
        game.idText.text = displayName;
        game.idCamera.Render();
        id = true;
        GameObject idCursorSelect = null;
        if (canCommunicate)
        {
            idCursorSelect = game.cursorTalk;
        }
        else if (canTake)
        {
            idCursorSelect = game.cursorTake;
        }
        else if (canGrab)
        {
            idCursorSelect = game.cursorGrab;
        }
        else if (canPush)
        {
            idCursorSelect = game.cursorPush;
        }
        else if (canMount)
        {
            idCursorSelect = game.cursorMount;
        }
        else
        {
            idCursorSelect = game.cursorLook;
        }
        idCursor = Instantiate(idCursorSelect, RayCaster() + (0.02f * hitInfo.normal), Quaternion.LookRotation(-ray.direction, Vector3.up), game.transform);
        idNote = Instantiate(game.idPrefab, hitInfo.point + (0.002f*hitInfo.normal), Quaternion.LookRotation(hitInfo.normal), game.transform);
    }

    public Vector3 RayCaster()
    {
        hitInfo = new RaycastHit();
        ray = cam.ScreenPointToRay(Input.mousePosition);
        hit = Physics.Raycast(ray, out hitInfo, 15f);
        if (hit)
        {
            return(hitInfo.point);
        }
        else
        {
            return(Vector3.zero);
        }
    }
}
