using TMPro;
using UnityEngine;
using UnityEngine.XR;

public enum BattleState
{
    Initializing,
    PlayerTurn,
    EnemyTurn,
    EvaluateVictory
}
public class BattleManager : MonoBehaviour
{
    private BattleState currentState;
    private EnemyData currentEnemy;
    private bool isPlayerTurn = true;

    public void ChangeState(BattleState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case BattleState.Initializing:
                SetupBattle();
                break;
            case BattleState.PlayerTurn:
                isPlayerTurn = true;
                Debug.Log("플레이어의 턴입니다. 스킬을 선택하세요.");
                break;
            case BattleState.EnemyTurn:
                isPlayerTurn = false;
                ExecuteEnemyPattern();
                break;
            case BattleState.EvaluateVictory:
                CheckBattleResult();
                break;
        }
    }
    public void OnPlayerSkillSelected(SkillBase selectedSkill)
    {
        if(currentState != BattleState.PlayerTurn)
        {
            Debug.LogWarning("지금은 스킬을 사용할 수 없는 상태입니다!");
            return;
        }
        selectedSkill.Execute(ref currentEnemy);
    }

    private void SetupBattle()
    {
        currentEnemy = new EnemyData(5);
        Debug.Log("전투 세팅 완료");
        ArrayVisualizer visualizer = FindObjectOfType<ArrayVisualizer>();
        if(visualizer != null)
        {
            visualizer.RenderBlocks(currentEnemy.GetArray());
        }
        ChangeState(BattleState.PlayerTurn);
    }

    private void ExecuteEnemyPattern()
    {
        Debug.Log("적이 공격합니다!");

        // 씬에서 플레이어의 방패 스킬 컴포넌트를 찾습니다.
        DataStructureSkill shieldSkill = FindFirstObjectByType<DataStructureSkill>();
        PlayerController player = FindFirstObjectByType<PlayerController>();

        // 1. 방패 스택이 존재한다면, 스택을 1개 소모하고 데미지를 무효화합니다.
        if (shieldSkill != null && shieldSkill.ConsumeShield())
        {
            Debug.Log("방어 성공! 플레이어가 피해를 입지 않았습니다.");
        }
        // 2. 방패가 없다면 평소처럼 플레이어의 체력을 깎습니다.
        else
        {
            if (player != null)
            {
                player.TakeDamage(10);
            }
        }

        // 공격 및 방어 처리가 끝났으니 승패 판정으로 상태를 넘깁니다.
        ChangeState(BattleState.EvaluateVictory);
    }

    private void CheckBattleResult()
    {
        if (currentEnemy.IsSorted())
        {
            // 🔥 직접 RewardManager를 부르지 않고, GameManager에게 처리를 넘깁니다.
            GameManager.Instance.OnStageCleared();
        }
        else
        {
            if (isPlayerTurn)
            {
                ChangeState(BattleState.EnemyTurn);
            }
            else
            {
                ChangeState(BattleState.PlayerTurn);
            }
        }
    }
}
