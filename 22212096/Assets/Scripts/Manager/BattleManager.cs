using UnityEngine;
using System.Collections;

public enum BattleState { Initializing, PlayerTurn, EnemyTurn, EvaluateVictory }

public class BattleManager : MonoBehaviour
{
    [Header("Enemy Gimmick Probabilities")]
    [Range(0f, 1f)] [SerializeField] private float shuffleProbability = 0.2f; 
    [Range(0f, 1f)] [SerializeField] private float lockProbability = 0.3f;    
    
    public bool IsInputBlocked { get; private set; } = false;
    private BattleState currentState;
    public EnemyData currentEnemy;
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
                isPlayerTurn = true; 
                StartCoroutine(PlayerTurnStartRoutine());
                break;
            case BattleState.EnemyTurn:
                isPlayerTurn = false; 
                StartCoroutine(EnemyTurnDelayRoutine());
                break;
            case BattleState.EvaluateVictory:
                CheckBattleResult();
                break;
        }
    }

    private IEnumerator PlayerTurnStartRoutine()
    {
        IsInputBlocked = true; 
        yield return new WaitForSeconds(0.6f); 
        IsInputBlocked = false; 
    }

    private IEnumerator EnemyTurnDelayRoutine()
    {
        yield return new WaitForSeconds(0.7f);
        ExecuteEnemyPattern();
    }

    private void SetupBattle()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayBGM(BGMType.NormalBattle);

        int currentStage = GameManager.Instance.CurrentStage;
        int calculatedSize = Mathf.Min(8, 5 + (currentStage / 3));

        currentEnemy = new EnemyData(calculatedSize);
        
        ArrayVisualizer visualizer = FindFirstObjectByType<ArrayVisualizer>();
        if(visualizer != null) visualizer.RenderBlocks(currentEnemy.GetArray());
        
        ChangeState(BattleState.PlayerTurn);
    }

    private void ExecuteEnemyPattern()
    {
        LockedBlockIndex = -1; 
        DataStructureSkill stackShield = FindFirstObjectByType<DataStructureSkill>();
        QueueShieldSkill queueShield = FindFirstObjectByType<QueueShieldSkill>();
        PlayerController player = FindFirstObjectByType<PlayerController>();

        int baseEnemyDamage = 10; 
        float reductionRate = 0f;

        if (stackShield != null) reductionRate = stackShield.ConsumeShield();
        if (reductionRate <= 0f && queueShield != null) reductionRate = queueShield.ConsumeShield();

        int finalDamage = Mathf.RoundToInt(baseEnemyDamage * (1f - reductionRate));
        if (player != null) player.TakeDamage(finalDamage);

        int currentStage = GameManager.Instance.CurrentStage;
        if (currentStage >= 15)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < lockProbability) ExecuteLockPattern();
            else if (rand < lockProbability + shuffleProbability) ExecuteShufflePattern();
        }
        else if (currentStage >= 5)
        {
            if (Random.Range(0f, 1f) < shuffleProbability) ExecuteShufflePattern();
        }

        ChangeState(BattleState.EvaluateVictory);
    }

    private void ExecuteLockPattern()
    {
        int[] data = currentEnemy.GetArray();
        if (data.Length > 0)
        {
            LockedBlockIndex = Random.Range(0, data.Length);
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.EnemyGimmick);
            
            UIManager uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null) uiManager.ShowWarning("블록 빙결!\n1턴간 조작할 수 없습니다!", 1.5f);

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
            while (indexA == indexB) indexB = Random.Range(0, data.Length);

            int temp = data[indexA];
            data[indexA] = data[indexB];
            data[indexB] = temp;

            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.EnemyGimmick);
            UIManager uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null) uiManager.ShowWarning("적의 방해 공작!\n배열이 뒤섞입니다!", 1.5f);

            ArrayVisualizer visualizer = FindFirstObjectByType<ArrayVisualizer>();
            if (visualizer != null) visualizer.RenderBlocks(data);
        }
    }

    private void CheckBattleResult()
    {
        if (currentEnemy.IsSorted()) GameManager.Instance.OnStageCleared();
        else
        {
            if (isPlayerTurn) ChangeState(BattleState.EnemyTurn);
            else ChangeState(BattleState.PlayerTurn);
        }
    }

    public void SwapBlocks(int indexA, int indexB)
    {
        int[] data = currentEnemy.GetArray();
        int temp = data[indexA];
        data[indexA] = data[indexB];
        data[indexB] = temp;

        ArrayVisualizer visualizer = FindFirstObjectByType<ArrayVisualizer>();
        if (visualizer != null) visualizer.RenderBlocks(data);
    }
}