using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    
    // 외부에서 최대 체력을 읽을 수 있도록 허용
    public int MaxHP { get { return maxHP; } }

    private int currentHP;

    private void Start()
    {
        currentHP = maxHP;
        UpdateUI(); 
    }
// 데미지 감소량을 임시로 기억할 변수
    private int damageReduction = 0;

    // 버블 정렬 스킬 발동 시 호출될 데미지 감소 함수
    public void AddDamageReduction(int amount)
    {
        damageReduction += amount;
        Debug.Log($"[버블 정렬 진화 효과] 다음 적의 공격 데미지가 {amount} 감소합니다!");
    }

    // 기존의 TakeDamage 함수를 아래와 같이 수정합니다.
    public void TakeDamage(int damage)
    {
        // 1. 방패 스킬과 별개로 데미지 감소 수치를 먼저 계산합니다.
        int finalDamage = damage - damageReduction;
        if (finalDamage < 0) finalDamage = 0; // 데미지가 마이너스가 되지 않도록 방지
        
        currentHP -= finalDamage;
        if (currentHP < 0) currentHP = 0;
        
        Debug.Log($"적의 공격! (원래 데미지: {damage} ➔ 감소된 데미지: {finalDamage}) 남은 체력: {currentHP}/{maxHP}");
        
        // 2. 데미지 감소 효과는 1회용이므로 맞고 나면 다시 0으로 초기화합니다.
        damageReduction = 0; 
        
        UpdateUI(finalDamage);

        if (currentHP <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
        Debug.Log($"체력을 {amount} 회복했습니다. 현재 체력: {currentHP}/{maxHP}");
        UpdateUI();
    }

    // 🔥 에러의 원인이었던 새로운 함수 추가 부분!
    public void IncreaseMaxHP(int amount)
    {
        maxHP += amount;
        currentHP += amount;
        Debug.Log($"최대 체력이 {amount} 증가했습니다. 현재 체력: {currentHP}/{maxHP}");
        UpdateUI();
    }

    private void UpdateUI(int lastDamage = 0)
    {
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.UpdateHPBar(currentHP, maxHP);
            
            if (lastDamage > 0)
            {
                uiManager.SpawnDamageText(lastDamage);
            }
        }
    }
    
}