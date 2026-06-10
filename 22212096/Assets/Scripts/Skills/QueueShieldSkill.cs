using UnityEngine;
using System.Collections.Generic;

public class QueueShieldSkill : MonoBehaviour
{
    // C#의 Queue 자료구조를 사용하여 방패를 관리
    private Queue<float> shieldQueue = new Queue<float>();

    public void ExecuteShieldSkill()
    {
        PlayerSkillManager skillManager = FindFirstObjectByType<PlayerSkillManager>();
        int level = (skillManager != null) ? skillManager.GetSkillLevel("QueueShield") : 1;
        
        if (level > 3) level = 3;

        shieldQueue.Clear(); // 새로운 턴에 사용 시 기존 큐 초기화

        // 큐(FIFO) 특성: 먼저 들어간 것이 먼저 나옴
        if (level >= 1) shieldQueue.Enqueue(0.3f); // 1타: 30% 방어
        if (level >= 2) shieldQueue.Enqueue(0.5f); // 2타: 50% 방어
        if (level >= 3) shieldQueue.Enqueue(0.7f); // 3타: 70% 방어
        
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
            float reductionRate = shieldQueue.Dequeue();
            return reductionRate;
        }
        
        return 0f; // 큐가 비어있으면 방어 불가
    }
}