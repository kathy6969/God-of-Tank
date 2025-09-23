using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // Chỉ spawn player khi đang ở LobbyScene
        if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            GameObject player = Instantiate(playerPrefab);
            NetworkServer.AddPlayerForConnection(conn, player);
        }
        else
        {
            Debug.Log("Không spawn player ở scene: " + SceneManager.GetActiveScene().name);
        }
    }
}
