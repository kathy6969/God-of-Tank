//using Mirror;
//using TMPro;
//using Unity.Netcode;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class HostManager : MonoBehaviour
//{
//    [Header("UI")]
//    public TMP_InputField playerNameInput;
//    public TextMeshProUGUI currentPlayersText;
//    public TextMeshProUGUI roomIDText;
//    public TMP_Text statusText;
//    public GameObject startGameButton;

//    private string roomID;
//    private int maxPlayers = 5;

//    void Start()
//    {
//        roomID = Random.Range(1000, 9999).ToString();
//        roomIDText.text = roomID;
//        currentPlayersText.text = "1 / " + maxPlayers;
//        startGameButton.SetActive(false);
//    }

//    public void CopyRoomID()
//    {
//        GUIUtility.systemCopyBuffer = roomIDText.text;
//        statusText.text = "Đã copy ID phòng!";
//    }

//    public void CreateRoom()
//    {
//        if (string.IsNullOrEmpty(playerNameInput.text))
//        {
//            statusText.text = "Vui lòng nhập tên!";
//            return;
//        }

//        // Start Host
//        NetworkManager.Singleton.StartHost();
//        statusText.text = "Đang chờ người chơi...";
//    }

//    // Gọi khi có client join
//    public void UpdatePlayerCount(int count)
//    {
//        currentPlayersText.text = $"{count} / {maxPlayers}";

//        if (count > 1)
//            startGameButton.SetActive(true); // Enable start game nếu có người khác
//    }

//    public void StartGame()
//    {
//        // Host load scene gameplay
//        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
//    }
//}
