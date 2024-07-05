using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPlayerStats : MonoBehaviour
{
    [SerializeField] TMP_Text lengthText;

    private void OnEnable()
    {
        PlayerLength.ChangedLengthEvent += ChangeLengthText;
    }

    private void OnDisable()
    {
        PlayerLength.ChangedLengthEvent -= ChangeLengthText;
    }

    private void ChangeLengthText(ushort length)
    {
        lengthText.text = length.ToString();
    }
}
