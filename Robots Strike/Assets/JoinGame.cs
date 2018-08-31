using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoinGame : MonoBehaviour
{
    List<GameObject> roomList = new List<GameObject>();

    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private Transform roomListParent;

    private NetworkManager networkManager;

    private void Start()
    {
        networkManager = NetworkManager.singleton;

        if(networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        RefeshRoomList();
    }

    public void RefeshRoomList()
    {
        ClearRoomList();
        networkManager.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);
        status.text = "Loading...";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        status.text = "";

        if(matchList == null)
        {
            status.text = "Couldnt get room list!";
            return;
        }

        foreach(MatchInfoSnapshot match in matchList)
        {
            GameObject _roomListItemGO = Instantiate(roomListItemPrefab);
            _roomListItemGO.transform.SetParent(roomListParent);

            RoomListItem _roomListItem = _roomListItemGO.GetComponent<RoomListItem>();
            if (_roomListItem != null)
            {
                _roomListItem.Setup(match, JoinRoom);
            }

            // player amount, room name and callback function to join the game

            roomList.Add(_roomListItemGO);
        }

        if(roomList.Count == 0)
        {
            status.text = "No rooms at the moment";
        }
    }

    void ClearRoomList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        // remove references
        roomList.Clear();
    }

    public void JoinRoom(MatchInfoSnapshot _match)
    {
        networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        ClearRoomList();
        status.text = "Joining...";
    }
}
