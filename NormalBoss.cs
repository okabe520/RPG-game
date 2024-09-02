using UnityEngine;

public class NormalBoss : Monster
{
    public override float DecideAction(Hero hero)
    {
        if (IsFrozen)
        {
            SetLastAction("Frozen");
            return 0;
        }

        float damage = Attack(hero);
        SetLastAction("Attack");
        return damage;
    }

    // 可以根据需要重写其他方法，例如：
    public override void ResetState()
    {
        base.ResetState();
        // 添加 NormalBoss 特定的重置逻辑（如果有的话）
    }

    protected override void Start()
    {
        monsterName = "小杂兵"; // 设置特定的Boss名字
        maxHealth = 200f;
        health = maxHealth;
        base.Start();
    }
}