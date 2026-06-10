using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    [Header("Effects")]
    [SerializeField] private GameObject damageTextPrefab; 
    [SerializeField] private Transform damageTextSpawnPoint; 
    
    public int MaxHP => maxHP;
    private int currentHP;
    private int damageReduction = 0;

    private void Start()
    {
        currentHP = maxHP;
        UpdateUI(); 
    }

    public void AddDamageReduction(int amount)
    {
        damageReduction += amount;
    }

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(0, damage - damageReduction);
        currentHP = Mathf.Max(0, currentHP - finalDamage);
        damageReduction = 0; 
        UpdateUI(); 

        if (finalDamage > 0)
        {
            UIManager uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null) uiManager.ShakeCamera(0.2f, 0.4f);

            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.PlayerHit);

            if (damageTextPrefab != null && damageTextSpawnPoint != null)
            {
                GameObject dmgTextObj = Instantiate(damageTextPrefab, damageTextSpawnPoint.position, Quaternion.identity, damageTextSpawnPoint.parent);
                DamageText dmgTextScript = dmgTextObj.GetComponent<DamageText>();
                if (dmgTextScript != null) dmgTextScript.Setup(finalDamage);
            }
        }

        if (currentHP <= 0 && GameManager.Instance != null) GameManager.Instance.GameOver();
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
        UpdateUI();
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.PlayerBuff);
    }

    public void IncreaseMaxHP(int amount)
    {
        maxHP += amount;
        currentHP += amount;
        UpdateUI();
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.PlayerBuff);
    }

    public void UpdateUI()
    {
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null) uiManager.UpdateHPBar(currentHP, maxHP); 
    }
}