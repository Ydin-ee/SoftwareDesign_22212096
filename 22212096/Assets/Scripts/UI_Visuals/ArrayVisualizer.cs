using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스

public class ArrayVisualizer : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject blockPrefab; // 방금 만든 NumberBlock 프리팹을 넣을 칸
    [SerializeField] private Transform blockParent;  // Horizontal Layout Group이 있는 BlockPanel을 넣을 칸

    // 생성된 블록들을 관리하기 위한 리스트
    private List<GameObject> activeBlocks = new List<GameObject>();

    public void RenderBlocks(int[] data)
    {
        Debug.Log("초기화: 화면에 숫자 블록들을 배치합니다.");

        // 1. 기존에 남아있는 블록이 있다면 싹 지워서 초기화합니다.
        foreach (GameObject block in activeBlocks)
        {
            Destroy(block);
        }
        activeBlocks.Clear();

        // 2. 전달받은 배열(EnemyData)의 길이만큼 새로운 블록을 생성합니다.
        for (int i = 0; i < data.Length; i++)
        {
            // 부모(blockParent) 아래에 프리팹 생성
            GameObject newBlock = Instantiate(blockPrefab, blockParent);
            
            // 프리팹 자식에 있는 TextMeshPro 컴포넌트를 찾아 숫자를 덮어씌웁니다.
            TextMeshProUGUI textComp = newBlock.GetComponentInChildren<TextMeshProUGUI>();
            if (textComp != null)
            {
                textComp.text = data[i].ToString();
            }

            // 리스트에 추가하여 추후 스왑 애니메이션 등에서 추적할 수 있게 합니다.
            activeBlocks.Add(newBlock);
        }
    }

    public IEnumerator PlaySwapAnimation(int indexA, int indexB)
    {
        Debug.Log($"[연출 시작] {indexA}번 블록과 {indexB}번 블록의 위치를 바꿉니다.");
        
        // TODO: activeBlocks[indexA]와 activeBlocks[indexB]의 실제 Transform 이동 로직
        yield return new WaitForSeconds(1.0f);

        Debug.Log("[연출 완료] 위치 변경 애니메이션이 끝났습니다.");
    }
}