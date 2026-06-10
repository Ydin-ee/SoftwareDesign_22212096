using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ArrayVisualizer : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Transform blockParent;
    
    [Header("Slime Sprites")]
    [SerializeField] private Sprite greenSlime; // 1~5층
    [SerializeField] private Sprite blueSlime;  // 6~10층
    [SerializeField] private Sprite redSlime;   // 11층 이상

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

        // 🔥 [추가된 부분] 현재 스테이지를 확인하여 슬라임 색상을 결정합니다.
        int currentStage = GameManager.Instance.CurrentStage;
        Sprite targetSlimeSprite = greenSlime; // 기본은 초록

        if (currentStage >= 6 && currentStage <= 10)
        {
            targetSlimeSprite = blueSlime;
        }
        else if (currentStage >= 11)
        {
            targetSlimeSprite = redSlime;
        }

        // 2. 새로운 블록 생성 및 세팅
        for (int i = 0; i < data.Length; i++)
        {
            GameObject newBlock = Instantiate(blockPrefab, blockParent);
            
            // 🔥 [추가된 부분] 생성된 블록의 Image 컴포넌트에 타겟 슬라임 이미지를 넣습니다.
            Image blockImage = newBlock.GetComponent<Image>();
            if (blockImage != null && targetSlimeSprite != null)
            {
                blockImage.sprite = targetSlimeSprite;
            }

            // 텍스트 세팅
            TextMeshProUGUI textComp = newBlock.GetComponentInChildren<TextMeshProUGUI>();
            if (textComp != null)
            {
                textComp.text = data[i].ToString();
            }

            // 기존 로직 유지: 블록에 클릭 이벤트 연결 (선택 정렬용)
            Button blockButton = newBlock.GetComponent<Button>();
            if (blockButton != null)
            {
                int currentIndex = i; // 클로저(Closure) 이슈 방지

                blockButton.onClick.RemoveAllListeners();
                blockButton.onClick.AddListener(() => 
                {
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

        HighlightBlock(indexA, Color.red);
        HighlightBlock(indexB, Color.red);

        yield return new WaitForSeconds(0.5f);

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

        ResetAllBlocksColor();

        yield return new WaitForSeconds(0.5f);

        Debug.Log("[연출 완료] 위치 변경 애니메이션이 끝났습니다.");
    }

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

    public void ResetAllBlocksColor()
    {
        BattleManager battleManager = FindFirstObjectByType<BattleManager>();
        int lockedIndex = (battleManager != null) ? battleManager.LockedBlockIndex : -1;

        for (int i = 0; i < activeBlocks.Count; i++)
        {
            Image blockImage = activeBlocks[i].GetComponent<Image>();
            if (blockImage != null)
            {
                // 🔥 슬라임 이미지는 그대로 둔 채 '색상 필터(Color)'만 바꿔주므로 안전합니다.
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