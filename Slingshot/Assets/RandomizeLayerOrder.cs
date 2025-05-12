using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeLayerOrder : MonoBehaviour
{
    public List<SpriteRenderer> images; // Assign in the Inspector
    public float changeInterval = 1f; // Time interval to change sorting order

    private void Start()
    {
        if (images != null && images.Count > 0)
        {
            StartCoroutine(ChangeLayerOrderCoroutine());
        }
    }

    private IEnumerator ChangeLayerOrderCoroutine()
    {
        while (true)
        {
            // Randomly change the order of each SpriteRenderer
            foreach (var image in images)
            {
                if (image != null)
                {
                    // Randomly decide whether to set order to 1 or 0
                    image.sortingOrder = Random.Range(0, 2);
                }
            }

            // Wait for the specified interval before changing again
            yield return new WaitForSeconds(changeInterval);
        }
    }
}
