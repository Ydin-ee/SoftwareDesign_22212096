using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    private Dictionary<string, int> skillLevels = new Dictionary<string, int>();
    public List<string> equippedSortSkills = new List<string>();

    private void Start()
    {
        skillLevels.Add("BubbleSort", 1);
        skillLevels.Add("StackShield", 1);
        equippedSortSkills.Add("BubbleSort"); 
    }
    
    public List<string> GetOwnedSkills()
    {
        return new List<string>(skillLevels.Keys);
    }

    public int GetSkillLevel(string skillName)
    {
        if (skillLevels.ContainsKey(skillName)) 
            return skillLevels[skillName];
        
        return 0; // 스킬이 없으면 0 반환
    }

    // 스킬의 레벨을 1 올리거나 새로 획득하는 함수
    public void UpgradeSkill(string skillName)
    {
        if (skillLevels.ContainsKey(skillName))
        {
            skillLevels[skillName]++;
        }
        else
        {
            skillLevels.Add(skillName, 1);
        }
    }

    // 임시 테스트용: 현재 가진 스킬 중 하나를 무작위로 골라 강화
    public void UpgradeRandomOwnedSkill()
    {
        List<string> ownedSkills = new List<string>(skillLevels.Keys);
        if (ownedSkills.Count > 0)
        {
            int randomIndex = Random.Range(0, ownedSkills.Count);
            UpgradeSkill(ownedSkills[randomIndex]);
        }
    }
    
}