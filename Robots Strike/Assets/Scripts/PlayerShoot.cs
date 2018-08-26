using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private PlayerWeapon weapon;

    [SerializeField]
    private GameObject weaponGFX;

    [SerializeField]
    private string weaponLayerName = "Weapon";

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

        // assign weapon layer name index
        weaponGFX.layer = LayerMask.NameToLayer(weaponLayerName);
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

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, weapon.range, mask) )
        {
            // we hit PLAYER  
            if(_hit.transform.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, weapon.damage);
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
