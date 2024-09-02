using UnityEngine;
using UnityEngine.UI;

public class HeroStatusBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider rageSlider;
    public Text healthText;
    public Text manaText;
    public Text rageText;
    public Hero hero;

    private void Start()
    {
        if (hero == null)
        {
            hero = FindObjectOfType<Hero>();
            if (hero == null)
            {
                Debug.LogError("Hero not found in the scene!");
                return;
            }
        }

        // 延迟初始化，确保Hero的Start方法已经执行
        Invoke("InitializeStatusBars", 0.1f);
    }

    public void InitializeStatusBars()
    {
        if (hero != null)
        {
            healthSlider.maxValue = hero.maxHealth;
            manaSlider.maxValue = hero.maxMana;
            rageSlider.maxValue = hero.maxRage;
            UpdateStatusBars();
        }
        else
        {
            Debug.LogError("Hero reference is null in HeroStatusBar!");
        }
    }

    public void UpdateStatusBars()
    {
        if (hero == null)
        {
            Debug.LogWarning("Hero reference is null in HeroStatusBar!");
            return;
        }

        healthSlider.maxValue = hero.maxHealth;
        manaSlider.maxValue = hero.maxMana;
        rageSlider.maxValue = hero.maxRage;

        healthSlider.value = hero.health;
        manaSlider.value = hero.mana;
        rageSlider.value = hero.rage;

        if (healthText != null)
            healthText.text = $"{hero.health:F0}/{hero.maxHealth:F0}";
        if (manaText != null)
            manaText.text = $"{hero.mana:F0}/{hero.maxMana:F0}";
        if (rageText != null)
            rageText.text = $"{hero.rage:F0}/{hero.maxRage:F0}";

        Debug.Log($"Updated HeroStatusBar. Health: {hero.health}/{hero.maxHealth}, Mana: {hero.mana}/{hero.maxMana}, Rage: {hero.rage}/{hero.maxRage}");
    }
}