using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeOptionsScript : MonoBehaviour
{
    public UpgradeSO Upgrade;
    public CameraController CamController;
    public HealthManager HealthManager;
    public void ActualizePanel(UpgradeSO upgrade)
    {
        Upgrade = upgrade;
        TextMeshProUGUI title = transform.Find("Title").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI description = transform.Find("Description").GetComponent<TextMeshProUGUI>();
        if (title != null)
        {
            title.text = upgrade.UpgradeType.ToString();
        } else { Debug.LogError("Not found title"); }

        switch (Upgrade.UpgradeType)
        {
            case UpgradeType.Damage:
                description.text = $"Augmente les dégats des tirs";
                break;

            case UpgradeType.ShootCooldown:
                description.text = $"Diminue le teps entre chaque tir de {upgrade.Modifier}s";
                break;

            case UpgradeType.ReloadTime:
                description.text = $"Diminue le temps de rechargement de {upgrade.Modifier}s";
                break;

            case UpgradeType.Health:
                description.text = $"Soigne de {upgrade.Modifier}";
                break;
        }
        
    }

    public void ApplyUpgrade()
    {
        switch (Upgrade.UpgradeType)
        {
            case UpgradeType.Damage:
                CamController.DamageModifier += Mathf.RoundToInt(Upgrade.Modifier);
                break;

            case UpgradeType.ShootCooldown:
                CamController.TimeModifier = Mathf.Max(0.05f, CamController.Weapon.shootCooldown - Upgrade.Modifier);
                break;

            case UpgradeType.ReloadTime:
                CamController.ReloadModifier = Mathf.Max(0.1f, CamController.Weapon.reloadTime - Upgrade.Modifier);
                break;

            case UpgradeType.Health:
                HealthManager.Heal(Mathf.RoundToInt(Upgrade.Modifier));
                break;
        }
    }
}
