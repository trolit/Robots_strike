using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class HostGame : MonoBehaviour
{
    [SerializeField]
    private uint roomSize = 6;

    [SerializeField]
    private Dropdown levelChoose;

    private string roomName;

    private NetworkManager networkManager;

    private void Start()
    {
        networkManager = NetworkManager.singleton;

        // if matchMaker is not enabled
        if(networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
    }

    public void SetRoomName(string _name)
    {
        roomName = _name;
    }

    public void SetLevel()
    {
        if (levelChoose.value == 0)
            NetworkManager.singleton.onlineScene = "arena";
        else if (levelChoose.value == 1)
            NetworkManager.singleton.onlineScene = "dust2";
    }

    public void CreateRoom()
    {
        if(roomName != "" && roomName != null)
        {
            Debug.Log("Creating a Room" + roomName);

            // Create room
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
        }
    }

}
