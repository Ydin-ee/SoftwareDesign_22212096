using System.Collections.Generic;
using UnityEngine;
public class InsertionSortSkill : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private ArrayVisualizer visualizer;

    private bool isActive = false;
    private int selectedSourceIndex = -1;

    public void ActivateSkill()
    {
        isActive = true;
        selectedSourceIndex = -1;

        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowWarning("삽입 정렬 발동!\n이동할 블록을 선택하세요.", 1.0f);
        }
    }

    // 블록을 클릭했을 때 호출될 함수
    public void OnBlockClicked(int blockIndex)
    {
        if (!isActive) return;

        // 얼어붙은 블록(잠금 기믹) 클릭 방지
        if (battleManager != null && battleManager.LockedBlockIndex == blockIndex)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.EnemyGimmick);
            return;
        }

        // 일반 클릭음 재생
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        if (selectedSourceIndex == -1)
        {
            // [1단계] 이동할 원본 블록 선택
            selectedSourceIndex = blockIndex;
            
            // 선택 정렬(노란색)과 구분되도록 삽입 정렬은 초록색으로 하이라이트
            if (visualizer != null) visualizer.HighlightBlock(blockIndex, Color.green);
        }
        else
        {
            // [2단계] 목표 위치 선택 및 끼워 넣기 실행
            if (selectedSourceIndex == blockIndex)
            {
                // 제자리를 다시 클릭하면 스킬 취소
                CancelSkill();
                return;
            }

            ExecuteInsertion(selectedSourceIndex, blockIndex);
        }
    }

    private void ExecuteInsertion(int sourceIndex, int destIndex)
    {
        int[] data = battleManager.currentEnemy.GetArray();
        int temp = data[sourceIndex]; 

        // 블록이 이동한 거리(밀어낸 블록의 수)를 계산
        int shiftedDistance = Mathf.Abs(sourceIndex - destIndex);

        if (sourceIndex < destIndex)
        {
            for (int i = sourceIndex; i < destIndex; i++)
            {
                data[i] = data[i + 1];
            }
        }
        else
        {
            for (int i = sourceIndex; i > destIndex; i--)
            {
                data[i] = data[i - 1];
            }
        }
        
        data[destIndex] = temp;

        if (visualizer != null)
        {
            visualizer.RenderBlocks(data);
            visualizer.ResetAllBlocksColor();
        }

        Debug.Log($"[{temp}] 값을 {destIndex}번 인덱스에 삽입 완료!");

        // 이동한 거리가 1 이상일 경우 (제자리 이동이 아닐 경우) 회복 발동
        if (shiftedDistance > 0)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                // 밀어낸 칸 수 * 4 만큼의 체력을 회복. (예: 3칸 이동 시 12 회복)
                int healAmount = shiftedDistance * 4;
                player.Heal(healAmount);
                
                UIManager uiManager = FindFirstObjectByType<UIManager>();
                if (uiManager != null)
                {
                    uiManager.ShowWarning($"연쇄 이동 보너스!\n체력 {healAmount} 회복!", 1.5f);
                }
            }
        }

        isActive = false;
        selectedSourceIndex = -1;
        battleManager.ChangeState(BattleState.EnemyTurn);
    }

    private void CancelSkill()
    {
        isActive = false;
        selectedSourceIndex = -1;
        if (visualizer != null) visualizer.ResetAllBlocksColor();
    }
}