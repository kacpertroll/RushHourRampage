using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerExpManager : MonoBehaviour
{
    public float currentExp = 0f;   // wartoœæ wyœwietlana na barze
    public float currentLevel = 1f;
    public float expToLevelUp = 20f;

    private float overallExp = 0f;
    private float finExp = 0f;      // faktyczna iloœæ exp

    public Slider expBar;

    private void Awake()
    {
        expBar.maxValue = expToLevelUp;
        expBar.value = 0;
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

        // sprawdzamy level up zanim odpalimy tween
        if (finExp >= expToLevelUp)
        {
            finExp -= expToLevelUp;
            LevelUp();
        }

        // animacja p³ynnego przejœcia currentExp - finExp
        DOTween.Kill("ExpTween"); // zabij poprzednie tweeny
        DOTween.To(
            () => currentExp,
            x => {
                currentExp = x;
                expBar.value = currentExp;
            },
            finExp,
            0.5f
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

        Debug.Log($"EXP: {finExp}/{expToLevelUp}");
    }

    private void LevelUp()
    {
        currentLevel++;
        expToLevelUp += 10;

        expBar.maxValue = expToLevelUp;

        Debug.Log($"LEVEL UP! Nowy poziom: {currentLevel}");
    }
}
