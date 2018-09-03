using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }

        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    // everytime the value changes, its pushed back to ALL the clients
    // awesome feature :O 
    [SyncVar]
    private int currentHealth;

    public float GetHealthPct()
    {
        return (float)currentHealth / maxHealth;
    }
    [SyncVar]
    public string username = "Loading...";

    public int kills;

    public int deaths;

    private bool isFirstSetup = true;

    public void SetupPlayer()
    {
        if (isLocalPlayer)
        {
            // switch camera on local player
            GameManager.instance.SetSceneCameraActive(false);

            // enable player UI
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        CmdBroadCastNewPlayerSetup();
    }

    // telling the server that players should be set up
    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if(isFirstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];

            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            isFirstSetup = false;
        }

        SetDefaults();
    }

    //private void Update()
    //{
    //    if (!isLocalPlayer)
    //        return;

    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        RpcTakeDamage(9999);
    //    }
    //}

    //thanks to ClientRpc if someone gets damaged, the information will spread among
    //all connected players :)
    [ClientRpc]
    public void RpcTakeDamage(int _amount, string _sourceID)
    {
        if (isDead)
            return;

        currentHealth -= _amount;

        // only called on the server:
        // (only host will be able to see this in console)
        Debug.Log(transform.name + " now has " + currentHealth + " health.");

        if (currentHealth <= 0)
        {
            Die(_sourceID);
        }
    }

    private void Die(string _sourceID)
    {
        isDead = true;

        Player sourcePlayer = GameManager.GetPlayer(_sourceID);

        if(sourcePlayer != null)
        {
            sourcePlayer.kills++;
            GameManager.instance.onPlayerKilledCallback.Invoke(username, sourcePlayer.username);
        }


        deaths++;

        // DISABLE COMPONENTS (cannot move, cannot collider with him, etc.)
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        // DISABLE GameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        Debug.Log(transform.name + " is DEAD");

        Collider _col = GetComponent<Collider>();

        if (_col != null)
        {
            _col.enabled = false;
        }

        // spawn a death effect
        GameObject _gfxIns = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(_gfxIns, 3f);

        // switch cameras
        if(isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);

            // disable player UI
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        // CALL RESPAWN METHOD
        StartCoroutine(Respawn());
    }
    
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        // get one of spawn points
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();

        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        // throw message to all clients that player is setting up
        SetupPlayer();
    }

    public void SetDefaults()
    {
        isDead = false;

        currentHealth = maxHealth;

        // disable cursor
        Cursor.visible = false;

        // enable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        // enable gameobjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        // enable collider
        Collider _col = GetComponent<Collider>();

        if(_col != null)
        {
            _col.enabled = true;
        }

        // create spawn effect
        GameObject _gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);

        Destroy(_gfxIns, 3f);
    }
}
