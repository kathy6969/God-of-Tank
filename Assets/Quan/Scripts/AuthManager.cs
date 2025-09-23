using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class AuthManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPassInput;

    public TMP_InputField regEmailInput;
    public TMP_InputField regPassInput;
    public TMP_InputField regRePassInput;

    [Header("Thông báo")]
    public GameObject messagePanel;
    public TMP_Text messageText;

    [Header("UI tài khoản")]
    public TMP_Text accountText;
    public Button logoutButton;

    private FirebaseAuth auth;
    private FirebaseUser user;

    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

        accountText.gameObject.SetActive(false);
        logoutButton.gameObject.SetActive(false);
        messagePanel.SetActive(false);

        // Hiển thị email đã lưu nếu có
        if (PlayerPrefs.HasKey("SavedEmail"))
        {
            string savedEmail = PlayerPrefs.GetString("SavedEmail");
            accountText.text = "Đang đăng nhập: " + savedEmail;
            accountText.gameObject.SetActive(true);
            logoutButton.gameObject.SetActive(true);
        }
    }

    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        user = auth.CurrentUser;
        if (user != null)
        {
            accountText.text = "Đang đăng nhập: " + user.Email;
            accountText.gameObject.SetActive(true);
            logoutButton.gameObject.SetActive(true);

            // Lưu email
            PlayerPrefs.SetString("SavedEmail", user.Email);
            PlayerPrefs.Save();
        }
        else
        {
            accountText.gameObject.SetActive(false);
            logoutButton.gameObject.SetActive(false);
        }
    }

    private void ShowMessage(string msg)
    {
        StopAllCoroutines(); // Dừng các thông báo trước nếu có
        messageText.text = msg;
        messagePanel.SetActive(true);
        StartCoroutine(HideMessageAfterDelay(3f)); // Tự tắt sau 3 giây
    }

    private IEnumerator HideMessageAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        messagePanel.SetActive(false);
    }

    public void CloseMessagePanel()
    {
        messagePanel.SetActive(false);
    }

    // -------- Đăng nhập --------
    public void OnLogin()
    {
        string email = loginEmailInput.text.Trim();
        string pass = loginPassInput.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
        {
            ShowMessage("⚠️ Vui lòng nhập email và mật khẩu.");
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, pass).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                ShowMessage("❌ Đăng nhập thất bại: Vui lòng kiểm tra lại email và mật khẩu.");
                return;
            }

            user = task.Result.User;
            ShowMessage("✅ Đăng nhập thành công!");

            // Lưu email
            PlayerPrefs.SetString("SavedEmail", user.Email);
            PlayerPrefs.Save();

            // Cập nhật UI ngay
            accountText.text = "Đang đăng nhập: " + user.Email;
            accountText.gameObject.SetActive(true);
            logoutButton.gameObject.SetActive(true);
        });
    }

    // -------- Đăng ký --------
    public void OnRegister()
    {
        string email = regEmailInput.text.Trim();
        string pass = regPassInput.text.Trim();
        string repass = regRePassInput.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(repass))
        {
            ShowMessage("⚠️ Vui lòng nhập đầy đủ thông tin.");
            return;
        }

        if (pass != repass)
        {
            ShowMessage("⚠️ Mật khẩu nhập lại không khớp.");
            return;
        }

        if (pass.Length < 6)
        {
            ShowMessage("⚠️ Mật khẩu phải ít nhất 6 ký tự.");
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, pass).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                ShowMessage("❌ Đăng ký thất bại: Vui lòng kiểm tra lại email và mật khẩu.");
                return;
            }

            user = task.Result.User;
            ShowMessage("✅ Đăng ký thành công! Hãy đăng nhập.");
        });
    }

    // -------- Đăng xuất --------
    public void OnLogout()
    {
        auth.SignOut();
        accountText.gameObject.SetActive(false);
        logoutButton.gameObject.SetActive(false);
        ShowMessage("📤 Bạn đã đăng xuất.");

        PlayerPrefs.DeleteKey("SavedEmail");
    }

    // -------- Nút Play --------
    public void OnPlay()
    {
        if (user == null)
        {
            ShowMessage("⚠️ Bạn chưa đăng nhập.");
        }
        else
        {
            SceneManager.LoadScene("Start");
        }
    }
}
