using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFeed : MonoBehaviour
{
    [SerializeField]
    GameObject killFeedItemPrefab;

	void Start ()
    {
        // whenever onPlayerKilledCallback is called , call OnKill method
        GameManager.instance.onPlayerKilledCallback += OnKill;
	}
	
    public void OnKill(string player, string source)
    {
        GameObject go = (GameObject)Instantiate(killFeedItemPrefab, this.transform);
        go.GetComponent<KillFeedItem>().Setup(player, source);
        go.transform.SetSiblingIndex(0);
        Destroy(go, 4f);
    }
}
