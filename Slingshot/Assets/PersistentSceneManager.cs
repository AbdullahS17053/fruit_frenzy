using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // For Slider
using TMPro;

public class PersistentSceneManager : MonoBehaviour
{
    public GameObject loadingScreen; // Assign in the Inspector
    public Slider loadingSlider; // Assign in the Inspector
    public TextMeshProUGUI loadingText; // Assign in the Inspector
    public TextMeshProUGUI sentences;

    private string[] loadingPrompts = new string[]
  {
        "Slice and dice! Your fruity adventure is about to begin!",
        "Hold tight! We're prepping the juiciest levels just for you!",
        "Fruit ninja training in progress... Get ready to sharpen those skills!",
        "Good things come to those who wait... Especially juicy fruit!",
        "Our fruit basket is getting packed! Hang on while we set up the fun!",
        "Baskets are being filled, and fruits are getting sliced! 🍇 Almost ready!",
        "Cutting-edge fun is almost here! Prepare to slice your way to victory!",
        "Juicy surprises are coming your way! Get your slicing skills ready!",
        "The fruit carnival is almost open! Stay tuned for juicy action!",
        "Your fruity journey is just around the corner! Get ready to slice and dice!",
        "Hang tight! We’re mixing up some fruity fun just for you!",
        "The fruit frenzy is almost ready! Sharp knives and sharp wits required!",
        "Almost time to cut loose! The fruit fun is about to begin!",
        "Patience is a virtue, and juicy fruits are the reward! Thanks for waiting!"
  };

    private void Start()
    {
        if (loadingText != null && sentences != null)
        {
            DisplaySentences();
            StartCoroutine(AnimateLoadingText());
        }
    }

    // Method to call when the "New Game" button is clicked
    public void LoadGame()
    {
        Debug.Log("Loading Game");
        loadingScreen.SetActive(true); // Show the loading screen
        loadingSlider.value = 0; // Reset slider value
        StartCoroutine(LoadWindowLevelScene());
    }

    private IEnumerator LoadWindowLevelScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("WindowScene");
        asyncLoad.allowSceneActivation = false; // Prevent the scene from activating immediately

        while (!asyncLoad.isDone)
        {
            loadingSlider.value = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            if (asyncLoad.progress >= 0.9f)
            {
                loadingSlider.value = 1f;

                asyncLoad.allowSceneActivation = true;
            }

            yield return new WaitForSeconds(2f);
        }

        loadingScreen.SetActive(false);
    }

    private void DisplaySentences() {

        string prompt = loadingPrompts[Random.Range(0, loadingPrompts.Length)];
        sentences.text = prompt;

    }
    private IEnumerator AnimateLoadingText()
    {
        while (true)
        {
            for (int i = 1; i <= 3; i++)
            {
                loadingText.text = "" + new string('.', i);
                yield return new WaitForSeconds(0.5f); // Adjust the delay as needed
            }
        }
    }
}
