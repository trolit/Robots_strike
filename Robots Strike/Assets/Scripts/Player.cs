using UnityEngine.Networking;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private int maxHealth = 100;

    // everytime the value changes, its pushed back to ALL the clients
    // awesome feature :O 
    [SyncVar]
    private int currentHealth;

    private void Awake()
    {
        SetDefaults();
    }

    public void TakeDamage(int _amount)
    {
        currentHealth -= _amount;

        // only called on the server:
        // (only host will be able to see this in console)
        Debug.Log(transform.name + " now has " + currentHealth + " health.");
    }

    public void SetDefaults()
    {
        currentHealth = maxHealth;
    }
}
