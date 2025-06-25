using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MasterScript : MonoBehaviour
{
    public static MasterScript Master { get; private set; }

    public List<Transform> Spawners;
    public int Wave = 0;
    public GameObject EnnemyPrefab;

    private int numberOfEnnemiesLeft;
    private int numberOfEnnemiesSpawned;

    public TextMeshProUGUI waveText;
    private bool isWaveChanging = false;

    public GameObject GameOverPanel;
    public bool IsGameOver;
    public Slider LifeSlider;

    [Header("Upgrade")]
    public List<UpgradeSO> UpgradeSOs;
    public List<UpgradeOptionsScript> UpgradePanels;
    public GameObject UpgradePanel;

    private void Awake()
    {
        if (Master != null && Master != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Master = this;
        }
        Time.timeScale = 1f;
    }

    void Start()
    {
        waveText.gameObject.SetActive(false);
        GameOverPanel.gameObject.SetActive(false);
        ChangeWave();
    }


    private void ChangeWave()
    {
        StartCoroutine(ChangeWaveRoutine());
    }

    private IEnumerator ChangeWaveRoutine()
    {
        Wave += 1;

        waveText.text = $"Wave {Wave}";
        waveText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        waveText.gameObject.SetActive(false);
        numberOfEnnemiesSpawned = Wave * 3 + 5;
        numberOfEnnemiesLeft = numberOfEnnemiesSpawned;
        for (int i = 0; i < numberOfEnnemiesSpawned; i++)
        {
            Transform spawnPosition = Spawners[Random.Range(0, Spawners.Count)];
            Instantiate(EnnemyPrefab, spawnPosition.position, Quaternion.identity);
        }

        isWaveChanging = false;
    }

    public void ActivateUpgrade()
    {
        Time.timeScale = 0.0001f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        LoadUpgrade();
        UpgradePanel.gameObject.SetActive(true);
    }

    public void DeactivateUpgrade()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UpgradePanel.gameObject.SetActive(false);
        ChangeWave();
    }


    public void EnnemyDied()
    {
        numberOfEnnemiesLeft -= 1;

        if (numberOfEnnemiesLeft <= 0 && !isWaveChanging)
        {
            numberOfEnnemiesLeft = 0;
            isWaveChanging = true;
            ActivateUpgrade();
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0.0001f;
        IsGameOver = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        GameOverPanel.gameObject.SetActive(true);

        if (Wave > PlayerPrefs.GetInt("MaxWave"))
        {
            PlayerPrefs.SetInt("MaxWave", Wave);
        }
    }

    public void LifeActualize(int life)
    {
        LifeSlider.value = life;
    }
    
    private void LoadUpgrade()
    {
        List<UpgradeSO> shuffled = new List<UpgradeSO>(UpgradeSOs);
        ShuffleList(shuffled);
        List<UpgradeSO> chosenUpgrades = shuffled.GetRange(0, 3);

        for (int i = 0; i < UpgradePanels.Count && i < chosenUpgrades.Count; i++)
        {
            UpgradePanels[i].ActualizePanel(chosenUpgrades[i]);
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }
}
