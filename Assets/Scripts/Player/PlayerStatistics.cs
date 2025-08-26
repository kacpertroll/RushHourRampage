using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerStatistics : MonoBehaviour
{
    [Header("Combat Stats")]
    public float damage = 5f;
    public float attackCooldown = 1f;
    public float attackRange = 10f;
    public float critChance = 0.01f;   // 10%
    public float critMultiplier = 2f; // obra¿enia krytyczne = dmg * multiplier

    [Header("Survival Stats")]
    public float maxHP = 100f;
    public float currentHP = 100f;
    public float hpRegen = 0f;
    public float immunityTime = 1f;

    [Header("Utility Stats")]
    public float moveSpeed = 5f;
    public float pickupRange = 3f;

    [Header("Progression")]
    public float currentExp = 0f;
    public float currentLevel = 1f;
    public float expToLevelUp = 20f;
    private float finExp = 0f;
    public float overallExp = 0f;

    [Header("UI References")]
    public Slider expBar;

    private void Awake()
    {
        if (expBar != null)
        {
            expBar.maxValue = expToLevelUp;
            expBar.value = 0;
        }
    }

    private void Update()
    {
        if (currentExp >= expToLevelUp)
        {
            currentExp -= expToLevelUp;
            LevelUp();
        }
    }

    public void AddExp(float amount)
    {
        finExp += amount;
        overallExp += amount;

        if (finExp >= expToLevelUp)
        {
            finExp -= expToLevelUp;
            LevelUp();
        }

        if (expBar != null)
        {
            DOTween.Kill("ExpTween");
            DOTween.To(
                () => currentExp,
                x => { currentExp = x; expBar.value = currentExp; },
                finExp,
                0.2f
            )
            .SetEase(Ease.OutQuad)
            .SetId("ExpTween")
            .OnUpdate(() =>
            {
                if (Mathf.Abs(currentExp - finExp) <= 0.01f)
                {
                    currentExp = finExp;
                    expBar.value = currentExp;
                    DOTween.Kill("ExpTween");
                }
            });
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        expToLevelUp *= 1.04f;
        if (expBar != null)
            expBar.maxValue = expToLevelUp;

        Debug.Log($"LEVEL UP! Nowy poziom: {currentLevel}");
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP <= 0) Die();
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        // tutaj logika œmierci
    }
}
