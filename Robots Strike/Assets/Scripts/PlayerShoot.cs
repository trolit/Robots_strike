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
            Debug.Log("No camera referenced!");
            // disabled component
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

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

    // method called only on Client - not SERVER!
    [Client]
    void Shoot()
    {
        // comment
        Debug.Log("SHOOT");

        RaycastHit _hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask) )
        {
            // we hit PLAYER  
            if(_hit.transform.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage);
            }
        }
    }

    // Command - method that is called ONLY ON SERVER!
    [Command]
    void CmdPlayerShot(string _playerID, int _damage)
    {
        // player has been shot
        Debug.Log(_playerID + " has been shot!");

        Player _player = GameManager.GetPlayer(_playerID);

        _player.RpcTakeDamage(_damage);
    }
}
