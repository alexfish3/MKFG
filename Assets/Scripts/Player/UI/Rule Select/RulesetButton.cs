using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RulesetButton : MonoBehaviour
{
    [SerializeField] TMP_Text ruleText;
    [SerializeField] Button ruleButton;

    public Button RuleButton {  get { return ruleButton; } }

    public void SetRuleText(string newText)
    {
        ruleText.text = newText;
    }
}
