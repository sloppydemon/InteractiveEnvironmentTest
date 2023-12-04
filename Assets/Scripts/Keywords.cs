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
    
    Camera cam;
    Game game;
    GameObject idCube;
    GameObject idCursor;
    bool id;
    Ray ray;
    RaycastHit hitInfo;
    bool hit;

    void Start()
    {
        cam = Camera.main;
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        idCube = null;
        id = false;
        idCursor = null;
    }

    private void OnMouseEnter()
    {
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            if (!id)
            {
                Identify();
            }
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            if (id)
            {
                idCursor.transform.position = RayCaster() + (0.02f * hitInfo.normal);
                idCursor.transform.rotation = Quaternion.Euler(-ray.direction);
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
            Destroy(idCube);
            idCube = null;
            id = false;
        }
    }

    void Identify()
    {
        game.idText.text = displayName;
        game.GetComponent<Camera>().Render();
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
        idCursor = Instantiate(idCursorSelect, RayCaster() + (0.02f * hitInfo.normal), Quaternion.Euler(-ray.direction), game.transform);
        idCube = Instantiate(game.idPrefab, idCursor.transform.position + (0.01f * Vector3.up), idCursor.transform.rotation, game.transform);
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
