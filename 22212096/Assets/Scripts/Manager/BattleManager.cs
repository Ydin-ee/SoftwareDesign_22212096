using TMPro;
using UnityEngine;

public enum BattleState
{
    Initializing,
    PlayerTurn,
    EnemyTurn,
    EvaluateVictory
}

public class BattleManager : MonoBehaviour
{
    [Header("Enemy Gimmick Probabilities")]
    [Range(0f, 1f)]
    [SerializeField] private float shuffleProbability = 0.2f; // 섞기 확률

    [Range(0f, 1f)]
    [SerializeField] private float lockProbability = 0.3f;    // 잠금(빙결) 확률
    
    public bool IsInputBlocked { get; private set; } = false;

    private BattleState currentState;
    public EnemyData currentEnemy;
    
    // 🔥 턴 교대를 추적할 플래그 변수
    private bool isPlayerTurn = true;

    public int LockedBlockIndex { get; private set; } = -1;

    public void ChangeState(BattleState newState)
    {
        IsInputBlocked = true;
        currentState = newState;
        StopAllCoroutines();

        switch (currentState)
        {
            case BattleState.Initializing:
                SetupBattle();
                break;

            case BattleState.PlayerTurn:
                // 🔥 플레이어 턴으로 진입 시 플래그를 true로 맞춥니다.
                isPlayerTurn = true; 
                StartCoroutine(PlayerTurnStartRoutine());
                break;

            case BattleState.EnemyTurn:
                // 🔥 적 턴으로 진입 시 플래그를 false로 명확히 바꿉니다.
                isPlayerTurn = false; 
                StartCoroutine(EnemyTurnDelayRoutine());
                break;

            case BattleState.EvaluateVictory:
                CheckBattleResult();
                break;
        }
    }

    private System.Collections.IEnumerator PlayerTurnStartRoutine()
    {
        IsInputBlocked = true; 
        yield return new WaitForSeconds(0.6f); 
        IsInputBlocked = false; 
        Debug.Log("플레이어 입력 잠금 해제. 행동을 선택하세요.");
    }

    private System.Collections.IEnumerator EnemyTurnDelayRoutine()
    {
        yield return new WaitForSeconds(0.7f);
        ExecuteEnemyPattern();
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
        // 🔥 [버그 픽스] 전투 세팅 시점에 기본 배틀 BGM을 확실하게 다시 틀어줍니다.
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(BGMType.NormalBattle);
        }

        int currentStage = GameManager.Instance.CurrentStage;
        int calculatedSize = 5 + (currentStage / 3);

        if (calculatedSize > 8) calculatedSize = 8;

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
        LockedBlockIndex = -1; 

        // 🔥 스택과 큐를 모두 찾습니다.
        DataStructureSkill stackShield = FindFirstObjectByType<DataStructureSkill>();
        QueueShieldSkill queueShield = FindFirstObjectByType<QueueShieldSkill>();
        PlayerController player = FindFirstObjectByType<PlayerController>();

        int baseEnemyDamage = 10; 
        float reductionRate = 0f;

        // 1. 스택 쉴드가 켜져 있다면 먼저 방어율을 가져옵니다.
        if (stackShield != null)
        {
            reductionRate = stackShield.ConsumeShield();
        }
        
        // 2. 스택 쉴드가 없거나 소모되었다면, 큐 쉴드가 있는지 확인합니다.
        if (reductionRate <= 0f && queueShield != null)
        {
            reductionRate = queueShield.ConsumeShield();
        }

        int finalDamage = Mathf.RoundToInt(baseEnemyDamage * (1f - reductionRate));

        if (reductionRate > 0)
        {
            Debug.Log($"[방어 성공] 적의 데미지가 {reductionRate * 100}% 감소하여 {finalDamage}의 피해만 입습니다!");
        }

        if (player != null)
        {
            player.TakeDamage(finalDamage);
        }

        int currentStage = GameManager.Instance.CurrentStage;
        if (currentStage >= 15)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < lockProbability) 
            {
                ExecuteLockPattern();
            }
            else if (rand < lockProbability + shuffleProbability) 
            {
                ExecuteShufflePattern();
            }
        }
        else if (currentStage >= 5)
        {
            if (Random.Range(0f, 1f) < shuffleProbability)
            {
                ExecuteShufflePattern();
            }
        }

        // 공격 및 기믹 적용이 완전히 끝난 후 판정 스테이트로 이동
        ChangeState(BattleState.EvaluateVictory);
    }

    private void ExecuteLockPattern()
    {
        int[] data = currentEnemy.GetArray();
        if (data.Length > 0)
        {
            LockedBlockIndex = Random.Range(0, data.Length);
            Debug.Log($"[적 방해 패턴 발동!] {LockedBlockIndex}번 블록이 얼어붙었습니다!");
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.EnemyGimmick);
            
            UIManager uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowWarning("블록 빙결!\n1턴간 해당 블록을 조작할 수 없습니다!", 1.5f);
            }

            ArrayVisualizer visualizer = FindFirstObjectByType<ArrayVisualizer>();
            if (visualizer != null)
            {
                visualizer.RenderBlocks(data);
                visualizer.HighlightBlock(LockedBlockIndex, Color.gray);
            }
        }
    }

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
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.EnemyGimmick);
            
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
            GameManager.Instance.OnStageCleared();
        }
        else
        {
            // 🔥 버그 수정 완료: 방금 행동을 마친 주체(isPlayerTurn)의 '반대' 주체에게 턴을 넘겨줍니다.
            if (isPlayerTurn)
            {
                // 플레이어가 정렬을 시도했으나 미완성인 경우 ➔ 적 턴으로 전환
                ChangeState(BattleState.EnemyTurn);
            }
            else
            {
                // 적이 공격 패턴을 모두 마친 경우 ➔ 다시 플레이어 턴으로 안전하게 환원
                ChangeState(BattleState.PlayerTurn);
            }
        }
    }

    public void SwapBlocks(int indexA, int indexB)
    {
        int[] data = currentEnemy.GetArray();
        int temp = data[indexA];
        data[indexA] = data[indexB];
        data[indexB] = temp;

        Debug.Log($"[BattleManager] 데이터 스왑 완료: {data[indexA]} <-> {data[indexB]}");

        ArrayVisualizer visualizer = FindFirstObjectByType<ArrayVisualizer>();
        if (visualizer != null)
        {
            visualizer.RenderBlocks(data);
        }
    }
}