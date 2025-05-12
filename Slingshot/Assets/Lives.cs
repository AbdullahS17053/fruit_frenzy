using Microlight.MicroBar;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.SceneManagement;

public class Lives : MonoBehaviour
{
    [Header("Microbar Prefab")]
    [SerializeField] MicroBar healthBar;
    public MicroBar TricksterHealthBar;
    public MicroBar PatternHealthBar;


    [Header("Sounds")]
    [SerializeField] AudioClip hurtSound;
    [SerializeField] AudioClip healSound;
    [SerializeField] AudioSource soundSource;
    [SerializeField] Text soundButtonText;
    public bool soundOn = false;

    public TMP_Text health;
    public TMP_Text diamonds;
    public int currLives;
    public int currDiamonds;
    public int maxHP = 100;

    public GameObject bullet;
    public GameObject curtainLeft;
    public GameObject curtainRight;

    private Animator animator1;
    private Animator animator2;
    bool isPlaying = false;

    void Start()
    {
        currLives = maxHP;
        healthBar.Initialize(maxHP);
        UpdateHealth(currLives);

        curtainLeft = GameObject.FindGameObjectWithTag("Curtain1");
        curtainRight = GameObject.FindGameObjectWithTag("Curtain2");
        TricksterHealthBar = GameObject.FindGameObjectWithTag("TricksterHealthBar").GetComponent<MicroBar>();
        TricksterHealthBar.gameObject.SetActive(false);
        PatternHealthBar = GameObject.FindGameObjectWithTag("PatternHealthBar").GetComponent<MicroBar>();
        PatternHealthBar.gameObject.SetActive(false);

        if (curtainLeft != null) { 
        
            animator1 = curtainLeft.GetComponent<Animator>();
        }
        if (curtainRight != null)
        {
            animator2 = curtainRight.GetComponent<Animator>();
        }
    }

    private void Update()
    {

        if (currLives == 0 && isPlaying == false) {
            PlayAnimations();
            isPlaying = true;

        }
    }
    private void PlayAnimations() {

        animator1.SetTrigger("CloseCurtain1");
        animator2.SetTrigger("CloseCurtain2");

        bullet = GameObject.FindGameObjectWithTag("Projectile");
        Destroy(bullet.gameObject, 0.5f);
        StartCoroutine(mainScene());
    }

    IEnumerator mainScene()
    {
        yield return new WaitForSecondsRealtime(2f);
        SceneManager.LoadScene("MainMenu");
    }
    private void UpdateDiamond(int sc)
    {
        diamonds.text = sc.ToString();
    }

    public void AddDiamond() {

        currDiamonds++;
        UpdateDiamond(currDiamonds);
    }

    public void RemoveDiamond(int price)
    {
        if (currDiamonds - price >= 0) { 
        
            currDiamonds -= price;
        }
    }

    private void UpdateHealth(int sc)
    {
        health.text = sc.ToString() + "HP";
    }

    public void AddHealth(int value) {

        currLives += value;
        if (currLives > maxHP) currLives = maxHP;
        soundSource.clip = healSound;
        if (soundOn) soundSource.Play();

        // Update HealthBar
        if (healthBar != null) healthBar.UpdateBar(currLives, false, UpdateAnim.Heal);
        //leftAnimator.SetTrigger("Heal");
        UpdateHealth(currLives);
    }
    public void RemoveHealth(int value) {

        currLives -= value;
        if (currLives < 0f) currLives = 0;
        soundSource.clip = hurtSound;
        if (soundOn) soundSource.Play();

        // Update HealthBar
        if (healthBar != null) healthBar.UpdateBar(currLives, false, UpdateAnim.Damage);
        //leftAnimator.SetTrigger("Damage");
        UpdateHealth(currLives);
    }

    public void resetHealth()
    {
        AddHealth(maxHP);
    }
}
