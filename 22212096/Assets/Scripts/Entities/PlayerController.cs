using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    [Header("Effects")]
    [SerializeField] private GameObject damageTextPrefab; // 띄울 데미지 텍스트 프리팹
    [SerializeField] private Transform damageTextSpawnPoint; // 텍스트가 생성될 위치 (주로 HP바 근처)
    
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
        int finalDamage = damage - damageReduction;
        if (finalDamage < 0) finalDamage = 0; 
        
        currentHP -= finalDamage;
        if (currentHP < 0) currentHP = 0;
        
        damageReduction = 0; 
        UpdateUI(finalDamage);

        if (finalDamage > 0)
        {
            CameraManager.Instance.ShakeCamera(0.2f, 0.4f);

            // 🔥 오리지널 DamageText의 Setup 함수를 호출하는 로직
            if (damageTextPrefab != null && damageTextSpawnPoint != null)
            {
                // 1. 프리팹 생성
                GameObject dmgTextObj = Instantiate(damageTextPrefab, damageTextSpawnPoint.position, Quaternion.identity, damageTextSpawnPoint.parent);
                
                // 2. 생성된 오브젝트에서 DamageText 스크립트 컴포넌트를 추출합니다.
                DamageText dmgTextScript = dmgTextObj.GetComponent<DamageText>();
                
                if (dmgTextScript != null)
                {
                    // 3. 가지고 계시던 고유의 Setup 함수에 최종 데미지를 넘겨 연출을 시작합니다!
                    dmgTextScript.Setup(finalDamage);
                }
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