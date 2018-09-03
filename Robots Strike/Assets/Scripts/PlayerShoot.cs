using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private WeaponManager weaponManager;
    private PlayerWeapon currentWeapon;


    private void Start()
    {
        if(cam == null)
        {
            // Debug.Log("No camera referenced!");
            // disabled component
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if(PauseMenu.isOn)
        {
            return;
        }

        if(currentWeapon.bullets < currentWeapon.maxBullets)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                weaponManager.Reload();
                return;
            }
        }

        if(currentWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f/currentWeapon.fireRate);
            }
            else if(Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
    }

    // called only on the server
    // is called on the server when a player shoots
    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    // go from a server to all clients
    // is called on all clients when we need to do shoot effect
    [ClientRpc]
    void RpcDoShootEffect()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }

    // is called on the server when we hit something, takes in the hit point and the normal of the surface
    [Command]
    void CmdOnHit(Vector3 _pos, Vector3 _normal)
    {
        RpcDoHitEffect(_pos, _normal);
    }

    // is called on all client, here we can spawn an cool effects
    [ClientRpc]
    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
    {
        GameObject _hitEffect = (GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));

        // destroy effect after 2 seconds
        Destroy(_hitEffect, 2f);
    }


    // method called only on Client - not SERVER!
    [Client]
    void Shoot()
    {
        if(!isLocalPlayer || weaponManager.isReloading)
        {
            return;
        }

        if(currentWeapon.bullets <= 0)
        {
            // cant fire - you have to reload!
            // weaponManager.Reload();
            return;
        }

        currentWeapon.bullets--;

        // we are shooting, call the on shoot method on the server
        CmdOnShoot();

        RaycastHit _hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask) )
        {
            // we hit PLAYER  
            if(_hit.transform.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage, transform.name);
            }

            // we hit something, call the OnHit method on the server
            CmdOnHit(_hit.point, _hit.normal);
        }
    }

    // Command - method that is called ONLY ON SERVER!
    [Command]
    void CmdPlayerShot(string _playerID, int _damage, string _sourceID)
    {
        // player has been shot
        // Debug.Log(_playerID + " has been shot!");

        Player _player = GameManager.GetPlayer(_playerID);

        _player.RpcTakeDamage(_damage, _sourceID);
    }
}
