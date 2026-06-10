using UnityEngine;
using System.Collections.Generic;

public class QueueShieldSkill : MonoBehaviour
{
    // C#의 Queue 자료구조를 사용하여 방패를 관리합니다.
    private Queue<float> shieldQueue = new Queue<float>();

    public void ExecuteShieldSkill()
    {
        PlayerSkillManager skillManager = FindFirstObjectByType<PlayerSkillManager>();
        int level = (skillManager != null) ? skillManager.GetSkillLevel("QueueShield") : 1;
        
        if (level > 3) level = 3;

        shieldQueue.Clear(); // 새로운 턴에 사용 시 기존 큐 초기화

        // 큐(FIFO) 특성: 먼저 들어간 것이 먼저 나옵니다.
        if (level >= 1) shieldQueue.Enqueue(0.3f); // 1타: 30% 방어
        if (level >= 2) shieldQueue.Enqueue(0.5f); // 2타: 50% 방어
        if (level >= 3) shieldQueue.Enqueue(0.7f); // 3타: 70% 방어

        Debug.Log($"큐 쉴드 전개! (현재 레벨: {level}, 장전된 방패 수: {shieldQueue.Count}개)");
        
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowWarning($"큐 쉴드 활성화! (Lv.{level})", 1.0f);
        }
    }

    public float ConsumeShield()
    {
        if (shieldQueue.Count > 0)
        {
            // 가장 먼저 들어간 스택을 뽑아냅니다 (Dequeue).
            float reductionRate = shieldQueue.Dequeue();
            Debug.Log($"큐 방패 활성화! 데미지 {reductionRate * 100}% 감소. (남은 스택: {shieldQueue.Count}개)");
            return reductionRate;
        }
        
        return 0f; // 큐가 비어있으면 방어 불가
    }
}