///
/// Created by Alex Fischer | August 2024
/// 

using UnityEngine;
using UnityEngine.UI;

public class SpecialCooldownUI : MonoBehaviour
{
    [Header("UI Info")]
    public Image fillBar;
    public Image cooldownIcon;

    bool filling = false;
    [SerializeField] float cooldown = 0;
    [SerializeField] float cooldownCounter = 999;

    /// <summary>
    /// Initalizes the cooldown ui
    /// </summary>
    /// <param name="Cooldown"></param>
    public void InitalizeCooldown(float Cooldown, Sprite CooldownIcon)
    {
        Debug.Log("setting cooldown to be " +  Cooldown);
        cooldown = Cooldown;
        fillBar.fillAmount = 0;
        cooldownIcon.sprite = CooldownIcon;
    }

    public void StartCooldown()
    {
        if (filling == true || cooldown == 0)
            return;

        filling = true;
        cooldownCounter = 0;
        fillBar.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(cooldownCounter <= cooldown)
        {
            fillBar.fillAmount = cooldownCounter / cooldown;
            cooldownCounter += Time.deltaTime;

            if(cooldownCounter > cooldown)
            {
                this.gameObject.SetActive(false);
                filling = false;
                return;
            }
        }
    }
}
