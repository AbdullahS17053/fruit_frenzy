using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class cherryHelper : MonoBehaviour
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

            // Define the radius of the OverlapSphere
            float radius = 1.0f; // Adjust as needed
            Vector3 explosionPosition = collision.contacts[0].point; // Center of the sphere

            // Visualize the sphere in the Scene view
            //DrawOverlapSphereGizmo(explosionPosition, radius);

            Collider[] colliders = Physics.OverlapSphere(explosionPosition, radius);

            foreach (Collider nearbyObject in colliders)
            {
                if (nearbyObject.CompareTag("WindowObject"))
                {
                    CustomSpeeds customSpeeds = nearbyObject.GetComponent<CustomSpeeds>();
                    if (customSpeeds != null)
                    {
                        customSpeeds.collisionFunc(collision);
                    }
                }
            }
        }
    }

}
