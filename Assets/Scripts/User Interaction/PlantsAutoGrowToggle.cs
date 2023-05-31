using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlantsAutoGrowToggle : MonoBehaviour
{
    [SerializeField] TMP_Text toggle;

    Color32 onGreen;
    Color32 offRed;

    private void Awake()
    {
        onGreen = new Color32(51, 246, 0, 255);
        offRed = new Color32(255, 45, 0, 255);

        toggle.text = "OFF";
        toggle.color = offRed;
    }

    public void ToggleText(bool isON)
    {
        if (isON)
        {
            toggle.text = "ON";
            toggle.color = onGreen;
        }
        else
        {
            toggle.text = "OFF";
            toggle.color = offRed;
        }
    }
}
