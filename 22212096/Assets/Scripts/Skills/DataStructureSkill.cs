using UnityEngine;
using System.Collections.Generic;
public class DataStructureSkill : MonoBehaviour
{
    // C#의 실제 Stack 자료구조를 사용하여 방패 스택을 관리
    private Stack<float> shieldStack = new Stack<float>();

    public void ExecuteShieldSkill()
    {
        // PlayerSkillManager에서 스택 쉴드의 레벨을 가져옵니다. (없으면 기본 1레벨)
        PlayerSkillManager skillManager = FindFirstObjectByType<PlayerSkillManager>();
        int level = (skillManager != null) ? skillManager.GetSkillLevel("StackShield") : 1;
        
        // 최대 레벨 제한 (최대 3)
        if (level > 3) level = 3;

        shieldStack.Clear();

        // 레벨에 따라 스택을 푸시(Push)
        if (level >= 1) shieldStack.Push(0.3f); // 30% 방어
        if (level >= 2) shieldStack.Push(0.5f); // 50% 방어
        if (level >= 3) shieldStack.Push(0.7f); // 70% 방어
        
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowWarning($"스택 쉴드 활성화! (Lv.{level})", 1.0f);
        }
    }

    // 적이 공격할 때 BattleManager가 호출할 함수
    // 방어율(예: 0.7)을 반환하고, 방패가 없으면 0을 반환
    public float ConsumeShield()
    {
        if (shieldStack.Count > 0)
        {
            // 가장 위에 있는 스택(마지막에 넣은 것) (Pop).
            float reductionRate = shieldStack.Pop();
            return reductionRate;
        }
        
        return 0f; // 스택이 비어있으면 방어 불가
    }
}