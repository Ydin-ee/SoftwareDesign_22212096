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

    public int LockedBlockIndex { get; private set; } = -1;

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
        // 1. GameManager에서 현재 스테이지 번호를 가져옵니다.
        int currentStage = GameManager.Instance.CurrentStage;

        // 2. 난이도 공식 적용 (예: 기본 5칸에서 시작하여, 3스테이지마다 1칸씩 증가)
        int calculatedSize = 5 + (currentStage / 3);

        // 3. UI 화면(가로 길이)을 벗어나지 않도록 최대 칸수를 제한합니다. (예: 최대 8칸)
        if (calculatedSize > 8) calculatedSize = 8;

        // 4. 계산된 크기로 새로운 적(난수 배열)을 생성합니다.
        currentEnemy = new EnemyData(calculatedSize);
        Debug.Log($"전투 세팅 완료: {currentStage}스테이지, 배열 크기 {calculatedSize}칸 등장!");
        
        ArrayVisualizer visualizer = FindFirstObjectByType<ArrayVisualizer>();
        if(visualizer != null)
        {
            visualizer.RenderBlocks(currentEnemy.GetArray());
        }
        
        ChangeState(BattleState.PlayerTurn);
    }

    private void ExecuteEnemyPattern()
    {
        Debug.Log("적이 공격합니다!");

        // 매 턴이 시작될 때 이전 턴의 잠금을 해제합니다.
        LockedBlockIndex = -1; 

        DataStructureSkill shieldSkill = FindFirstObjectByType<DataStructureSkill>();
        PlayerController player = FindFirstObjectByType<PlayerController>();

        if (shieldSkill != null && shieldSkill.ConsumeShield())
        {
            Debug.Log("방어 성공! 플레이어가 피해를 입지 않았습니다.");
        }
        else if (player != null)
        {
            player.TakeDamage(10);
        }

        // 🔥 적 방해 패턴 고도화 (층수에 따른 기믹 진화)
        int currentStage = GameManager.Instance.CurrentStage;
        if (currentStage >= 15)
        {
            // 15층 이상: 30% 확률로 빙결, 30% 확률로 셔플
            float rand = Random.Range(0f, 1f);
            if (rand < 0.6f) ExecuteLockPattern();
            else if (rand < 0.4f) ExecuteShufflePattern();
        }
        else if (currentStage >= 5)
        {
            // 5층~14층: 30% 확률로 셔플만
            if (currentStage % 5 == 0 || Random.Range(0f, 1f) < 0.3f)
            {
                ExecuteShufflePattern();
            }
        }

        ChangeState(BattleState.EvaluateVictory);
    }

    private void ExecuteLockPattern()
    {
        int[] data = currentEnemy.GetArray();
        if (data.Length > 0)
        {
            // 무작위 블록 하나를 골라 잠가버립니다.
            LockedBlockIndex = Random.Range(0, data.Length);
            Debug.Log($"[적 방해 패턴 발동!] {LockedBlockIndex}번 블록이 얼어붙었습니다!");

            UIManager uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowWarning("블록 빙결!\n1턴간 해당 블록을 조작할 수 없습니다!", 1.5f);
            }

            ArrayVisualizer visualizer = FindFirstObjectByType<ArrayVisualizer>();
            if (visualizer != null)
            {
                // 화면을 갱신하고, 잠긴 블록을 회색으로 칠합니다.
                visualizer.RenderBlocks(data);
                visualizer.HighlightBlock(LockedBlockIndex, Color.gray);
            }
        }
    }

    // 무작위 두 블록의 위치를 강제로 바꾸는 함수
    private void ExecuteShufflePattern()
    {
        int[] data = currentEnemy.GetArray();
        
        if (data.Length >= 2)
        {
            int indexA = Random.Range(0, data.Length);
            int indexB = Random.Range(0, data.Length);
            
            while (indexA == indexB)
            {
                indexB = Random.Range(0, data.Length);
            }

            int temp = data[indexA];
            data[indexA] = data[indexB];
            data[indexB] = temp;

            Debug.Log($"[적 방해 패턴 발동!] 적이 {indexA}번과 {indexB}번 블록을 섞어버렸습니다!");

            // 🔥 추가된 부분: UIManager를 찾아 경고 텍스트를 띄웁니다.
            UIManager uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowWarning("적의 방해 공작!\n배열이 뒤섞입니다!", 1.5f);
            }

            ArrayVisualizer visualizer = FindFirstObjectByType<ArrayVisualizer>();
            if (visualizer != null)
            {
                visualizer.RenderBlocks(data);
            }
        }
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

    // 선택 정렬 스킬 등에서 특정 두 블록의 위치를 바꿀 때 호출하는 함수입니다.
    public void SwapBlocks(int indexA, int indexB)
    {
        // 1. 내부 데이터 배열의 위치를 바꿉니다.
        int[] data = currentEnemy.GetArray();
        int temp = data[indexA];
        data[indexA] = data[indexB];
        data[indexB] = temp;

        Debug.Log($"[BattleManager] 데이터 스왑 완료: {data[indexA]} <-> {data[indexB]}");

        // 2. 바뀐 데이터를 바탕으로 화면의 블록 UI를 즉시 갱신합니다.
        ArrayVisualizer visualizer = FindFirstObjectByType<ArrayVisualizer>();
        if (visualizer != null)
        {
            visualizer.RenderBlocks(data);
        }
    }
}
