using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField]
    GameObject playerScoreBoardItem;

    [SerializeField]
    Transform playerScoreBoardList;

    private void OnEnable()
    {
        // Get an array of players
        Player[] players = GameManager.GetAllPlayers();

        foreach(Player player in players)
        {
            GameObject itemGO = Instantiate(playerScoreBoardItem, playerScoreBoardList);
            PlayerScoreBoarditem item = itemGO.GetComponent<PlayerScoreBoarditem>();
            if(item != null)
            {
                item.Setup(player.username, player.kills, player.deaths);
            }
        }
    }

    private void OnDisable()
    {
        // Clean up our list of items
        foreach(Transform child in playerScoreBoardList)
        {
            Destroy(child.gameObject);
        }
    }
}
