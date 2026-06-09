using UnityEngine;

public class SelectionSortSkill : MonoBehaviour
{
    public void ActivateSkill()
    {
        // 스킬 버튼을 누르면 즉시 정렬되는 것이 아니라, 
        // "어떤 블록을 바꿀지" 플레이어의 마우스 클릭을 기다리는 상태로 전환해야 합니다.
        Debug.Log("[선택 정렬 발동 대기] 바꿀 블록 2개를 마우스로 클릭해 주세요!");
        
        // TODO: BattleManager의 상태를 'WaitForBlockSelection' 같은 입력 대기 상태로 변경
    }
}