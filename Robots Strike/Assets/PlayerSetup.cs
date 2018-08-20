using UnityEngine;
using UnityEngine.Networking;

// dziedziczymy z klasy NetworkBehaviour
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    Camera sceneCamera;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            for(int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
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
    }

    private void OnDisable()
    {
        // jeżeli wychodzimy z gry, włącz kamerę lokalną
        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
    }
}
