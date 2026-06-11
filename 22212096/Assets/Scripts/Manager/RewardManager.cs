using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardManager : MonoBehaviour
{
    [Header("Reward UI References")]
    [SerializeField] private GameObject rewardPanel; 
    [SerializeField] private Button[] rewardButtons; 
    [SerializeField] private TextMeshProUGUI[] rewardTexts; 

    [Header("Skill Evolution UI References")]
    [SerializeField] private GameObject skillEvolutionPanel;
    [SerializeField] private Button[] evolutionButtons;
    [SerializeField] private TextMeshProUGUI[] evolutionTexts;

    [Header("Reward UI Components")]
    [SerializeField] private Button selectionSortUpgradeButton; 
    [SerializeField] private TextMeshProUGUI selectionSortButtonText; 

    private int calculatedHealAmount = 0;
    private const int maxHpIncreaseAmount = 20; 

    private void Start()
    {
        if (rewardPanel != null) rewardPanel.SetActive(false);
        if (skillEvolutionPanel != null) skillEvolutionPanel.SetActive(false);
    }

    public void ShowRewards()
    {
        if (rewardPanel != null) rewardPanel.SetActive(true);
        if (skillEvolutionPanel != null) skillEvolutionPanel.SetActive(false);
        
        PlayerController player = FindFirstObjectByType<PlayerController>();
        PlayerSkillManager skillManager = FindFirstObjectByType<PlayerSkillManager>(); 
        int currentStage = GameManager.Instance.CurrentStage;

        // 1. 보스 스테이지 클리어 시 (5의 배수 층)
        if (currentStage % 5 == 0)
        {
            rewardButtons[0].gameObject.SetActive(false); 
            rewardButtons[2].gameObject.SetActive(false); 
            
            rewardButtons[1].gameObject.SetActive(true);
            rewardButtons[1].interactable = true;

            string bossSkill = (currentStage == 5) ? "SelectionSort" : "InsertionSort";
            string bossSkillNameKor = (currentStage == 5) ? "선택 정렬" : "삽입 정렬";

            int bossSkillLevel = (skillManager != null) ? skillManager.GetSkillLevel(bossSkill) : 0;

            if (bossSkillLevel >= 3)
            {
                if (rewardTexts[1] != null) 
                    rewardTexts[1].text = "보스 전리품\n[" + bossSkillNameKor + "] MAX 달성\n(클릭 시 다음 층 이동)";
            }
            else
            {
                if (rewardTexts[1] != null) 
                    rewardTexts[1].text = "보스 전리품\n[" + bossSkillNameKor + "] 해금";
            }
            
            rewardButtons[1].onClick.RemoveAllListeners();
            rewardButtons[1].onClick.AddListener(() => UnlockNewSkill(bossSkill));
        }
        // 2. 일반 스테이지 클리어 시
        else
        {
            for (int i = 0; i < rewardButtons.Length; i++)
            {
                rewardButtons[i].gameObject.SetActive(true);
                rewardButtons[i].interactable = true;
            }

            if (rewardTexts[0] != null) rewardTexts[0].text = $"최대 체력\n+{maxHpIncreaseAmount}";
            rewardButtons[0].onClick.RemoveAllListeners();
            rewardButtons[0].onClick.AddListener(() => SelectReward(0));

            if (player != null && rewardTexts[1] != null)
            {
                // 주주의 팁: 처음 열릴 때만 랜덤 계산을 수행하므로, 뒤로가기를 해도 회복량이 주작되지 않고 그대로 보존됩니다.
                float randomPercent = Random.Range(0.20f, 0.50f);
                calculatedHealAmount = Mathf.RoundToInt(player.MaxHP * randomPercent);
                rewardTexts[1].text = $"체력 회복\n+{calculatedHealAmount}";
            }
            rewardButtons[1].onClick.RemoveAllListeners();
            rewardButtons[1].onClick.AddListener(() => SelectReward(1));

            bool canEvolve = false;
            if (skillManager != null)
            {
                List<string> ownedSkills = skillManager.GetOwnedSkills();
                foreach (string skill in ownedSkills)
                {
                    if (skillManager.GetSkillLevel(skill) < 3)
                    {
                        canEvolve = true; 
                        break;
                    }
                }
            }

            if (rewardTexts[2] != null) 
            {
                if (canEvolve)
                {
                    rewardTexts[2].text = "자료구조/정렬\n스킬 진화";
                    rewardButtons[2].interactable = true; 
                }
                else
                {
                    rewardTexts[2].text = "스킬 진화\n<color=red>[모든 스킬 MAX]</color>";
                    rewardButtons[2].interactable = false; 
                }
            }

            rewardButtons[2].onClick.RemoveAllListeners();
            rewardButtons[2].onClick.AddListener(() => SelectReward(2));
        }
    }

    public void SelectReward(int index)
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();

        if (index == 0)
        {
            if (player != null) player.IncreaseMaxHP(maxHpIncreaseAmount);
            CloseRewardAndAdvance();
        }
        else if (index == 1)
        {
            if (player != null) player.Heal(calculatedHealAmount);
            CloseRewardAndAdvance();
        }
        else if (index == 2)
        {
            if (rewardPanel != null) rewardPanel.SetActive(false);
            OpenSkillEvolutionPanel();
        }
    }

    public void OnClickBackButtonFromEvolution()
    {
        if (skillEvolutionPanel != null) skillEvolutionPanel.SetActive(false);
        if (rewardPanel != null) rewardPanel.SetActive(true);
    }

    private void UnlockNewSkill(string newSkillName)
    {
        PlayerSkillManager skillManager = FindFirstObjectByType<PlayerSkillManager>();
        if (skillManager != null)
        {
            if (skillManager.GetSkillLevel(newSkillName) < 3)
            {
                skillManager.UpgradeSkill(newSkillName); 
            }

            if (!skillManager.equippedSortSkills.Contains(newSkillName))
            {
                skillManager.equippedSortSkills.Add(newSkillName);
            }
        }
        CloseRewardAndAdvance();
    }

    private void CloseRewardAndAdvance()
    {
        if (rewardPanel != null) rewardPanel.SetActive(false);
        if (skillEvolutionPanel != null) skillEvolutionPanel.SetActive(false);
        GameManager.Instance.AdvanceToNextStage();
    }

    private void OpenSkillEvolutionPanel()
    {
        if (skillEvolutionPanel != null) skillEvolutionPanel.SetActive(true);

        PlayerSkillManager skillManager = FindFirstObjectByType<PlayerSkillManager>();
        if (skillManager == null) return;

        List<string> ownedSkills = skillManager.GetOwnedSkills();

        for (int i = 0; i < evolutionButtons.Length; i++)
        {
            if (i < ownedSkills.Count)
            {
                evolutionButtons[i].gameObject.SetActive(true); 
                evolutionButtons[i].interactable = true; 

                string skillName = ownedSkills[i];
                int currentLevel = skillManager.GetSkillLevel(skillName);
                string korName = GetSkillKoreanName(skillName);

                if (currentLevel >= 3)
                {
                    if (evolutionTexts[i] != null)
                    {
                        evolutionTexts[i].text = $"{korName}\n[최대 레벨 달성]";
                    }
                    evolutionButtons[i].interactable = false; 
                }
                else
                {
                    if (evolutionTexts[i] != null)
                    {
                        evolutionTexts[i].text = $"{korName}\nLV.{currentLevel} -> LV.{currentLevel + 1}";
                    }
                    
                    evolutionButtons[i].onClick.RemoveAllListeners();
                    evolutionButtons[i].onClick.AddListener(() => OnSkillSelectedForEvolution(skillName));
                }
            }
            else
            {
                evolutionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnSkillSelectedForEvolution(string skillName)
    {
        PlayerSkillManager skillManager = FindFirstObjectByType<PlayerSkillManager>();
        if (skillManager != null)
        {
            skillManager.UpgradeSkill(skillName);
        }

        CloseRewardAndAdvance();
    }

    private string GetSkillKoreanName(string englishName)
    {
        switch (englishName)
        {
            case "BubbleSort": return "버블 정렬";
            case "SelectionSort": return "선택 정렬";
            case "InsertionSort": return "삽입 정렬";
            case "StackShield": return "스택 쉴드";
            case "QueueShield": return "큐 쉴드";
            default: return englishName;
        }
    }
}