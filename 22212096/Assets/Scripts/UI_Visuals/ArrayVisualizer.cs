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
        // 1. 기존 블록 초기화
        foreach (GameObject block in activeBlocks)
        {
            Destroy(block);
        }
        activeBlocks.Clear();

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
            activeBlocks.Add(newBlock);
        }
    }

    public IEnumerator PlaySwapAnimation(int indexA, int indexB)
    {
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