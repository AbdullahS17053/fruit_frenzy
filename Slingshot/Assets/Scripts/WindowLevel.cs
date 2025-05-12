using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using TMPro;

public class WindowLevel : MonoBehaviour
{
    public GameObject[] windows;
    public GameObject[] loadingWindows;
    public float loadTime;
    public float startLodTime;
    public Material originalParday;
    public Material[] parday;
    public Material loadingParday;
    public Color defaultParday;

    public Material originalBackground;
    public Material baseBg;
    public Material[] backgroundMaterials;
    public Material loadingBackgroundMaterial;
    public Color defaultBackground;
    public GameObject bullet;
    public ParticleSystem[] fires;
    public float normalFireLifetime = 1.0f; // Default lifetime for normal fire
    public float fireBoostLifetime = 2.0f; // Lifetime for boosted fire
    public float boostTime = 5.0f; // Duration for which the fire boost should last
    bool fireBoosted = false;

    public float[] time;

    public bool load = true;
    bool play = false;
    public bool level = false;

    public TMP_Text Score;
    int levelNum = 0;
    int oldLevelNum = -1;
    public int levelChangeScore = 50;

    ScoreCounter scoreCounter;
    GameManager gameManager;
    Lives life;
    public GameObject orderImages;
    private PostProcess postProcess;
    public TMP_Text loadingScreenStats;
    public ParticleSystem loadingParticleSystem;

    [Header("Sounds")]
    [SerializeField] AudioClip levelStart;
    [SerializeField] AudioClip levelOver;
    [SerializeField] AudioSource soundSource;

    public GameObject victoryBox;


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        scoreCounter = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ScoreCounter>();
        level = true;
        originalBackground.color = defaultBackground;
        originalParday.color = defaultParday;
        postProcess = PostProcess.Instance;
        //loadingScreenStats = GameObject.FindGameObjectWithTag("LoadingScreenStatsText").GetComponent<TMP_Text>();


        if (loadingParticleSystem != null)
        {
            var mainModule = loadingParticleSystem.main;

            mainModule.loop = true;

            StopParticleSystem();
        }

        // Turn off all GameObjects in the windows array
        for (int i = 0; i < windows.Length; i++)
        {
            if (windows[i] != null)
            {
                windows[i].SetActive(false);
            }
        }
        

        // Turn off all GameObjects in the loadingWindows array
        for (int i = 0; i < loadingWindows.Length; i++)
        {
            if (loadingWindows[i] != null)
            {
                loadingWindows[i].SetActive(false);
            }
        }

        foreach (ParticleSystem fire in fires)
        {
            fire.Play();
        }

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        life = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Lives>();
    }

    // Update is called once per frame

    public void changeLevel()
    {
        soundSource.clip = levelOver;
        soundSource.Play();
        levelNum = (levelNum + 1) % windows.Length;
        level = true;
    }
    void Update()
    {


        if (level && load)
        {
            if(levelNum > 0)
            {
               
                windows[levelNum - 1].SetActive(false);
                originalParday.color = loadingParday.color;

            }
               

            else
            {
               
                windows[windows.Length - 1].SetActive(false);
                originalParday.color = defaultParday;
            }
              
            SetFiresLifetimeToZero();
            
            loadingWindows[levelNum].SetActive(true);
            originalBackground.color = loadingBackgroundMaterial.color;
            baseBg.color = loadingBackgroundMaterial.color;
            load = false;
            bullet.SetActive(false);
            gameManager.levelStop();
       
            gameManager.ChangeCameraPriorityToCurtains();
            postProcess.BlurScreenOn();
            DisplayLoadingScreenStats();
            if (levelNum == 0)
                StartCoroutine(loadingLevel());
            else {
                //Time.timeScale = 0f;
                GameObject victory = GameObject.FindGameObjectWithTag("LoadingScore");
                if (victory != null)
                {
                    foreach (Transform child in victory.transform)
                    {
                        child.gameObject.SetActive(true);
                    }
                }
                //loadingScreenStats = GameObject.FindGameObjectWithTag("LoadingScreenStatsText").GetComponent<TMP_Text>();
            }
           


        }

        if (level && play)
        {
            //new level 

            life.resetHealth();
            scoreCounter.ResetScore();
            soundSource.clip = levelStart;
            soundSource.Play();

            play = false;
            loadingWindows[levelNum].SetActive(false);
            windows[levelNum].SetActive(true);
            windows[levelNum].GetComponentInChildren<MainSpawner>().enabled = false;

            //if blue windows then show order images only
            if (windows[levelNum].name == "Blue Windows") {
                if (orderImages != null) { 
                
                    orderImages.SetActive(true);
                }

            }
            else orderImages.SetActive(false);


            StartCoroutine(startSpawner());
            originalBackground.color = backgroundMaterials[levelNum].color;
            baseBg.color = backgroundMaterials[levelNum].color;
            originalParday.color = parday[levelNum].color;
           
            postProcess.BlurScreenOff();
            gameManager.ChangeCameraPriorityToBall();

            //
            GameObject victory = GameObject.FindGameObjectWithTag("LoadingScore");
            if (victory != null)
            {
                foreach (Transform child in victory.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }

            load = true;
            level = false;
        }
    }

    private void DisplayLoadingScreenStats()
    {

        loadingScreenStats.gameObject.SetActive(true);
        int showscore = int.Parse(Score.text);

        //string showLevelName = string.Empty;
        //if (levelNum == 0) { showLevelName = "TRICKSTER"; } else if (levelNum == 1) { showLevelName = "PATTERN"; } else if (levelNum == 2) { showLevelName = "PATTERN 2"; } else if (levelNum == 3) { showLevelName = "PATTERN 3"; }

        loadingScreenStats.text = showscore.ToString();
    }

    public void NextLevel() {

        GameObject[] objectsToDelete = GameObject.FindGameObjectsWithTag("WindowObject");

        // Loop through and delete each object
        foreach (GameObject obj in objectsToDelete)
        {
            Destroy(obj);
        }
        play = true;
        bullet.SetActive(true);
        bullet.GetComponent<BulletSpawner>().levelChange(0f);
        SetFiresToNormal();
        //scoreCounter.ResetScore();
        gameManager.levelStart(time[levelNum]);

        StopParticleSystem();

        GameObject victory = GameObject.FindGameObjectWithTag("LoadingScore");
        if (victory != null)
        {
            foreach (Transform child in victory.transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        Time.timeScale = 1f;

    }
    IEnumerator loadingLevel()
    {

        if (levelNum != 0)
        StartParticleSystem();

        if (levelNum == 0) { yield return new WaitForSeconds(startLodTime); } // first level load time
        else { yield return new WaitForSeconds(loadTime); }


        GameObject[] objectsToDelete = GameObject.FindGameObjectsWithTag("WindowObject");

        // Loop through and delete each object
        foreach (GameObject obj in objectsToDelete)
        {
            Destroy(obj);
        }
        play = true;
        bullet.SetActive(true);
        bullet.GetComponent<BulletSpawner>().levelChange(0f);
        SetFiresToNormal();
        //scoreCounter.ResetScore();
        gameManager.levelStart(time[levelNum]);

        StopParticleSystem();
    }

    public void StartParticleSystem()
    {
        if (levelNum == 0) { return; } // dont play at level 0
        loadingParticleSystem.gameObject.SetActive(true);
        if (loadingParticleSystem != null)
        {
            var mainModule = loadingParticleSystem.main;

            mainModule.loop = true;
            loadingParticleSystem.Play();
        }
    }

    public void StopParticleSystem()
    {
        if (loadingParticleSystem != null)
        {
            var mainModule = loadingParticleSystem.main;

            mainModule.loop = false;
            loadingParticleSystem.Stop();
        }
        loadingParticleSystem.gameObject.SetActive(false);
    }

        IEnumerator startSpawner()
    {
        yield return new WaitForSeconds(1f);
        windows[levelNum].GetComponentInChildren<MainSpawner>().enabled = true;
        windows[levelNum].GetComponentInChildren<MainSpawner>().Start();
    }
    public void SetFiresLifetimeToZero()
    {
        foreach (ParticleSystem fire in fires)
        {
            var main = fire.main;
            main.startLifetime = 0f;
        }
    }
    public void SetFiresToNormal()
    {
        foreach (ParticleSystem fire in fires)
        {
            var main = fire.main;
            main.startLifetime = normalFireLifetime;
        }
        fireBoosted = false;
    }
    public void SetFiresToBoost()
    {
        if (fireBoosted)
            return;
        fireBoosted = true;
        foreach (ParticleSystem fire in fires)
        {
            var main = fire.main;
            main.startLifetime = fireBoostLifetime;
        }
        StartCoroutine(RevertToNormalAfterBoost());
    }

    public int GetLevelChangeScore() { return levelChangeScore; }
    public string GetLevelName() { return windows[levelNum].name; }

    private IEnumerator RevertToNormalAfterBoost()
    {
        yield return new WaitForSeconds(boostTime);
        SetFiresToNormal();
    }

    public int GetLevelNum() { 
    
        return levelNum;
    }
}
