using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    [Header("Effects")]
    [SerializeField] private GameObject damageTextPrefab; 
    [SerializeField] private Transform damageTextSpawnPoint; 
    
    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;

    public int MaxHP { get { return maxHP; } }

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
        Debug.Log($"[버블 정렬 진화 효과] 다음 적의 공격 데미지가 {amount} 감소합니다!");
    }

    public void TakeDamage(int damage)
    {
        int finalDamage = damage - damageReduction;
        if (finalDamage < 0) finalDamage = 0; 
        
        currentHP -= finalDamage;
        if (currentHP < 0) currentHP = 0;
        damageReduction = 0; 
        UpdateUI(); 

        if (finalDamage > 0)
        {
            CameraManager.Instance.ShakeCamera(0.2f, 0.4f);

            // 🔥 플레이어 피격 효과음 재생
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.PlayerHit);

            if (damageTextPrefab != null && damageTextSpawnPoint != null)
            {
                GameObject dmgTextObj = Instantiate(damageTextPrefab, damageTextSpawnPoint.position, Quaternion.identity, damageTextSpawnPoint.parent);
                DamageText dmgTextScript = dmgTextObj.GetComponent<DamageText>();
                if (dmgTextScript != null) dmgTextScript.Setup(finalDamage);
            }
        }

        if (currentHP <= 0)
        {
            if (GameManager.Instance != null) GameManager.Instance.GameOver();
        }
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
        UpdateUI();

        // ⚡ 통합 버프 사운드 재생
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.PlayerBuff);
    }

    public void IncreaseMaxHP(int amount)
    {
        maxHP += amount;
        currentHP += amount;
        UpdateUI();

        // ⚡ 통합 버프 사운드 재생
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.PlayerBuff);
    }

    // 🔥 매개변수(int damage)를 삭제하여 깔끔하게 통일했습니다.
    public void UpdateUI()
    {
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.UpdateHPBar(currentHP, maxHP); 
        }
    }
}