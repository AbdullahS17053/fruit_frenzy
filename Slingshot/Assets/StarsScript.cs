using Microlight.MicroBar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsScript : MonoBehaviour
{

    public MicroBar healthbar;
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;
    private void Update()
    {
        if (healthbar.CurrentValue < 40)
        {
            star1.SetActive(true);
            star2.SetActive(false);
            star3.SetActive(false);
        }
        else if (healthbar.CurrentValue < 80)
        {
            star1.SetActive(true);
            star2.SetActive(true);
            star3.SetActive(false);
        }
        else {

            star1.SetActive(true);
            star2.SetActive(true);
            star3.SetActive(true);
        }
    }
}
