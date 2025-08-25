using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgrader : MonoBehaviour
{
    public static PlayerUpgrader Instance;
    // Factors that increase as the player levels up
    public float damageInLevelFactor = 0;
    public float attackSpeedInLevelFactor = 0;
    public float legendaryChance = 0;
    public float criticalHitChance = 0;
    public int shotQntInLevel = 0;
    public int jackpotQntDiscount = 0;

    // Permanent factors that persist
    public float damagePermanentFactor = 0;
    public float attackSpeedPermanentFactor = 0;
    public float criticalHitChancePermanet = 0;
    public int shotQntPermanent = 0;
    public int jackpotQntDiscountPermanent = 0;


    public bool hasGrenadeDrone = false;
    public bool hasRicochetDrone = false;
    public bool hasShieldOnStart = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    private void Start()
    {
        LoadPermanentFactors();
    }
    public float GetTotalDamageFactor()
    {
        return 1 + damageInLevelFactor + damagePermanentFactor;
    }
    public float GetTotalAttackSpeedFactor()
    {
        return 1 - (attackSpeedInLevelFactor + attackSpeedPermanentFactor);
    }
    public int GetTotalShotQuantity()
    {
        return shotQntInLevel + shotQntPermanent;
    }
    public int GetTotalJackpotQuantity()
    {
        return jackpotQntDiscount + jackpotQntDiscountPermanent;
    }
    public float GetLegendaryChanceToIncrease()
    {
        return legendaryChance;
    }
    public float GetTotalCriticalHitChance()
    {
        return criticalHitChance + criticalHitChancePermanet;
    }
    public void IncreaseDamageFactor(float amount)
    {
        damageInLevelFactor += amount;
    }
    public void IncreaseAttackSpeedFactor(float amount)
    {
        attackSpeedInLevelFactor += amount;
    }
    public void IncreaseShotQuantity(int amount)
    {
        shotQntInLevel += amount;
    }
    public void IncreaseJackpotDiscount(int amount)
    {
        jackpotQntDiscount += amount;
    }
    public void IncreaseLegendaryChance(float amount)
    {
        legendaryChance += amount;
    }
    public void IncreaseCriticalHitChance(float amount)
    {
        criticalHitChance += amount;
        SavePermanentFactors();
    }
    public void IncreasePermanentJackpotDiscount(int amount)
    {
        jackpotQntDiscountPermanent += amount;
        SavePermanentFactors();
    }
    public void IncreasePermanentDamageFactor(float amount)
    {
        damagePermanentFactor += amount;
        SavePermanentFactors();
    }
    public void IncreasePermanentAttackSpeedFactor(float amount)
    {
        attackSpeedPermanentFactor += amount;
        SavePermanentFactors();
    }
    public void IncreasePermanentShotQuantity(int amount)
    {
        shotQntPermanent += amount;
        SavePermanentFactors();
    }
    public void IncreasePermanentCriticalHitChance(float amount)
    {
        criticalHitChancePermanet += amount;
        SavePermanentFactors();
    }
    public void UnlockShieldOnStart()
    {
        hasShieldOnStart = true;
        SavePermanentFactors() ;
    }
    private void SavePermanentFactors()
    {
        PlayerPrefs.SetFloat("DamagePermanent", damagePermanentFactor);
        PlayerPrefs.SetFloat("AttackSpeedPermanent", attackSpeedPermanentFactor);
        PlayerPrefs.SetInt("ShotQntPermanent", shotQntPermanent);
        PlayerPrefs.SetFloat("CriticalHitChancePermanent", criticalHitChancePermanet);
        PlayerPrefs.SetInt("JackpotQntDiscountPermanent", jackpotQntDiscountPermanent);

        // Save drone booleans
        PlayerPrefs.SetInt("HasBoomerangDrone", hasRicochetDrone ? 1 : 0);
        PlayerPrefs.SetInt("HasGrenadeDrone", hasGrenadeDrone ? 1 : 0);
        PlayerPrefs.SetInt("HasShieldOnStart", hasShieldOnStart ? 1 : 0);
        PlayerPrefs.Save();
    }
    private void LoadPermanentFactors()
    {
        damagePermanentFactor = PlayerPrefs.GetFloat("DamagePermanent", 0);
        attackSpeedPermanentFactor = PlayerPrefs.GetFloat("AttackSpeedPermanent", 0);
        shotQntPermanent = PlayerPrefs.GetInt("ShotQntPermanent", 0);
        criticalHitChancePermanet = PlayerPrefs.GetFloat("CriticalHitChancePermanent", 0);
        jackpotQntDiscountPermanent = PlayerPrefs.GetInt("JackpotQntDiscountPermanent", 0);

        // Load drone booleans
        hasRicochetDrone = PlayerPrefs.GetInt("HasBoomerangDrone", 0) == 1;
        hasGrenadeDrone = PlayerPrefs.GetInt("HasGrenadeDrone", 0) == 1;
        hasShieldOnStart = PlayerPrefs.GetInt("HasShieldOnStart", 0) == 1;
    }
}
