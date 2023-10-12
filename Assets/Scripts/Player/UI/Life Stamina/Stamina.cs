using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    public float value = 100;
    public float maxValue = 100;
    public bool regenStam = true;
    public Slider staminaSlider;

    private void Update() {
        if (value < 0)
            value = 0;
        if (value < maxValue && regenStam)
            value += 0.5f;
        staminaSlider.value = value;
    }
}
