using UnityEngine;
using UnityEngine.SceneManagement; // cần để gọi LoadScene

public class SimpleButtonSceneChanger : MonoBehaviour
{
   
    public string sceneName;

    // Gọi từ Button OnClick()
    public void ChangeScene()
    {
       
        SceneManager.LoadScene(sceneName);
    }
}
