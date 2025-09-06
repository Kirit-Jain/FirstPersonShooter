using System;
using StarterAssets;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Refernce Variables")]
    [SerializeField] GameObject fireBallPrefab;
    [SerializeField] Transform spellOrigin;
    

    [Header("Tunning Variables")]
    [SerializeField] float maxChargeTime = 2.0f;

    GameObject chargeSpellInstance;
    TrailRenderer[] spellTrails;
    StarterAssetsInputs starterAssetsInputs;
    float currentChargeTime = 0f;
    bool isCharging = false;
    bool isFullyCharged = false;
    

    void Awake()
    {
        starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
    }

    void Update()
    {
        HandleCharging();
    }

    void HandleCharging()
    {
        if (starterAssetsInputs.charge && !isCharging)
        {
            isCharging = true;
            isFullyCharged = false;
            currentChargeTime = 0f;

            chargeSpellInstance = Instantiate(fireBallPrefab, spellOrigin.position, Quaternion.identity);
            chargeSpellInstance.transform.localScale = Vector3.zero;

            spellTrails = chargeSpellInstance.GetComponentsInChildren<TrailRenderer>();

            if (spellTrails != null)
            {
                foreach (var trail in spellTrails)
                {
                    trail.emitting = false;
                }
            }
        }

        if (!starterAssetsInputs.charge && isCharging)
        {
            isCharging = false;
            CancelCharge();
        }

        if (isCharging && chargeSpellInstance != null)
        {
            chargeSpellInstance.transform.position = spellOrigin.position;

            if (!isFullyCharged)
            {
                currentChargeTime += Time.deltaTime;
                float scale = Mathf.Clamp01(currentChargeTime / maxChargeTime);
                chargeSpellInstance.transform.localScale = Vector3.one * scale;

                if (currentChargeTime >= maxChargeTime)
                {
                    isFullyCharged = true;
                }
            }

            if (starterAssetsInputs.shoot)
            {
                FireChargedSpell();
                starterAssetsInputs.ShootInput(false);
            }
        }

    }

    void FireChargedSpell()
    {
        if (!isFullyCharged || chargeSpellInstance == null) return;

        if (spellTrails != null)
        {
            foreach (var trail in spellTrails)
            {
                trail.emitting = true; 
            }
        }

        Vector3 targetPoint = GetAimTarget();
        chargeSpellInstance.transform.LookAt(targetPoint);
        chargeSpellInstance.GetComponent<ProjectileController>().Fire();

        // starterAssetsInputs.ChargeInput(false);
        chargeSpellInstance = null;
        isCharging = false;
        isFullyCharged = false;
    }

    void CancelCharge()
    {
        if (chargeSpellInstance == null) return;

        Destroy(chargeSpellInstance);
        chargeSpellInstance = null;
    }

    Vector3 GetAimTarget()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity))
        {
            return hit.point;
        }
        else
        {
            return Camera.main.transform.position + Camera.main.transform.forward * 100;
        }
    }
}
