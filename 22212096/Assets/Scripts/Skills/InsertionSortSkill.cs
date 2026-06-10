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
        Debug.Log("삽입 정렬 대기 중: 쏙 뽑아낼 블록을 먼저 선택하세요.");

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
            Debug.LogWarning("얼어붙은 블록은 조작할 수 없습니다!");
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.EnemyGimmick);
            return;
        }

        // 일반 클릭음 재생
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        if (selectedSourceIndex == -1)
        {
            // [1단계] 이동할 원본 블록 선택
            selectedSourceIndex = blockIndex;
            
            // 선택 정렬(노란색)과 구분되도록 삽입 정렬은 초록색으로 하이라이트합니다.
            if (visualizer != null) visualizer.HighlightBlock(blockIndex, Color.green); 
            Debug.Log($"[{blockIndex}]번 블록 선택됨. 끼워 넣을 위치를 클릭하세요.");
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
        int temp = data[sourceIndex]; // 이동할 값을 미리 빼둡니다.

        // 🔥 삽입 정렬의 핵심: 원소들 밀어내기 (Shift)
        if (sourceIndex < destIndex)
        {
            // 오른쪽으로 끼워 넣을 때: 사이의 값들을 왼쪽으로 한 칸씩 당김
            for (int i = sourceIndex; i < destIndex; i++)
            {
                data[i] = data[i + 1];
            }
        }
        else
        {
            // 왼쪽으로 끼워 넣을 때: 사이의 값들을 오른쪽으로 한 칸씩 밈
            for (int i = sourceIndex; i > destIndex; i--)
            {
                data[i] = data[i - 1];
            }
        }
        
        // 비어있는 목표 위치에 값 꽂아 넣기
        data[destIndex] = temp;

        // 화면 갱신
        if (visualizer != null)
        {
            visualizer.RenderBlocks(data);
            visualizer.ResetAllBlocksColor();
        }

        Debug.Log($"[{temp}] 값을 {destIndex}번 인덱스에 삽입 완료!");

        // 사용 완료 후 스킬 종료 및 턴 넘기기
        isActive = false;
        selectedSourceIndex = -1;
        battleManager.ChangeState(BattleState.EnemyTurn);
    }

    private void CancelSkill()
    {
        Debug.Log("제자리를 클릭하여 삽입 정렬이 취소되었습니다.");
        isActive = false;
        selectedSourceIndex = -1;
        if (visualizer != null) visualizer.ResetAllBlocksColor();
    }
}