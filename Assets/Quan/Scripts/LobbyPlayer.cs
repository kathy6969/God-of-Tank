using Mirror;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour
{
    [SyncVar] public string playerName = "";
    [SyncVar] public bool isReady = false;

    // ❌ Không cần OnStartLocalPlayer tự set tên nữa
    // CustomNetworkManager đã gán tên đúng theo slot rồi.
    // Nếu bạn muốn cho người chơi tự nhập tên thì gọi CmdSetName từ UI sau.

    [Command]
    public void CmdSetName(string newName)
    {
        playerName = newName;

        if (NetworkManager.singleton is CustomNetworkManager customNM)
            customNM.RefreshLobbyUI();
    }

    [Command]
    public void CmdSetReady(bool ready)
    {
        isReady = ready;

        if (NetworkManager.singleton is CustomNetworkManager customNM)
            customNM.RefreshLobbyUI();
    }
}
