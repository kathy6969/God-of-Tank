using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CustomNetworkManager : NetworkManager
{
    [Header("Prefabs")]
    public GameObject lobbyPlayerPrefab; // Prefab cho Lobby (có NetworkIdentity + LobbyPlayer)
    public GameObject tankPrefab;        // Prefab cho GameScene (có NetworkIdentity + Tank controller)

    private Dictionary<int, int> connToSlot = new Dictionary<int, int>();
    private bool[] slotUsed = new bool[4];

    int GetFirstFreeSlot()
    {
        for (int i = 0; i < slotUsed.Length; i++)
        {
            if (!slotUsed[i]) return i;
        }
        return -1;
    }

    void UpdateClientsSlotUI()
    {
        if (LobbyManager.Instance == null) return;

        string[] names = new string[slotUsed.Length];
        bool[] occupied = new bool[slotUsed.Length];
        bool[] ready = new bool[slotUsed.Length];

        // Reset toàn bộ slot
        for (int i = 0; i < slotUsed.Length; i++)
        {
            names[i] = "Empty";
            occupied[i] = false;
            ready[i] = false;
        }

        // Gán thông tin player theo slot
        foreach (var kvp in connToSlot)
        {
            int connId = kvp.Key;
            int slot = kvp.Value;

            occupied[slot] = true;
            if (NetworkServer.connections.TryGetValue(connId, out NetworkConnectionToClient conn))
            {
                if (conn.identity != null)
                {
                    var lp = conn.identity.GetComponent<LobbyPlayer>();
                    if (lp != null)
                    {
                        names[slot] = string.IsNullOrEmpty(lp.playerName) ? $"Player {slot + 1}" : lp.playerName;
                        ready[slot] = lp.isReady;
                    }
                }
            }
        }

        LobbyManager.Instance.RpcUpdateAllSlots(names, occupied, ready);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "LobbyScene")
        {
            int free;

            // ✅ Host (connectionId = 0) luôn slot 0
            if (conn.connectionId == 0)
            {
                free = 0;
            }
            else
            {
                free = GetFirstFreeSlot();
            }

            if (free == -1)
            {
                Debug.LogWarning("Lobby full!");
                return;
            }

            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            if (LobbyManager.Instance != null && free < LobbyManager.Instance.spawnPoints.Length)
            {
                pos = LobbyManager.Instance.spawnPoints[free].position;
                rot = LobbyManager.Instance.spawnPoints[free].rotation;
            }

            GameObject playerObj = Instantiate(lobbyPlayerPrefab, pos, rot);
            NetworkServer.AddPlayerForConnection(conn, playerObj);

            connToSlot[conn.connectionId] = free;
            slotUsed[free] = true;

            // ✅ Đặt tên theo slot
            var lp = playerObj.GetComponent<LobbyPlayer>();
            if (lp != null)
            {
                if (free == 0)
                    lp.playerName = "Player 1";
                else
                    lp.playerName = $"Player {free + 1}";
            }

            Debug.Log($"✅ Player {conn.connectionId} assigned to slot {free}");
            UpdateClientsSlotUI();
        }
        else if (sceneName == "GameScene")
        {
            int slot = -1;
            if (!connToSlot.TryGetValue(conn.connectionId, out slot))
                slot = GetFirstFreeSlot();

            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;

            if (FindFirstObjectByType<GameSpawnManager>() is GameSpawnManager gsm && slot < gsm.spawnPoints.Length)
            {
                pos = gsm.spawnPoints[slot].position;
                rot = gsm.spawnPoints[slot].rotation;
            }

            GameObject tank = Instantiate(tankPrefab, pos, rot);
            NetworkServer.AddPlayerForConnection(conn, tank);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (connToSlot.TryGetValue(conn.connectionId, out int slot))
        {
            connToSlot.Remove(conn.connectionId);
            slotUsed[slot] = false;
        }

        base.OnServerDisconnect(conn);
        UpdateClientsSlotUI();
    }

    [Server]
    public void RefreshLobbyUI()
    {
        UpdateClientsSlotUI();
    }
}
