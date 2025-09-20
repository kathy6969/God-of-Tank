using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponSelectUI : MonoBehaviour
{
    // Lưu lựa chọn cho scene gameplay
    public static TankShooter.ShootMode selectedMode = TankShooter.ShootMode.Single;

    // Gọi hàm này từ button
    public void ChooseWeapon(int modeIndex)
    {
        selectedMode = (TankShooter.ShootMode)modeIndex;
        Debug.Log("Selected: " + selectedMode);
    }

    // Nút Start Game
    public void StartGame()
    {
        SceneManager.LoadScene("testTank"); // thay "GameScene" bằng tên scene gameplay của bạn
    }
}
