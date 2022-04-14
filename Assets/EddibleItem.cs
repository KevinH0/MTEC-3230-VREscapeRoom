using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EddibleItem : MonoBehaviour
{

    GameObject originalFoodItem;
    GameObject eatenFoodItem;

    AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        // Handle object being eaten. 
        originalFoodItem.SetActive(false);
        eatenFoodItem.SetActive(true);

        if(audioSource != null) audioSource.Play();

    }
}
