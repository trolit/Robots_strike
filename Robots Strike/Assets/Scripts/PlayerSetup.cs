using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]

// dziedziczymy z klasy NetworkBehaviour
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    Camera sceneCamera;

    private void Start()
    {
        // jeżeli nie jesteśmy lokalnym graczem(tym którym sterujemy) to wyłączmy komponenty 
        // ruchu itd u innych postaci żeby nie było tak, że jak jedna postać się przemieszcza
        // to i druga to robi...
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {   // jeżeli wchodzimy do gry, wyłącz kamerę lokalną
            // (tą, która patrzy od góry)
            sceneCamera = Camera.main;

            if(sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
        }

        GetComponent<Player>().Setup();
    }

    // whenever someone joins..
    public override void OnStartClient()
    {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();

        GameManager.RegisterPlayer(_netID, _player);
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    private void OnDisable()
    {
        // jeżeli wychodzimy z gry, włącz kamerę lokalną
        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        // deregister player on disconnection
        GameManager.UnRegisterPlayer(transform.name);
    }
}
