using UnityEngine;

[CreateAssetMenu(menuName = "Weapon")]
public class WeaponSO : ScriptableObject
{
    public int damage;
    public float shootCooldown;
    public float reloadTime;
    public int cartridgeAmount;
    public WeaponType weaponType;
}

public enum WeaponType { Pistol, Rifle, Shotgun }