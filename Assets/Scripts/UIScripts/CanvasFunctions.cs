using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasFunctions : MonoBehaviour
{
    public TextMeshProUGUI bestWaveText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1.0f;
        if (bestWaveText != null )
        {
            bestWaveText.text = $"Best Wave : {PlayerPrefs.GetInt("MaxWave")}";
        }
    }

    public void LoadScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
