using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject canvasMain;
    public GameObject canvasLogin;
    public GameObject canvasRegister;

    void Start()
    {
        ShowMain(); // khi mở game thì hiện Main
    }

    public void ShowMain()
    {
        canvasMain.SetActive(true);
        canvasLogin.SetActive(false);
        canvasRegister.SetActive(false);
    }

    public void ShowLogin()
    {
        canvasMain.SetActive(false);
        canvasLogin.SetActive(true);
        canvasRegister.SetActive(false);
    }

    public void ShowRegister()
    {
        canvasMain.SetActive(false);
        canvasLogin.SetActive(false);
        canvasRegister.SetActive(true);
    }

    public void CloseLogin()  // Nút X ở Login
    {
        ShowMain();
    }

    public void CloseRegister() // Nút X ở Register
    {
        ShowLogin(); // quay về Login
    }
}
