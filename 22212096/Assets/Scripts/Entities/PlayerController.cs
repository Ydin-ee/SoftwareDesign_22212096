using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    [Header("Effects")]
    [SerializeField] private GameObject damageTextPrefab; 
    [SerializeField] private Transform damageTextSpawnPoint; 
    
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
        
        // 🔥 매개변수 없이 호출하도록 수정되었습니다.
        UpdateUI(); 

        if (finalDamage > 0)
        {
            CameraManager.Instance.ShakeCamera(0.2f, 0.4f);

            if (damageTextPrefab != null && damageTextSpawnPoint != null)
            {
                GameObject dmgTextObj = Instantiate(damageTextPrefab, damageTextSpawnPoint.position, Quaternion.identity, damageTextSpawnPoint.parent);
                DamageText dmgTextScript = dmgTextObj.GetComponent<DamageText>();
                if (dmgTextScript != null)
                {
                    dmgTextScript.Setup(finalDamage);
                }
            }
        }

        if (currentHP <= 0)
        {
            Debug.Log("🚨 [1단계] 플레이어 체력 0 도달! GameManager 호출 시도");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
            else
            {
                Debug.LogError("🚨 GameManager를 찾을 수 없습니다! 하이어라키에 GameManager 스크립트가 붙어있나요?");
            }
        }
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
        Debug.Log($"체력을 {amount} 회복했습니다. 현재 체력: {currentHP}/{maxHP}");
        UpdateUI();
    }

    public void IncreaseMaxHP(int amount)
    {
        maxHP += amount;
        currentHP += amount;
        Debug.Log($"최대 체력이 {amount} 증가했습니다. 현재 체력: {currentHP}/{maxHP}");
        UpdateUI();
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