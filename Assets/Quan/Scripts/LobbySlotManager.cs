using Mirror;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance;

    [Header("Spawn points (4 vị trí trong Lobby)")]
    public Transform[] spawnPoints;

    [Header("UI Slots")]
    public TMP_Text[] playerNameTexts; // 4 text cho tên
    public TMP_Text[] statusTexts;     // 4 text cho trạng thái

    [Header("Buttons")]
    public Button buttonStart; // Host only
    public Button buttonReady; // Client only

    void Awake() => Instance = this;

    void Start()
    {
        if (buttonStart) buttonStart.onClick.AddListener(OnClickStart);
        if (buttonReady) buttonReady.onClick.AddListener(OnClickReady);

        if (buttonStart) buttonStart.gameObject.SetActive(isServer);
        if (buttonReady) buttonReady.gameObject.SetActive(!isServer);
    }

    [ClientRpc]
    public void RpcUpdateAllSlots(string[] names, bool[] occupied, bool[] ready)
    {
        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            if (i < names.Length)
            {
                if (occupied[i])
                {
                    playerNameTexts[i].text = names[i];
                    statusTexts[i].text = ready[i] ? "Ready" : "Not Ready";
                    playerNameTexts[i].color = ready[i] ? Color.green : Color.white;
                }
                else
                {
                    playerNameTexts[i].text = "Empty";
                    statusTexts[i].text = "-";
                    playerNameTexts[i].color = Color.gray;
                }
            }
        }

        if (buttonStart) buttonStart.gameObject.SetActive(NetworkServer.active);
    }

    void OnClickReady()
    {
        if (NetworkClient.connection?.identity == null) return;
        var lp = NetworkClient.connection.identity.GetComponent<LobbyPlayer>();
        if (lp != null) lp.CmdSetReady(!lp.isReady);
    }

    void OnClickStart()
    {
        if (!NetworkServer.active) return;
        ServerTryStartGame();
    }

    [Server]
    void ServerTryStartGame()
    {
        int connected = 0;
        foreach (var kv in NetworkServer.connections)
        {
            var conn = kv.Value;
            if (conn.identity != null)
            {
                connected++;
                var lp = conn.identity.GetComponent<LobbyPlayer>();
                if (lp == null || !lp.isReady) return;
            }
        }

        if (connected < 2) return;

        NetworkManager.singleton.ServerChangeScene("GameScene");
    }
}
