using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class freezeHelper : MonoBehaviour
{
    private Lives life;
    // Start is called before the first frame update
    public GameObject[] particle;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        life = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Lives>();
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Projectile"))
        {
            foreach (GameObject obj in particle)
                obj.SetActive(false);

            // Find all objects in the scene with the CustomSpeeds script
            CustomSpeeds[] customSpeedObjects = FindObjectsOfType<CustomSpeeds>();

            // Start the speed lerp coroutine on each CustomSpeeds object
            foreach (CustomSpeeds customSpeed in customSpeedObjects)
            {
                //Debug.Log(customSpeed.gameObject.name);
                customSpeed.StartLerpCoroutine();
            }

        }
           
    }
}
