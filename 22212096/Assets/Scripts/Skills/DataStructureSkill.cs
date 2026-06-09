using UnityEngine;

public class DataStructureSkill : MonoBehaviour
{
    private int currentStacks = 0;

    // 플레이어가 방패 스킬을 사용했을 때(턴 소모 시) 호출되는 함수
    public void ExecuteShieldSkill()
    {
        // 1. 플레이어 몸체에 붙어있는 스킬 매니저를 찾습니다.
        PlayerSkillManager skillManager = GetComponent<PlayerSkillManager>();
        
        // 2. "StackShield" 스킬의 현재 레벨을 가져옵니다. (기본값 1)
        int level = 1;
        if (skillManager != null)
        {
            level = skillManager.GetSkillLevel("StackShield");
        }

        // 3. 레벨만큼 방패 스택을 한 번에 추가합니다!
        currentStacks += level; 
        
        Debug.Log($"[방패 스킬 발동] LV.{level} 적용: 방패가 {level}겹 추가되었습니다! (현재 총 {currentStacks}겹)");
    }

    // 적이 공격할 때 BattleManager가 호출하는 방어 판정 함수
    public bool ConsumeShield()
    {
        if (currentStacks > 0)
        {
            currentStacks--;
            Debug.Log($"[방어 성공] 방패가 적의 공격을 막고 1겹 깨졌습니다. (남은 방패: {currentStacks}겹)");
            return true;
        }
        return false;
    }
}