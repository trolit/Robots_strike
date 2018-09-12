using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MsgTypes
{
    public const short PlayerPrefab = MsgType.Highest + 1;

    public class PlayerPrefabMsg : MessageBase
    {
        public short controllerID;
        public short prefabIndex;
    }
}

public class NetManagerCustom : NetworkManager
{
    // in the Network Manager component, you must put your player prefabs 
    // in the Spawn Info -> Registered Spawnable Prefabs section 
    public short playerPrefabIndex;

    private void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if(sceneName == "Lobby")
        {
            Text classText = GameObject.Find("pickedCLasstext").GetComponent<Text>();

            if (Input.GetKeyDown(KeyCode.I))
            {
                playerPrefabIndex = 0;
                classText.text = "Recon";
            }
            else if(Input.GetKeyDown(KeyCode.O))
            {
                playerPrefabIndex = 1;
                classText.text = "Sniper";
            }
            else if(Input.GetKeyDown(KeyCode.P))
            {
                playerPrefabIndex = 2;
                classText.text = "Soldier";
            }
            else
            {
                if (playerPrefabIndex == 0)
                    classText.text = "Recon";
                else if (playerPrefabIndex == 1)
                    classText.text = "Sniper";
                else if (playerPrefabIndex == 2)
                    classText.text = "Soldier";
            }
        }
    }

    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler(MsgTypes.PlayerPrefab, OnResponsePrefab);
        base.OnStartServer();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        client.RegisterHandler(MsgTypes.PlayerPrefab, OnRequestPrefab);
        base.OnClientConnect(conn);
    }

    private void OnRequestPrefab(NetworkMessage netMsg)
    {
        MsgTypes.PlayerPrefabMsg msg = new MsgTypes.PlayerPrefabMsg();
        msg.controllerID = netMsg.ReadMessage<MsgTypes.PlayerPrefabMsg>().controllerID;
        msg.prefabIndex = playerPrefabIndex;
        client.Send(MsgTypes.PlayerPrefab, msg);
    }

    private void OnResponsePrefab(NetworkMessage netMsg)
    {
        MsgTypes.PlayerPrefabMsg msg = netMsg.ReadMessage<MsgTypes.PlayerPrefabMsg>();
        playerPrefab = spawnPrefabs[msg.prefabIndex];
        base.OnServerAddPlayer(netMsg.conn, msg.controllerID);
        Debug.Log(playerPrefab.name + " spawned!");
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        MsgTypes.PlayerPrefabMsg msg = new MsgTypes.PlayerPrefabMsg();
        msg.controllerID = playerControllerId;
        NetworkServer.SendToClient(conn.connectionId, MsgTypes.PlayerPrefab, msg);
    }

    // I have put a toggle UI on gameObjects called PC1 and PC2 to select two different character types.
    // on toggle, this function is called, which updates the playerPrefabIndex
    // The index will be the number from the registered spawnable prefabs that 
    // you want for your player

    //public void UpdatePC()
    //{
    //    Toggle PC1 = GameObject.Find("PC1").GetComponent<Toggle>();
    //    Toggle PC2 = GameObject.Find("PC2").GetComponent<Toggle>();
    //    Toggle PC3 = GameObject.Find("PC3").GetComponent<Toggle>();

    //    if (PC1.isOn)
    //    {
    //        playerPrefabIndex = 0;
    //    }
    //    else if (PC2.isOn)
    //    {
    //        playerPrefabIndex = 1;
    //    }
    //    else if (PC3.isOn)
    //    {
    //        playerPrefabIndex = 2;
    //    }
    //}
}