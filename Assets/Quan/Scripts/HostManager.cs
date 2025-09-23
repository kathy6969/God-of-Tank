using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System.Net;
using System.Net.Sockets;
using UnityEngine.SceneManagement;

public class HostingManager : MonoBehaviour
{
    [Header("Canvas UI")]
    public GameObject canvasMain;
    public GameObject canvasHost;
    public GameObject canvasJoin;

    [Header("Host UI")]
    public TMP_Text hostIPText;
    public Button copyButton;
    public Button createButton;
    public Button enterLobbyButton;
    public Button closeHostButton;

    [Header("Join UI")]
    public TMP_InputField joinIPInput;
    public Button joinLobbyButton;
    public Button closeJoinButton;

    private void Start()
    {
        ShowMain();

        // Host buttons
        createButton.onClick.AddListener(OnCreateRoom);
        copyButton.onClick.AddListener(OnCopyIP);
        enterLobbyButton.onClick.AddListener(OnEnterLobby);
        closeHostButton.onClick.AddListener(CloseHost);

        // Join buttons
        joinLobbyButton.onClick.AddListener(OnJoinLobby);
        closeJoinButton.onClick.AddListener(CloseJoin);
    }

    // ----------------- UI Switch -----------------
    public void ShowMain()
    {
        canvasMain.SetActive(true);
        canvasHost.SetActive(false);
        canvasJoin.SetActive(false);
    }

    public void ShowHost()
    {
        canvasMain.SetActive(false);
        canvasHost.SetActive(true);
        canvasJoin.SetActive(false);

        hostIPText.gameObject.SetActive(false);
        copyButton.gameObject.SetActive(false);
        enterLobbyButton.gameObject.SetActive(false);
    }

    public void ShowJoin()
    {
        canvasMain.SetActive(false);
        canvasHost.SetActive(false);
        canvasJoin.SetActive(true);
    }

    public void CloseHost() => ShowMain();
    public void CloseJoin() => ShowMain();

    // ----------------- Host -----------------
    void OnCreateRoom()
    {
        NetworkManager.singleton.StartHost();

        string localIP = GetLocalIPAddress();
        hostIPText.text = "Room IP: " + localIP;

        hostIPText.gameObject.SetActive(true);
        copyButton.gameObject.SetActive(true);
        enterLobbyButton.gameObject.SetActive(true);
    }

    void OnCopyIP()
    {
        GUIUtility.systemCopyBuffer = hostIPText.text.Replace("Room IP: ", "");
        Debug.Log("Copied IP: " + GUIUtility.systemCopyBuffer);
    }

    void OnEnterLobby()
    {
        if (NetworkServer.active) // chỉ host được phép đổi scene
        {
            NetworkManager.singleton.ServerChangeScene("LobbyScene");
        }
    }


    // ----------------- Join -----------------
    void OnJoinLobby()
    {
        string ip = joinIPInput.text.Trim();
        if (!string.IsNullOrEmpty(ip))
        {
            NetworkManager.singleton.networkAddress = ip;
            NetworkManager.singleton.StartClient();
            Debug.Log("Join lobby with IP: " + ip);
            SceneManager.LoadScene("LobbyScene"); // client cũng load lobby
        }
    }

    // ----------------- Utility -----------------
    string GetLocalIPAddress()
    {
        string localIP = "";
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("8.8.8.8", 65530); // fake connect tới Google DNS
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            localIP = endPoint.Address.ToString();
        }
        return localIP;
    }
}
