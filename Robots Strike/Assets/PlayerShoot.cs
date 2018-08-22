﻿using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    public PlayerWeapon weapon;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private void Start()
    {
        if(cam == null)
        {
            Debug.Log("No camera referenced!");
            // disabled component
            this.enabled = false;
        }
    }

    private void Update()
    {
        if(Input.GetButton("Fire1"))
        {
            Shoot();
        }
    }

    // method called only on Client - not SERVER!
    [Client]
    void Shoot()
    {
        RaycastHit _hit;

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, weapon.range, mask))
        {
            // we hit something
            
            if(_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name);
            }
        }
    }

    // Command - method that is called ONLY ON SERVER!
    [Command]
    void CmdPlayerShot(string _ID)
    {
        // player has been shot
    }
}
