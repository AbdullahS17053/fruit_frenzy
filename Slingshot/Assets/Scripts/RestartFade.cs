using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartFade : MonoBehaviour
{
    private AudioSource soundSource;
    // Start is called before the first frame update
    void Start()
    {
        soundSource = this.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("WindowObject"))
        {
            if (other.gameObject.GetComponent<CustomSpeeds>() != null)
            other.gameObject.GetComponent<CustomSpeeds>().inFilled = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("WindowObject"))
        {
            CustomSpeeds customSpeeds = other.gameObject.GetComponent<CustomSpeeds>();

            // Check if the component exists
            if (customSpeeds != null)
            {
                // If the component exists, perform the operations
                if (customSpeeds.inFilled)
                {
                    // Start the coroutine defined in the CustomSpeeds component
                    StartCoroutine(customSpeeds.isSeeThru());
                }
            }
            else
            {
                //Debug.Log("CustomSpeeds component not found on the GameObject.");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Projectile"))
        {
            soundSource.Play();
        }
    }
}
