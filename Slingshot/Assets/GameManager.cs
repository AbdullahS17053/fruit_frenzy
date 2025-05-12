using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private CinemachineVirtualCamera ballcam;
    private CinemachineVirtualCamera curtaincam;
    public float camTime;

    public float limit = 60f;
    private float currentTime;
    public Text timerText;
    public bool start = false;
    public GameObject settingsUI;
    public bool isVibrate = true;
    public AudioSource musicSource;
    private PostProcess postProcess;


    void Start()
    {
        postProcess = PostProcess.Instance;

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            ballcam = GameObject.FindGameObjectWithTag("vcam").GetComponent<CinemachineVirtualCamera>();
            curtaincam = GameObject.FindGameObjectWithTag("CurtainCam").GetComponent<CinemachineVirtualCamera>();

            curtaincam.Priority = 20;
        }
        else { 
        
        
        
        }
    }
    public void StartNewGame()
    {
        SceneManager.LoadScene("WindowScene");
    }

    public void ReplayGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Time.timeScale = 1f;

        SceneManager.LoadScene(currentSceneName);
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void backToMainMenu()
    {

        SceneManager.LoadScene("MainMenu");
    }

    public void ShowSettings()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (SceneManager.GetActiveScene().name == "WindowScene")
        {
            postProcess.BlurScreenOn();
            Time.timeScale = 0f;
        }
        GameObject settingsUI = GameObject.FindGameObjectWithTag("SettingsUI");
        if (settingsUI != null)
        {
            foreach (Transform child in settingsUI.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("SettingsUI GameObject not found");
        }

    }

    public void HideSettings()
    {

        if (SceneManager.GetActiveScene().name == "WindowScene")
        {
            postProcess.BlurScreenOff();
            Time.timeScale = 1f;
        }

        GameObject settingsUI = GameObject.FindGameObjectWithTag("SettingsUI");
        if (settingsUI != null)
        {
            foreach (Transform child in settingsUI.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("SettingsUI GameObject not found");
        }

    }


    public void TurnOffVibration() { 
    
        isVibrate = false;
    }

    public void TurnOnVibration() { 
    
        isVibrate = true;
        CameraShake.Instance.TriggerVibration();
    }
    public void TurnOnMusic()
    {
        if (musicSource != null)
        {
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("MusicSource is not assigned.");
        }
    }
  


    public void TurnOffMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
        else
        {
            Debug.LogWarning("MusicSource is not assigned.");
        }
    }
    public void levelStart(float time)
    {
        currentTime = time;
        start = true;
       
    }
    public void levelStop()
    {
        currentTime = 0;
        start = false;
        
    }
    void TimerEnded()
    {
        // Add any additional actions to perform when the timer ends
    }

    public void ChangeCameraPriorityToCurtains()
    {
        curtaincam.Priority = 20;
        ballcam.Priority = 10;
    }

    public void ChangeCameraPriorityToBall()
    {
        ballcam.Priority = 20;
        curtaincam.Priority = 10;
    }

    IEnumerator StartGameDelay()
    {
        yield return new WaitForSeconds(camTime);
        ChangeCameraPriorityToBall();
    }
}
