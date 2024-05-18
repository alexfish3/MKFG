using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideAttack : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject[] hitboxes;
    public HitBoxInfo[] hitboxesInfo;
    [SerializeField] float attackStartup;
    [SerializeField] float attackActive;
    [SerializeField] float attackRecovery;
    [SerializeField] float attackRecoveryIfLanded = 0;
    float attackTime = 0;
    float totalActive;
    float totalRecovery;
    bool active, recovery;
    [SerializeField] bool attackLanded;
    public bool hasLanded()
    {
        for (int i = 0; i < hitboxes.Length; i++)
        {
            if (hitboxesInfo[i].attackLanded)
            {
                return true;
            }
        }
        return false;
    }

    void OnEnable()
    {
        active = false;
        recovery = false;
        attackTime = 0;
        totalActive = attackStartup + attackActive;
        totalRecovery = totalActive + attackRecovery;

        //Set hitbox info
        hitboxesInfo = new HitBoxInfo[hitboxes.Length];
        for (int i = 0; i < hitboxes.Length; i++)
        {
            hitboxesInfo[i] = hitboxes[i].GetComponent<HitBoxInfo>();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (attackTime > attackStartup && !active)
        {
            active = true;
            //enable hitboxes
            for (int i = 0; i < hitboxes.Length; i++)
            {
                hitboxes[i].SetActive(true);
            }
        }
        else if (attackTime > totalActive && !recovery)
        {
            recovery = true;
            //disable hitboxes
            for (int i = 0; i < hitboxes.Length; i++)
            {
                Debug.Log("attackdone");
                hitboxes[i].SetActive(false);
            }
        }
        else if (attackTime > totalRecovery) 
        {
            Debug.Log("attackrecoverydone");
            this.gameObject.SetActive(false);
        }

        attackTime += Time.deltaTime;

    }

    private void FixedUpdate()
    {
        //If attack lands then set recovery to attack landed recovery frames
        if (hasLanded()) { 
            totalRecovery = totalActive + attackRecoveryIfLanded;
            attackLanded = true;
        } else
        {
            attackLanded = false;
        }
        ///
        ///
    }
}
