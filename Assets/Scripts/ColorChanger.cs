using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// #############################################################################################################
// ### Apply this script to a gameobject that should have it's color changed. Insert the color like this:    ###
// ### pipeInstance.GetComponent<ColorChanger>().color = color;                                              ###
// #############################################################################################################

public class ColorChanger : MonoBehaviour
{
    // private variable color
    private Color _color;

    // Public variable with a getter and setter
    public Color color
    {
        get { return _color; }
        set { _color = value; }
    }

    void Start()
    {
        // Get the Renderer component attached to the child GameObject
        Renderer renderer = GetComponentInChildren<Renderer>();

        // Assign the random color to the material's color property
        renderer.material.color = _color;
    }
}