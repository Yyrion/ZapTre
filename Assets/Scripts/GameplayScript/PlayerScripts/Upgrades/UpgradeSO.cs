using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade")]
public class UpgradeSO : ScriptableObject
{
    public float Modifier;
    public UpgradeType UpgradeType;
}

public enum UpgradeType { Damage, ShootCooldown, ReloadTime, Health, MaxHealth}

