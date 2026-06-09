using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    protected string skillName;
    protected int cooldown;
    public abstract void Execute(ref EnemyData target);
}
