using System.Collections.Generic;
using UnityEngine;

public class SelectionSortSkill : MonoBehaviour
{
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
        
        // 스킬을 새로 누를 때마다 혹시 남아있을지 모르는 색상을 초기화합니다.
        if (visualizer != null) visualizer.ResetAllBlocksColor();
        
        Debug.Log("[선택 정렬 발동] 위치를 바꿀 블록 2개를 순서대로 클릭해 주세요!");
    }

    // SelectionSortSkill.cs 의 OnBlockClicked 함수 수정
    public void OnBlockClicked(int blockIndex)
    {
        // 🔥 만약 클릭한 블록이 현재 잠겨있는 블록이라면 클릭을 무시합니다.
        if (battleManager != null && battleManager.LockedBlockIndex == blockIndex)
        {
            Debug.LogWarning("얼어붙은 블록은 선택할 수 없습니다!");
            // UI 매니저를 통해 "잠긴 블록입니다!" 같은 경고를 띄워주면 더 좋습니다.
            return; 
        }

        if (selectedIndices.Count >= 2 || selectedIndices.Contains(blockIndex)) return;

        selectedIndices.Add(blockIndex);
        Debug.Log($"[{selectedIndices.Count}/2] {blockIndex}번 블록 선택됨");

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

        Debug.Log($"[선택 정렬 실행] {indexA}번과 {indexB}번 블록의 위치를 바꿉니다!");

        // 스왑을 실행하기 전에 색상을 원래대로 되돌립니다.
        if (visualizer != null) visualizer.ResetAllBlocksColor();

        if (battleManager != null)
        {
            // 🔥 기존에 만들어두신 BattleManager의 스왑 로직을 호출합니다.
            // (함수 이름이 SwapBlocks가 아니라면, 버블 정렬에서 쓰시던 실제 스왑 함수명으로 바꿔주세요!)
            battleManager.SwapBlocks(indexA, indexB);
            
            // 행동을 마쳤으므로 적의 턴으로 넘깁니다.
            battleManager.ChangeState(BattleState.EnemyTurn);
        }

        selectedIndices.Clear();
    }
}