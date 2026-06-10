using System.Collections.Generic;
using UnityEngine;

public class SelectionSortSkill : MonoBehaviour
{
    [Header("Skill Level Settings")]
    [SerializeField] private int currentLevel = 1; // 현재 스킬 레벨
    [SerializeField] private int maxLevel = 5;     // 스킬의 최대 레벨 (예시: 5레벨 만렙)
    public bool IsMaxLevel => currentLevel >= maxLevel;
    public int CurrentLevel => currentLevel;
    public int MaxLevel => maxLevel;

    private List<int> selectedIndices = new List<int>();
    private BattleManager battleManager;
    private ArrayVisualizer visualizer;

    private void Start()
    {
        battleManager = FindFirstObjectByType<BattleManager>();
        visualizer = FindFirstObjectByType<ArrayVisualizer>();
    }

    public void ActivateSkill()
    {
        selectedIndices.Clear();
        
        // 스킬을 새로 누를 때마다 혹시 남아있을지 모르는 색상을 초기화
        if (visualizer != null) visualizer.ResetAllBlocksColor();
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowWarning("선택 정렬 발동!\n두 블록을 선택하세요.", 1.0f);
        }
    }

    public void OnBlockClicked(int blockIndex)
    {
        if (selectedIndices.Count >= 2 || selectedIndices.Contains(blockIndex)) return;

        selectedIndices.Add(blockIndex);

        if (selectedIndices.Count == 1)
        {
            if (visualizer != null) visualizer.HighlightBlock(blockIndex, Color.yellow);
        }
        else if (selectedIndices.Count == 2)
        {
            ExecuteSwap();
        }
    }

    private void ExecuteSwap()
    {
        int indexA = selectedIndices[0];
        int indexB = selectedIndices[1];

        if (visualizer != null) visualizer.ResetAllBlocksColor();

        if (battleManager != null)
        {
            battleManager.SwapBlocks(indexA, indexB);
            battleManager.ChangeState(BattleState.EnemyTurn);
        }

        selectedIndices.Clear();
    }
    
    public void UpgradeSkill()
    {
        if (IsMaxLevel)
        {
            return;
        }

        currentLevel++;
        // 쿨타임 감소, 위력 증가 등 로직 추가
    }
}