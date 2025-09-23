using Mirror;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour
{
    [SyncVar] public string playerName = "";
    [SyncVar] public bool isReady = false;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSetName($"Player {netId}");
    }

    [Command]
    public void CmdSetName(string newName)
    {
        playerName = newName;
        ((CustomNetworkManager)NetworkManager.singleton).RefreshLobbyUI();
    }

    [Command]
    public void CmdSetReady(bool ready)
    {
        isReady = ready;
        ((CustomNetworkManager)NetworkManager.singleton).RefreshLobbyUI();
    }
}
