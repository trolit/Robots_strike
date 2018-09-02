using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerScore : MonoBehaviour
{
    Player player;

    private void Start()
    {
        player = GetComponent<Player>();
        StartCoroutine(SyncScoreLoop());
    }

    private void OnDestroy()
    {
        // whenever quit the game / disconnect
        if(player != null)
            SyncNow();
    }

    IEnumerator SyncScoreLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(5f);

            SyncNow();
        }
    }

    void SyncNow()
    {
        if (UserAccountManager.IsLoggedIn)
        {
            UserAccountManager.instance.GetData(OnDataReceived);
        }
    }

    void OnDataReceived(string data)
    {
        if (player.kills == 0 && player.deaths == 0)
        {
            return;
        }

        int kills = DataTranslator.DataToKills(data);
        int deaths = DataTranslator.DataToDeaths(data);

        int newKills = player.kills + kills;
        int newDeaths = player.deaths + deaths;

        string newData = DataTranslator.ValuesToData(newKills, newDeaths);

        Debug.Log("Syncing " + newData);

        // reset values
        player.kills = 0;
        player.deaths = 0;

        UserAccountManager.instance.SendData(newData);
    }
}
