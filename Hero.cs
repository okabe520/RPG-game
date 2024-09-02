using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Hero : MonoBehaviour
{
    // 英雄属性
    public float health = 200f;
    public float maxHealth = 200f;
    public float mana = 150f;
    public float maxMana = 150f;
    public float attackPower = 15f;
    public float defense = 5f;
    public float speed = 5f;
    public float rage = 0f;
    public float maxRage = 100f;
    public string element = "Fire";

    // 初始属性值常量
    private const float INITIAL_HEALTH = 200f;
    private const float INITIAL_MANA = 150f;
    private const float INITIAL_DEFENSE = 5f;
    private const float INITIAL_ATTACKPOWER = 15f;

    // 新增道具
    public int healingPotions = 3; // 初始有三瓶回复药水
    public int manaPotions = 3; // 初始有三瓶魔力药水
    public int rageStones = 3; // 初始有三颗怒气石

    // 防御状态
    public bool isDefending = false; // 新增字段来跟踪防御状态
    private bool wasDefending = false; // 新增变量

    public List<Skill> skills = new List<Skill>();

    // 使用字典来存储道具及其数量
    private Dictionary<string, int> inventory = new Dictionary<string, int>();

    // 初始化道具
    void Start()
    {
        InitializeSkills();
        
        // 确保初始化时设置正确的值
        health = maxHealth = INITIAL_HEALTH;
        mana = maxMana = INITIAL_MANA;

        // 初始化道具
        inventory["治疗药水"] = 3;
        inventory["魔力药水"] = 3;
        inventory["怒气石"] = 3;
    }

    // 攻击方法
    public void Attack(Monster target)
    {
        // 普通攻击积攒20点怒气
        AddRage(20f);
    }

    // 使用技能方法
    public bool UseSkill(Skill skill, Monster target)
    {
        if (!skill.isUnlocked)
        {
            Debug.LogWarning($"技能 {skill.name} 尚未解锁，无法使用");
            return false;
        }

        if (skill.isUltimate && rage < maxRage)
        {
            return false;
        }

        if (!skill.isUltimate && mana < skill.manaCost)
        {
            return false;
        }

        if (!skill.isUltimate)
        {
            mana -= skill.manaCost;
            AddRage(skill.manaCost / 2f);
        }
        else
        {
            rage = 0f; // 使用终结技后重置怒气
        }

        // 这里不再计算伤害，而是返回技能的基本信息
        switch (skill.name)
        {
            case "虚弱":
                return true;
            case "火球术":
                // 这里不再调用 target.TakeDamage(damage);
                return true;
            case "冰锥":
                // 这里不再调用 target.TakeDamage(damage);
                return true;
            case "雷击":
                // 这里不再调用 target.TakeDamage(damage);
                return true;
            case "灵魂激流":
                // 这里不再调用 target.TakeDamage(damage);
                return true;
            case "治疗术":
                return true;
            default:
                return false;
        }
    }

    // 使用道具方法
    public bool UseItem(string itemName)
    {
        if (inventory.ContainsKey(itemName) && inventory[itemName] > 0)
        {
            switch (itemName)
            {
                case "治疗药水":
                    health += 50;
                    health = Mathf.Clamp(health, 0f, maxHealth);
                    break;
                case "魔力药水":
                    mana += 60;
                    mana = Mathf.Clamp(mana, 0f, maxMana);
                    break;
                case "怒气石":
                    AddRage(50);
                    break;
                default:
                    return false; // 未知道具
            }
            inventory[itemName]--;
            return true;
        }
        return false;
    }

    // 获取可用道具列表
    public List<string> GetAvailableItems()
    {
        List<string> availableItems = new List<string>();
        foreach (var item in inventory)
        {
            if (item.Value > 0)
            {
                availableItems.Add(item.Key);
            }
        }
        return availableItems;
    }

    // 防御方法
    public void Defend()
    {
        isDefending = true; // 设置防御状态为真
        wasDefending = true; // 设置标志
        defense *= 2;  // 临时增加防御力
    }

    // 受到伤害方法
    public void TakeDamage(float damage)
    {
        float actualDamage = damage - defense;
        if (actualDamage < 0) actualDamage = 0;

        health -= actualDamage;
        health = Mathf.Clamp(health, 0f, maxHealth);

        // 受到攻击，积攒怒气值为实际受伤害值的一半
        AddRage(actualDamage / 2f);
    }

    // 重置状态方法
    public void ResetState()
    {
        health = maxHealth;
        mana = maxMana;
        rage = 0f;
        defense = INITIAL_DEFENSE;
        attackPower = INITIAL_ATTACKPOWER;  
        isDefending = false;
        // Reset other stats as needed
    }

    // 每回合结束时调用，重置防御状态
    public void EndTurn()
    {
        // 移除重置防御状态的逻辑
        // 其他可能的回合结束逻辑保留在这里
    }

    // 初始化技能方法
    private void InitializeSkills()
    {
        skills.Add(new Skill("火球术", 20f, 30f, false, true)); // 默认解锁火球术
        skills.Add(new Skill("冰锥", 15f, 25f, false, true));
        skills.Add(new Skill("雷击", 25f, 40f, false, true));
        skills.Add(new Skill("虚弱", 30f, 0f, false, true));
        skills.Add(new Skill("治疗术", 30f, 0f, false, true)); 
        skills.Add(new Skill("灵魂激流", 0f, 0f, true, true));
    }

    // 添加增加怒气值的方法
    private void AddRage(float amount)
    {
        rage += amount;
        rage = Mathf.Clamp(rage, 0f, maxRage);
    }

    // 新增方法来在回合开始时执行一些操作
    public void StartTurn()
    {
        Debug.Log("Hero StartTurn called"); // 调试输出
    }

    // 添加一个新方法来重置防御状态
    public void ResetDefense()
    {
        if (isDefending)
        {
            isDefending = false;
            defense = INITIAL_DEFENSE;
            // 不重置 wasDefending，因为我们想知道英雄是否曾经处于防御状态
        }
    }

    // 新增方法来检查并重置 wasDefending 状态
    public bool CheckAndResetDefendingStatus()
    {
        bool status = wasDefending;
        wasDefending = false; // 重置状态
        return status;
    }

    // 新增方法来检查是否可以使用灵魂激流
    public bool CanUseMagicSurge()
    {
        return rage >= maxRage;
    }

    // 新增方法来检查是否有足够的魔力
    public bool HasEnoughMana(float manaCost)
    {
        return mana >= manaCost;
    }

    // 新增方法来重置怒气值
    public void ResetRage()
    {
        rage = 0f;
    }

    // 新增方法来解锁技能
    public void UnlockSkill(string skillName)
    {
        Skill skill = skills.Find(s => s.name == skillName);
        if (skill != null)
        {
            skill.isUnlocked = true;
            Debug.Log($"技能 {skillName} 已解锁！");
        }
        else
        {
            Debug.LogWarning($"未找到名为 {skillName} 的技能");
        }
    }

    // 新增方法来获取已解锁的技能列表
    public List<Skill> GetUnlockedSkills()
    {
        return skills.Where(s => s.isUnlocked).ToList();
    }

    // 调整道具数量的方法
    public void AdjustItemQuantity(string itemName, int amount)
    {
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName] += amount;
            if (inventory[itemName] < 0)
            {
                inventory[itemName] = 0;
            }
        }
        else if (amount > 0)
        {
            inventory[itemName] = amount;
        }
        Debug.Log($"道具 {itemName} 的数量调整为: {inventory[itemName]}");
    }

    // 获取道具数量的方法
    public int GetItemQuantity(string itemName)
    {
        return inventory.ContainsKey(itemName) ? inventory[itemName] : 0;
    }
}
