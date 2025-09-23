using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "WeaponData", order = 0)]
public class WeaponData : ScriptableObject {
    public string weaponName;
    public Sprite weaponIcon;
    public GameObject weaponPrefab;
    public string description;
}
