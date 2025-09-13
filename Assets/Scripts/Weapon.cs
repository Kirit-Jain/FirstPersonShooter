using System;
using System.Collections;
using StarterAssets;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Refernce Variables")]
    [SerializeField] Animator animator;
    [SerializeField] GameObject fireBallPrefab;
    [SerializeField] GameObject hitVFXPrefab;
    [SerializeField] Transform spellOrigin;


    [Header("Tunning Variables")]
    [SerializeField] float maxChargeTime = 2.0f;
    [SerializeField] float firedCooldownTimer = 0.5f;


    GameObject chargeSpellInstance;
    TrailRenderer[] spellTrails;
    StarterAssetsInputs starterAssetsInputs;
    float currentChargeTime = 0f;
    bool isCharging = false;
    bool isFullyCharged = false;
    float fireCoolDown = 0f;

    const string IDLE_STRING = "Idle";
    const string CHARGE_STRING = "Charge";
    const string SHOOT_STRING = "Shoot";


    void Awake()
    {
        starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
    }

    void Update()
    {
        if (fireCoolDown > 0)
        {
            fireCoolDown -= Time.deltaTime;
        }

        HandleCharging();
    }

    void HandleCharging()
    {
        if (starterAssetsInputs.charge && !isCharging && fireCoolDown <= 0)
        {
            isCharging = true;
            isFullyCharged = false;
            currentChargeTime = 0f;

            animator.Play(CHARGE_STRING, 0, 0f);

            chargeSpellInstance = Instantiate(fireBallPrefab, spellOrigin.position, Quaternion.identity);
            chargeSpellInstance.transform.localScale = Vector3.zero;
            chargeSpellInstance.GetComponent<ProjectileController>().enabled = false;

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
                return;
            }
        }

    }

    void FireChargedSpell()
    {
        if (!isFullyCharged || chargeSpellInstance == null) return;

        animator.Play(SHOOT_STRING, 0, 0f);
        fireCoolDown = firedCooldownTimer;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity))
        {
            // --- THIS CODE RUNS IF WE HIT SOMETHING ---

            // 1. Deal Damage
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                int damageAmount = fireBallPrefab.GetComponent<ProjectileController>().damageAmount;
                enemyHealth.TakeDamage(damageAmount);
            }

            // 2. Calculate travel time and start the delayed VFX
            float projectileSpeed = fireBallPrefab.GetComponent<ProjectileController>().speed;
            float distance = Vector3.Distance(chargeSpellInstance.transform.position, hit.point);
            float travelTime = distance / projectileSpeed;
            StartCoroutine(DelayedHitVFX(travelTime, hit.point, hit.normal));

            // 3. Fire the cosmetic projectile towards the hit point
            FireCosmeticProjectile(hit.point);
        }
        else
        {
            // --- THIS CODE RUNS IF WE HIT NOTHING (THE INFINITE CASE) ---

            // 1. Calculate a far-away point
            Vector3 targetPoint = Camera.main.transform.position + Camera.main.transform.forward * 200f;
            
            // 2. Fire the cosmetic projectile towards that point
            // (No damage is dealt and no hit VFX is created)
            FireCosmeticProjectile(targetPoint);
        }
        
        chargeSpellInstance = null;
        isCharging = false;
        isFullyCharged = false;
    }
    
    private void FireCosmeticProjectile(Vector3 targetPoint)
    {
        if (chargeSpellInstance == null) return;
        
        if (spellTrails != null)
        {
            foreach (var trail in spellTrails)
            {
                trail.emitting = true;
            }
        }
        
        chargeSpellInstance.transform.LookAt(targetPoint);
        ProjectileController projectile = chargeSpellInstance.GetComponent<ProjectileController>();
        projectile.enabled = true;
        projectile.Fire(targetPoint);
    }

    private IEnumerator DelayedHitVFX(float delay, Vector3 position, Vector3 normal)
    {
        yield return new WaitForSeconds(delay);

        if (hitVFXPrefab != null)
        {
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
            GameObject vfxInstance = Instantiate(hitVFXPrefab, position, rotation);

            Destroy(vfxInstance, 2f);
        }

    }

    void CancelCharge()
    {
        if (chargeSpellInstance == null) return;

        animator.Play(IDLE_STRING);
        Destroy(chargeSpellInstance);
        chargeSpellInstance = null;
    }

    
}
