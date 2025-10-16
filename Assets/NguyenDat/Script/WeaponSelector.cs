using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WeaponSelector : MonoBehaviour
{
    [Header("Weapon List")]
    public WeaponData[] weapons;

    [Header("UI")]
    public Image weaponPreview;
    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI description;

    private int currentIndex = 0;

    void Start()
    {
        ShowWeapon(0);
    }

    public void ShowWeapon(int index)
    {
        if (weapons.Length == 0) return;

        currentIndex = Mathf.Clamp(index, 0, weapons.Length - 1);

        weaponPreview.sprite = weapons[currentIndex].weaponIcon;
        weaponName.text = weapons[currentIndex].weaponName;
        description.text = weapons[currentIndex].description;
    }

    public void NextWeapon()
    {
        int next = (currentIndex + 1) % weapons.Length;
        ShowWeapon(next);
    }

    public void PreviousWeapon()
    {
        int prev = (currentIndex - 1 + weapons.Length) % weapons.Length;
        ShowWeapon(prev);
    }

    public void ConfirmWeapon()
    {
        // Lưu weapon đã chọn vào PlayerPrefs (để load sang gameplay scene)
        PlayerPrefs.SetInt("SelectedWeapon", currentIndex);
        PlayerPrefs.Save();
        Debug.Log("Weapon Selected: " + weapons[currentIndex].weaponName);
        // Load sang scene gameplay ở đây nếu muốn
        SceneManager.LoadScene("testTank"); // Thay "GameScene" bằng tên scene gameplay của bạn
    }
}
