using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microlight.MicroBar;
using TMPro;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.Rendering.Universal;

public class Pattern : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed;
    private float acceleration = 1.25f; // Change this value to control the rate of acceleration
    public float rotationSpeed = 100f;
    public bool xRot = false;
    public bool yRot = false;
    public bool zRot = true;
    public float fadeDuration = 1.0f;

    [Header("Window Layers")]
    public LayerMask window1Layer;
    public LayerMask window2Layer;
    public LayerMask window3Layer;
    public LayerMask window4Layer;
    public LayerMask window5Layer;

    [Header("Explosion Settings")]
    [SerializeField] private GameObject _replacement;
    public float explosionForceX = 15f; // Force magnitude in the x direction
    public float explosionForceY = 10f; // Force magnitude in the y direction
    public Transform cube1;      // First cube for x-position randomization
    public Transform cube2;      // Second cube for x-position randomization
    public Transform cube3;      // First cube for z-position randomization
    public Transform cube4;      // Second cube for z-position randomization


    private Rigidbody rb;
    public bool launched = false;
    private MeshRenderer meshRenderer;
    Color startColor;
    Color endColor;

    public GameObject FloatingText;
    private int currRow;
    private bool hasCollided = false;
    private ScoreCounter ScoreScript;
    private Lives life;
    private Lives diamond;
    public bool pan = false;

    private List<string> goodWords;
    private List<string> badWords;
    private float lerpDuration = 0.5f;
    private float bigDuration = 0.25f;// Duration for lerping to and from zero
    private float increaseAmount = 1f; // Amount to increase scale before lerping to zero

    private Vector3 originalScale;
    private Vector3 increasedScale;
    private bool lerping = false;

    private bool hasBeenTeleported = false;
    public GameManager gameManager;
    public WindowLevel windowLevel;

    [Header("Fruit Settings")]
    private GameObject mainFruitPosition;
    public GameObject[] patternFruitPrefabs; // Array to hold different fruit prefabs
    private GameObject[] patternFruitPositions; // Array of positions for pattern fruits

    [Header("Pattern Settings")]
    public int health = 30;
    public int hitValue = 10;
    private int patternLength = 0;
    public int minPatternLength = 3;
    public int maxPatternLength = 5;
    public MainSpawner spawner;

    private List<int> pattern; // List to store the pattern

    private GameObject mainFruit;
    private List<GameObject> patternFruits;
    private List<GameObject> origPattern;
    public List<GameObject> currPattern;

    [Header("Microbar Prefab")]
    [SerializeField] MicroBar patternHealthBar;
    public TMP_Text healthtext;

    [Header("Sounds")]
    [SerializeField] AudioClip choiceSound;
    [SerializeField] AudioClip hurtSound;
    [SerializeField] AudioSource soundSource;
    public bool soundOn = false;

    private int currrHealth;
    private bool anim = false;
    public bool playing = false;
    int index = 0;
    


    void Start()
    {
        spawner = GameObject.FindObjectOfType<MainSpawner>();
        rb = GetComponent<Rigidbody>();
        life = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Lives>();
        meshRenderer = GetComponent<MeshRenderer>();
        mainFruitPosition = GameObject.FindGameObjectWithTag("mainWindow");
        patternFruitPositions = GameObject.FindGameObjectsWithTag("patternWindow");
        patternHealthBar = life.PatternHealthBar;
        patternHealthBar.gameObject.SetActive(true);
        healthtext = GameObject.FindGameObjectWithTag("PatternHealthText").GetComponentInChildren<TMP_Text>();
        currrHealth = health;
        patternHealthBar.Initialize(health);
        UpdateHealth(health);
        goodWords = new List<string> { "Yummy", "Delicious", "Tasty", "Juicy", "Sweet" };
        badWords = new List<string> { "Yuck", "Gross", "Eww", "Disgusting", "Nasty" };

        ScoreScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ScoreCounter>();
        windowLevel = GameObject.FindGameObjectWithTag("GameManager").GetComponent<WindowLevel>();
        originalScale = transform.localScale;
        increasedScale = originalScale + new Vector3(increaseAmount, increaseAmount, increaseAmount);

        

        // Call the method to instantiate fruits
        InstantiateFruits();

        // Call the method to generate a random pattern
        GeneratePattern();
    }

    private void InstantiateFruits()
    {
        playing = false;
        patternFruits = new List<GameObject>();
        origPattern = new List<GameObject>();
        currPattern = new List<GameObject>();
        // Ensure there are enough positions and fruit prefabs
        if (patternFruitPositions.Length < 4 || patternFruitPrefabs.Length < 4)
        {
            Debug.LogError("Not enough positions or fruit prefabs defined.");
            return;
        }

        // Instantiate the main fruit at the designated position
        this.transform.position = mainFruitPosition.transform.position;
        StartCoroutine(LerpToNormalScaleRoutine());

        // Randomly select positions for the pattern fruits
        List<int> availablePositions = Enumerable.Range(0, patternFruitPositions.Length).ToList();

        for (int i = 0; i < 4; i++)
        {
            int randomIndex = Random.Range(0, availablePositions.Count);
            int positionIndex = availablePositions[randomIndex];
            availablePositions.RemoveAt(randomIndex);

            // Instantiate a pattern fruit at the selected position
            GameObject patternFruit = Instantiate(patternFruitPrefabs[i], patternFruitPositions[positionIndex].transform.position, Quaternion.identity);
            patternFruit.transform.SetParent(this.transform.parent);
            patternFruit.transform.position = patternFruitPositions[positionIndex].transform.position;

            patternFruit.GetComponent<PatternHelper>().spawn();
            patternFruit.GetComponent<PatternHelper>().parent = this.gameObject;
            // Store the pattern fruit for later use
            patternFruits.Add(patternFruit);
        }
    }

    private void GeneratePattern()
    {
        // Determine the length of the pattern
        patternLength = Random.Range(minPatternLength, maxPatternLength + 1);

        pattern = new List<int>();

        for (int i = 0; i < patternLength; i++)
        {
            // Randomly select an index from the instantiated pattern fruits
            int randomFruitIndex = Random.Range(0, patternFruits.Count);

            // Add the index to the pattern list
            pattern.Add(randomFruitIndex);
            //patternFruits[randomFruitIndex].GetComponent<PatternHelper>().selected();
        }

        // Debug log to show the generated pattern
        string patternString = string.Join("", pattern.Select(index => patternFruits[index].name));
        Debug.Log("Generated Pattern: " + patternString);
        StartCoroutine(waitDelay(2f));
    }

    private void Update()
    {
        if(anim)
        {
            if (index == patternLength)
            {
                anim = false;
                playing = true;
                index = 0;
                return;
            }
            patternFruits[pattern[index]].GetComponent<PatternHelper>().selected();
            patternFruits[pattern[index]].GetComponent<PatternHelper>().parent = this.gameObject;
            StartCoroutine(waitDelay(2f));
            origPattern.Add(patternFruits[pattern[index]]);
            index++;
            anim = false;
        }
    }

    public void childHit(GameObject obj)
    {
        currPattern.Add(obj);
        if (playing)
        {
            bool correct = true;
            for (int i = 0; i < currPattern.Count; i++)
            {
                if (currPattern[i] != origPattern[i])
                {
                    correct = false;
                    for (int j = 0; j < patternFruits.Count; j++)
                    {
                        Destroy(patternFruits[j]);
                    }
                    CameraShake.Instance.ShakeCamera(1.2f, 5f, 0.5f);
                    life.RemoveHealth(20);


                    obj.GetComponent<PatternHelper>().ShowText(false);
                    playing = false;

                    InstantiateFruits();

                    // Call the method to generate a random pattern
                    GeneratePattern();
                    return;
                }
                else if (i == origPattern.Count - 1)
                {
                    for (int j = 0; j < patternFruits.Count; j++)
                    {
                        Destroy(patternFruits[j]);
                    }
                    RemoveHealth(10);
                    if (currrHealth <= 0)
                    {
                        death();
                        return;
                    }
                    playing = false;

                    InstantiateFruits();

                    // Call the method to generate a random pattern
                    GeneratePattern();
                    return;

                }
                else
                {
                    
                }
            }
            if(correct)
                obj.GetComponent<PatternHelper>().ShowText(true);
        }
    }

    IEnumerator waitDelay(float time)
    {
        yield return new WaitForSeconds(time);
        anim = true;
    }

    public void RemoveHealth(int value)
    {

        currrHealth -= value;
        if (currrHealth <= 0f)
        {
            spawner.setOver();
            spawner.bossOver = true;
            currrHealth = 0;
        }
        soundSource.clip = choiceSound;
        if (soundOn) soundSource.Play();

        // Update HealthBar
        if (patternHealthBar != null) patternHealthBar.UpdateBar(currrHealth, false, UpdateAnim.Damage);
        //leftAnimator.SetTrigger("Damage");
        UpdateHealth(currrHealth);
        
    }

    private void UpdateHealth(int sc)
    {
        healthtext.text = sc.ToString() + "HP";
    }

    IEnumerator LerpToNormalScaleRoutine()
    {

        // Lerp back to slightly increased scale
        yield return StartCoroutine(LerpScale(Vector3.zero, increasedScale, lerpDuration));

        // Lerp back to original scale
        yield return StartCoroutine(LerpScale(increasedScale, originalScale, bigDuration));
        lerping = false;
    }

    IEnumerator LerpScale(Vector3 startScale, Vector3 endScale, float duration)
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            // Calculate the lerp value
            float lerpValue = Mathf.Clamp01(timeElapsed / duration);

            // Lerp the scale
            transform.localScale = Vector3.Lerp(startScale, endScale, lerpValue);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final scale is exactly the endScale
        transform.localScale = endScale;
    }




    public void death()
    {
        int foodlayer = this.gameObject.layer; //check layer of good foood or bad food
        ShowText(currRow, foodlayer);
        launched = true;
        rotationSpeed *= 15f;
        var replacement = Instantiate(_replacement, transform.parent);
        replacement.transform.position = transform.position;
        replacement.transform.rotation = transform.rotation;
        var rbs = replacement.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rbs)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionZ;
            // Calculate the direction from the explosion to the object in world space
            Vector3 force;
            // Calculate the force to apply in the x and y directions only
            if (rb == rbs[0]) // First Rigidbody
            {
                force = new Vector3(
                    -explosionForceX, // Negative x direction
                    explosionForceY,
                    0 // No force in z direction
                );
            }
            else if (rb == rbs[1]) // Second Rigidbody
            {
                force = new Vector3(
                    explosionForceX, // Positive x direction
                    explosionForceY,
                    0 // No force in z direction
                );
            }
            else
            {
                // For any other Rigidbodies, if applicable
                force = new Vector3(
                    explosionForceX,
                    explosionForceY,
                    0
                );
            }

            // Apply the calculated force
            rb.AddForce(force, ForceMode.Impulse);
            

            // Enable gravity
            rb.useGravity = true;

            patternHealthBar.gameObject.SetActive(false);
        }
        soundSource.clip = hurtSound;
        if (soundOn) soundSource.Play();


        //Destroy(gameObject);
        transform.SetParent(null);
        rb.useGravity = true;

        GameObject[] cubesX = GameObject.FindGameObjectsWithTag("cubeX");
        GameObject[] cubesZ = GameObject.FindGameObjectsWithTag("cubeZ");

        // Check if the correct number of cubes were found
        if (cubesX.Length < 2 || cubesZ.Length < 2)
        {
            //Debug.LogError("Not enough cubes found with specified tags.");
            return;
        }

        // Assign the transforms of the found cubes
        cube1 = cubesX[0].transform;
        cube2 = cubesX[1].transform;
        cube3 = cubesZ[0].transform;
        cube4 = cubesZ[1].transform;

        float randomX = Random.Range(cube1.position.x, cube2.position.x);

        // Randomize the z-position between cube3 and cube4
        float randomZ = Random.Range(cube3.position.z, cube4.position.z);

        // Use the y-position from cube1 (assuming all cubes have the same y-position)
        float yPosition = cube1.position.y;

        // Set the Food object's position to the random values
        transform.position = new Vector3(randomX, yPosition, randomZ);
        rb.velocity = new Vector3(0, 0, 0);
        this.gameObject.tag = "WindowObject";
        this.gameObject.GetComponent<Collider>().isTrigger = false;
    }

    public void ShowText(int layer, int foodlayer)
    {
        GameObject textObject = Instantiate(FloatingText, transform.position, Quaternion.identity);

        // Get the TMP_Text component
        TMP_Text textComponent = textObject.GetComponent<TMP_Text>();
        textComponent.fontSize = 2;

        if (textComponent == null)
        {
            //Debug.LogError("TMP_Text component not found on the prefab.");
            return;
        }

        int score = 200;
        textComponent.text = "Boss! " + score.ToString() + "!";
        ScoreScript.AddScore(score);

        // Destroy the text object after a short delay
        Destroy(textObject, 1f);
    }

}
