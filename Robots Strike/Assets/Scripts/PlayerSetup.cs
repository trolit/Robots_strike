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
        // jeżeli nie jesteśmy lokalnym graczem(tym którym sterujemy) to wyłączmy komponenty 
        // ruchu itd u innych postaci żeby nie było tak, że jak jedna postać się przemieszcza
        // to i druga to robi...
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
