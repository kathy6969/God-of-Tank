//using Firebase;
//using Firebase.Auth;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using TMPro;

//public class FirebaseAuthManager : MonoBehaviour
//{
//    public TMP_InputField emailInput;
//    public TMP_InputField passInput;
//    public TMP_Text messageText;

//    private FirebaseAuth auth;
//    private FirebaseUser user;

//    private void Awake()
//    {
//        auth = FirebaseAuth.DefaultInstance;

//        // 👉 Check xem có user nào đang login sẵn không
//        auth.StateChanged += AuthStateChanged;
//        AuthStateChanged(this, null);
//    }

//    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
//    {
//        if (auth.CurrentUser != user)
//        {
//            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
//            if (!signedIn && user != null)
//            {
//                Debug.Log("Signed out " + user.UserId);
//            }
//            user = auth.CurrentUser;
//            if (signedIn)
//            {
//                Debug.Log("Signed in: " + user.Email);
//                messageText.text = "🔑 Đã đăng nhập: " + user.Email;

//                // 👉 Auto login: nếu user tồn tại thì vào game luôn
//                SceneManager.LoadScene("Start");
//            }
//        }
//    }

//    // ---- Đăng ký ----
//    public void Register()
//    {
//        auth.CreateUserWithEmailAndPasswordAsync(emailInput.text, passInput.text).ContinueWith(task =>
//        {
//            if (task.IsCanceled || task.IsFaulted)
//            {
//                Debug.LogError("Register Failed: " + task.Exception);
//                messageText.text = "❌ Lỗi đăng ký!";
//                return;
//            }
//            user = task.Result;
//            messageText.text = "✅ Đăng ký thành công!";
//        });
//    }

//    // ---- Đăng nhập ----
//    public void Login()
//    {
//        auth.SignInWithEmailAndPasswordAsync(emailInput.text, passInput.text).ContinueWith(task =>
//        {
//            if (task.IsCanceled || task.IsFaulted)
//            {
//                Debug.LogError("Login Failed: " + task.Exception);
//                messageText.text = "❌ Lỗi đăng nhập!";
//                return;
//            }
//            user = task.Result;
//            messageText.text = "✅ Đăng nhập thành công!";
//        });
//    }

//    // ---- Đăng xuất ----
//    public void Logout()
//    {
//        auth.SignOut();
//        messageText.text = "📤 Đã đăng xuất!";
//    }
//}
