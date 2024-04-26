using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    void Start()
    {
        // Get the Renderer component attached to the child GameObject
        Renderer renderer = GetComponentInChildren<Renderer>();

        // Generate a random color
        Color randomColor = new Color(Random.value, Random.value, Random.value);

        // Assign the random color to the material's color property
        renderer.material.color = randomColor;
    }
}