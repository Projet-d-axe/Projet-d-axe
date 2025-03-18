using UnityEngine;
using UnityEngine.UI;

public class ManaSystem : MonoBehaviour
{
    public float maxMana = 100f;
    public float currentMana;
    public float rechargeRate = 5f;
    public Slider manaBar;

    void Start()
    {
        currentMana = maxMana;
    }

    void Update()
    {
        RechargeMana();
        UpdateManaBar();
    }

    void RechargeMana()
    {
        if (currentMana < maxMana)
        {
            currentMana += rechargeRate * Time.deltaTime;
            currentMana = Mathf.Min(currentMana, maxMana);
        }
    }

    void UpdateManaBar()
    {
        if (manaBar != null)
        {
            manaBar.value = currentMana / maxMana;
        }
    }

    public void UseMana(float amount)
    {
        currentMana -= amount;
        currentMana = Mathf.Max(currentMana, 0);
        Debug.Log("Mana utilisé : " + amount + ", Mana restante : " + currentMana);
    }
}