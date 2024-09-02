using UnityEngine;

[System.Serializable]
public class Skill
{
    public string name;
    public float manaCost;
    public float damage;
    public bool isUltimate;
    public bool isUnlocked; // 新增变量，表示技能是否已解锁

    public Skill(string name, float manaCost, float damage, bool isUltimate = false, bool isUnlocked = false)
    {
        this.name = name;
        this.manaCost = manaCost;
        this.damage = damage;
        this.isUltimate = isUltimate;
        this.isUnlocked = isUnlocked; // 初始化时设置解锁状态
    }
}