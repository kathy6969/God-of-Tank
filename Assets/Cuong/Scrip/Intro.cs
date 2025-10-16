using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Intro: MonoBehaviour
{
    public string sceneToLoad;        // Tên scene muốn chuyển
    public float delay = 3f;          // Thời gian chờ trước khi mờ dần
    public float fadeDuration = 1f;   // Thời gian hiệu ứng mờ

    private Image image;
    private float timer;
    private bool fading = false;

    void Start()
    {
        image = GetComponent<Image>();
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Sau delay thì bắt đầu mờ dần
        if (timer >= delay && !fading)
        {
            fading = true;
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color color = image.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            image.color = color;
            yield return null;
        }

        // Chuyển scene sau khi mờ xong
        SceneManager.LoadScene(sceneToLoad);
    }
}
