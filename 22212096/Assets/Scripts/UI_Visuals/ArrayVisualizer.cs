using UnityEngine;
using UnityEngine.UI; // Button 컴포넌트를 사용하기 위해 추가
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ArrayVisualizer : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Transform blockParent;

    private List<GameObject> activeBlocks = new List<GameObject>();

    public void RenderBlocks(int[] data)
    {
        Debug.Log("초기화: 화면에 숫자 블록들을 배치합니다.");

        // 1. 기존 블록 초기화
        foreach (GameObject block in activeBlocks)
        {
            Destroy(block);
        }
        activeBlocks.Clear();

        // 2. 새로운 블록 생성 및 세팅
        for (int i = 0; i < data.Length; i++)
        {
            GameObject newBlock = Instantiate(blockPrefab, blockParent);
            
            // 텍스트 세팅
            TextMeshProUGUI textComp = newBlock.GetComponentInChildren<TextMeshProUGUI>();
            if (textComp != null)
            {
                textComp.text = data[i].ToString();
            }

            // 🔥 추가된 부분: 블록에 클릭 이벤트 연결
            Button blockButton = newBlock.GetComponent<Button>();
            if (blockButton != null)
            {
                int currentIndex = i; // 클로저(Closure) 이슈 방지를 위해 지역 변수에 저장

                blockButton.onClick.RemoveAllListeners();
                blockButton.onClick.AddListener(() => 
                {
                    // 클릭 시 선택 정렬 스킬 스크립트를 찾아 인덱스를 전달합니다.
                    SelectionSortSkill selectionSkill = FindFirstObjectByType<SelectionSortSkill>();
                    if (selectionSkill != null)
                    {
                        selectionSkill.OnBlockClicked(currentIndex);
                    }
                });
            }
            else
            {
                Debug.LogWarning("블록 프리팹에 Button 컴포넌트가 없어서 클릭할 수 없습니다!");
            }

            activeBlocks.Add(newBlock);
        }
    }

    public IEnumerator PlaySwapAnimation(int indexA, int indexB)
    {
        Debug.Log($"[연출 시작] {indexA}번 블록과 {indexB}번 블록의 위치를 바꿉니다.");

        // 1. 유저가 인지할 수 있도록 스왑할 두 블록을 빨간색으로 강조합니다.
        HighlightBlock(indexA, Color.red);
        HighlightBlock(indexB, Color.red);

        // 0.5초 대기하며 빨간색 블록을 보여줍니다.
        yield return new WaitForSeconds(0.5f);

        // 2. 실제 블록의 텍스트(숫자)를 서로 맞바꿔줍니다.
        if (indexA < activeBlocks.Count && indexB < activeBlocks.Count)
        {
            TextMeshProUGUI textA = activeBlocks[indexA].GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI textB = activeBlocks[indexB].GetComponentInChildren<TextMeshProUGUI>();

            if (textA != null && textB != null)
            {
                string tempText = textA.text;
                textA.text = textB.text;
                textB.text = tempText;
            }
        }

        // 3. 스왑이 완료되었으니 색상을 다시 원래대로 복구합니다.
        ResetAllBlocksColor();

        // 0.5초 대기하며 스왑된 결과를 보여줍니다.
        yield return new WaitForSeconds(0.5f);

        Debug.Log("[연출 완료] 위치 변경 애니메이션이 끝났습니다.");
    }

    // 특정 인덱스의 블록 색상을 변경하는 함수
    public void HighlightBlock(int index, Color color)
    {
        if (index >= 0 && index < activeBlocks.Count)
        {
            Image blockImage = activeBlocks[index].GetComponent<Image>();
            if (blockImage != null)
            {
                blockImage.color = color;
            }
        }
    }

    // 전체 블록의 색상을 기본값(흰색)으로 되돌리는 함수
    public void ResetAllBlocksColor()
    {
        // BattleManager에 접근하여 현재 잠긴 블록이 있는지 확인합니다.
        BattleManager battleManager = FindFirstObjectByType<BattleManager>();
        int lockedIndex = (battleManager != null) ? battleManager.LockedBlockIndex : -1;

        for (int i = 0; i < activeBlocks.Count; i++)
        {
            Image blockImage = activeBlocks[i].GetComponent<Image>();
            if (blockImage != null)
            {
                // 잠긴 블록이라면 회색을 유지하고, 아니라면 원래 색(흰색)으로 돌립니다.
                if (i == lockedIndex)
                {
                    blockImage.color = Color.gray;
                }
                else
                {
                    blockImage.color = Color.white; 
                }
            }
        }
    }
}