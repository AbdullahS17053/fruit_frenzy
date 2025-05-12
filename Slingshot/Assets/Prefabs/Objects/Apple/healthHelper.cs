using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthHelper : MonoBehaviour
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
            life.AddHealth(life.maxHP);
        }
           
    }
}
