using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningPath : MonoBehaviour
{

    // A component used to specify what paths are open
    // This is used to connect layout types
    // For example, if we have a layout with a left opening, we need to
    // enable a layout with a right opening, and thus connect the two
    // based on how much from the x or y position it has to be enabled
    // relatived to the enabled layout.

    [SerializeField]
    private bool leftOpen, rightOpen, topOpen, bottomOpen;

    [SerializeField]
    private float xUnit, yUnity;
}
