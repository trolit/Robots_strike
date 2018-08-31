using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;


public class RoomListItem : MonoBehaviour
{
    // pointer to a lot different functions = delegate
    // a bunch of references to set of a functions that we want to call
    public delegate void JoinRoomDelegate(MatchInfoSnapshot _match);

    // instance of Delegate 
    private JoinRoomDelegate joinRoomCallBack;

    [SerializeField]
    private Text roomNameText;

    private MatchInfoSnapshot match;

    public void Setup(MatchInfoSnapshot _match, JoinRoomDelegate _joinRoomCallBack)
    {
        match = _match;
        joinRoomCallBack = _joinRoomCallBack;

        roomNameText.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ")";
    }

    public void JoinRoom()
    {
        joinRoomCallBack.Invoke(match);
    }
}
