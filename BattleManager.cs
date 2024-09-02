using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private Hero hero;        
    [SerializeField] private Monster monster;  
    [SerializeField] private BattleUI battleUI; // 添加对 BattleUI 的引用
    [SerializeField] private BattleLog battleLog; // 添加 SerializeField 属性

    public bool isHeroTurn { get; private set; } 

    public static BattleManager Instance { get; private set; }

    public float turnDelaySeconds = 3f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 确保 hero 和 monster 已被正确赋值
        if (hero == null || monster == null)
        {
            Debug.LogError("Hero 或 Monster 未被正确赋值！");
            return;
        }

        // 初始化 BattleLog
        if (battleLog == null)
        {
            battleLog = FindObjectOfType<BattleLog>();
            if (battleLog == null)
            {
                Debug.LogError("BattleLog not found in the scene!");
                return;
            }
        }

        // 初始化战斗
        StartBattle();

        // 设置 Monster 的 BattleLog
        if (monster != null)
        {
            monster.SetBattleLog(battleLog);
        }
        else
        {
            Debug.LogError("Monster reference is null in BattleManager");
        }
    }

    // 开始战斗方法
    // 在适当的地方调用 UpdateSkillButtons 方法
    // 例如，在战斗开始时、技能解锁时、或每回合开始时
    void StartBattle()
    {
        Debug.Log("开始新的战斗");

        // 重置英雄和怪物状态
        ResetBattleState();

        // 比较速度，决定谁先行动
        if (hero.speed >= monster.speed)
        {
            isHeroTurn = true; // 英雄先攻
            StartHeroTurn();
        }
        else
        {
            isHeroTurn = false; // 怪物先攻
            StartMonsterTurn();
        }

        UpdateSkillButtons();
    }

    // 重置战斗状态方法
    void ResetBattleState()
    {

        // 重置英雄状态
        hero.ResetState();

        // 重置怪物状态
        monster.ResetState();
    }

    // 修改 NextTurn 方法
    public void NextTurn()
    {
        if (hero.health <= 0 || monster.health <= 0)
        {
            EndBattle();
            return;
        }

        StartCoroutine(DelayedNextTurn());
    }

    // 新增的协程方法
    private IEnumerator DelayedNextTurn()
    {
        yield return new WaitForSeconds(turnDelaySeconds); // 等待3秒

        isHeroTurn = !isHeroTurn;

        if (isHeroTurn)
        {
            StartHeroTurn();
        }
        else
        {
            StartMonsterTurn();
        }
    }

    // 修改 HeroTurn 方法
    public void HeroTurn(string action, Skill skill = null, string item = null)
    {
        if (!isHeroTurn)
        {
            Debug.LogWarning("不是英雄的回合，无法执行操作");
            return;
        }

        bool actionTaken = false;
        float actualDamage = 0f;
        GameObject heroobject = GameObject.Find("Wizard");
        switch (action)
        {
            case "Attack":
                hero.Attack(monster);
                actualDamage = Mathf.Max(hero.attackPower - monster.defense, 0);
                LogMessage($"英雄使用了普通攻击");
                monster.TakeDamage(actualDamage);
                heroobject.GetComponent<Animator>().SetTrigger("Attack");
                actionTaken = true;
                break;
            case "Skill":
                if (skill != null)
                {
                    if (skill.name == "灵魂激流")
                    {
                        if (!hero.CanUseMagicSurge())
                        {
                            LogMessage("怒气值不足，无法使用灵魂激流！");
                            battleUI.EnableButtons(); // 重新启用按钮
                            return;
                        }
                    }
                    else if (!hero.HasEnoughMana(skill.manaCost))
                    {
                        LogMessage($"魔力值不足，无法使用{skill.name}！");
                        battleUI.EnableButtons(); // 重新启用按钮
                        return;
                    }

                    actionTaken = hero.UseSkill(skill, monster);
                    
                    if (actionTaken)
                    {
                        LogMessage($"英雄使用了技能 {skill.name}");

                        switch (skill.name)
                        {
                            case "火球术":
                                actualDamage = Mathf.Max(skill.damage - monster.defense, 0);
                                monster.TakeDamage(actualDamage);
                                
                                heroobject.GetComponent<Animator>().SetTrigger("Attack");
                                if (Random.value < 0.4f || monster.BurnTurnsRemaining > 0)
                                {
                                    monster.ApplyBurn();
                                    if (monster.BurnTurnsRemaining > 0)
                                    {
                                        LogMessage($"火球术重置了{monster.monsterName}的燃烧状态，持续3回合");
                                    }
                                    else
                                    {
                                        LogMessage($"火球术使{monster.monsterName}陷入燃烧状态，持续3回合");
                                    }
                                }
                                break;
                            case "冰锥":
                                actualDamage = Mathf.Max(skill.damage - monster.defense, 0);
                                monster.TakeDamage(actualDamage);
                                
                                heroobject.GetComponent<Animator>().SetTrigger("Attack");
                                if (Random.value < 0.5f)
                                {
                                    monster.ApplyFreeze();
                                    LogMessage($"冰锥使{monster.monsterName}陷入冰冻状态！");
                                }
                                break;
                            case "雷击":
                                actualDamage = Mathf.Max(skill.damage - monster.defense, 0);
                                monster.TakeDamage(actualDamage);

                                heroobject.GetComponent<Animator>().SetTrigger("Attack");
                                bool wasElectrified = monster.IsElectrified;
                                bool applyElectrified = Random.value < 0.3f;

                                if (applyElectrified || wasElectrified)
                                {
                                    monster.ApplyElectrified();
                                    if (wasElectrified)
                                    {
                                        LogMessage($"{monster.monsterName}的触电状态被重置，持续3回合");
                                    }
                                    else
                                    {
                                        LogMessage($"{monster.monsterName}陷入触电状态，持续3回合");
                                    }
                                }
                                break;
                            case "虚弱":
                                monster.ApplyWeaken(3);
                                if (monster.IsWeakened)
                                {
                                    LogMessage($"虚弱技能重置了{monster.monsterName}的虚弱状态，怪物攻击力降低25%，受到伤害增加20%，持续3回合");
                                }
                                else
                                {
                                    LogMessage($"虚弱技能使{monster.monsterName}陷入虚弱状态，怪物攻击力降低25%，受到伤害增加20%，持续3回合");
                                }
                                break;
                            case "灵魂激流":
                                actualDamage = Mathf.Max(hero.attackPower * 5 - monster.defense, 0);
                                monster.TakeDamage(actualDamage);

                                heroobject.GetComponent<Animator>().SetTrigger("Attack");
                                hero.ResetRage(); // 假设你有一个重置怒气值的方法
                                break;
                            case "治疗术":
                                float healAmount = 60f;
                                hero.health = Mathf.Min(hero.health + healAmount, hero.maxHealth);
                                LogMessage($"英雄使用了治疗术，恢复了 {healAmount} 点生命值！");
                                break;
                            default:
                                Debug.LogError($"Unknown skill: {skill.name}");
                                break;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Skill is null in HeroTurn");
                }
                break;
            case "Item":
                if (!string.IsNullOrEmpty(item))
                {
                    actionTaken = hero.UseItem(item);
                    if (actionTaken)
                    {
                        switch (item)
                        {
                            case "治疗药水":
                                LogMessage($"英雄使用了治疗药水，恢复50点生命值！当前生命值: {hero.health:F1}");
                                break;
                            case "魔力药水":
                                LogMessage($"英雄使用了魔力药水，恢复60点魔力值！当前魔力值: {hero.mana:F1}");
                                break;
                            case "怒气石":
                                LogMessage($"英雄使用了怒气石，增加50点怒气值！当前怒气值: {hero.rage:F1}");
                                break;
                        }
                    }
                    else
                    {
                        LogMessage($"无法使用 {item}！");
                    }
                }
                else
                {
                    LogMessage("没有选择道具！");
                }
                break;
            case "Defend":
                hero.Defend();
                LogMessage("英雄进入防御状态");
                actionTaken = true;
                break;
            default:
                Debug.LogError("Unknown action in HeroTurn: " + action);
                break;
        }

        if (actionTaken)
        {
            Debug.Log($"Hero performed action: {action}");
            hero.EndTurn();
            if (hero.isDefending)
            {
            }
            NextTurn(); // 这里会调用 DelayedNextTurn
        }
        else
        {
            Debug.Log($"Action not taken: {action}");
            battleUI.EnableButtons(); // 如果动作未成功执行，重新启用按钮
        }
    }

    // 修改 MonsterTurn 方法
    void MonsterTurn()
    {
        // 处理冰冻状态
        if (monster.IsFrozen)
        {
            LogMessage($"{monster.monsterName}被冰冻，本回合无法行动！");
            monster.HandleFrozenState();
            monster.EndTurn();
            NextTurn(); // 这里会调用 DelayedNextTurn
            return;
        }

        // 处理虚弱状态
        bool wasWeakened = monster.attackPower < 15f; // 假设 15f 是怪物的初始攻击力

        float damage = monster.DecideAction(hero);

        if (monster.lastAction == "DeathRoar" || monster.lastAction == "BloodSacrifice" || 
            monster.lastAction == "BloodSacrificeAttack" || monster.lastAction == "Heal" || 
            monster.lastAction == "VampiricDagger")
        {
            LogMessage(monster.lastActionDescription); // 使用存储的描述
        }
        else
        {
            string actionDescription = GetMonsterActionDescription(monster.lastAction);
            if (damage > 0)
            {
                LogMessage($"{monster.monsterName}{actionDescription}，对英雄造成 {Mathf.Max(damage - hero.defense,0):F1} 点伤害。英雄剩余生命值：{hero.health:F1}");
                UpdateHeroStatus(); // 更新英雄状态
            }
            else
            {
                LogMessage($"{monster.monsterName}{actionDescription}");
            }
        }
        
        monster.EndTurn();

        if (monster.HasWeakenJustEnded())
        {
            LogMessage($"{monster.monsterName}的虚弱状态结束了。");
        }

        // 在怪物回合结束后重置英雄的防御状态
        hero.ResetDefense();
        if (hero.CheckAndResetDefendingStatus())
        {
            LogMessage("英雄的防御状态结束了。");
        }

        if (monster.HasChargePowerJustEnded())
        {
            LogMessage($"{monster.monsterName}的蓄力状态结束了。");
        }

        NextTurn(); // 这里会调用 DelayedNextTurn
    }

    // 修改 GetMonsterActionDescription 方法
    private string GetMonsterActionDescription(string action)
    {
        switch (action)
        {
            case "Attack":
                return "进行了普通攻击";
            case "HeavyStrike":
                return "使用了猛击";
            case "FullPowerStrike":
                return "使用了全力一击";
            case "MayRainSlash":
                return "使用了五月雨斩";
            case "ChargePower":
                return "蓄力了";
            case "DeathRoar":
                return "释放了死亡怒吼";
            case "BloodSacrifice":
                return "使用了鲜血献祭";
            case "BloodSacrificeAttack":
                return "使用了鲜血献祭强化攻击";
            case "Heal":
                return "使用了治疗";
            case "VampiricDagger":
                return "使用了吸血匕首";
            case "BloodDagger":
                return "使用了附血匕首";
            case "ChargeDeathContract":
                return "正在签订死神契约";
            case "ReleaseDeathContract":
                return "签订了死神契约";
            case "StoneShards":
                return "使用了碎石";
            case "GroundSmash":
                return "使用了砸地";
            case "StoneWill":
                return "使用了石之意志";
            default:
                return "进行了攻击";
        }
    }

    // 新增方法 LogMessage
    public void LogMessage(string message)
    {
        if (battleLog != null)
        {
            battleLog.AddMessage(message);
        }
        else
        {
            Debug.LogWarning("BattleLog is null, cannot log message: " + message);
            // 如果 BattleLog 为空，使用 Debug.Log 作为备选
            Debug.Log("Battle Log: " + message);
        }
    }

    // 添加这个方法到 BattleManager 类中
    public void SetBattleUI(BattleUI ui)
    {
        battleUI = ui;
    }

    // 新增方法 StartHeroTurn 和 StartMonsterTurn
    void StartHeroTurn()
    {
        LogMessage($"英雄回合开始");
        hero.StartTurn();

        if (monster.HasElectrifiedJustEnded())
        {
            LogMessage("怪物的触电状态结束了。");
        }

        battleUI.EnableButtons();
        battleUI.UpdateUI();
    }

    void StartMonsterTurn()
    {
        LogMessage($"{monster.monsterName}回合开始");
        Monster.BurnInfo burnInfo = monster.StartTurn();
        HandleBurnEffects(burnInfo);
        battleUI.DisableButtons();
        MonsterTurn();
    }

    // 新增方法 EndBattle
    private IEnumerator EndBattleWithDelay(string message, float delay)
    {
        LogMessage(message);
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MapScene"); // 确保这是你的地图场景的名称
    }

    void EndBattle()
    {
        if (hero.health <= 0)
        {
            StartCoroutine(EndBattleWithDelay("战斗结束：英雄阵亡", 10f));
        }
        else if (monster.health <= 0)
        {
            HealingBoss healingBoss = monster as HealingBoss;
            if (healingBoss != null && healingBoss.IsInSecondPhase())
            {
                StartCoroutine(EndBattleWithDelay("战斗结束：最终Boss被击败", 10f));
            }
            else if (healingBoss == null)
            {
                StartCoroutine(EndBattleWithDelay($"战斗结束：{monster.monsterName}被击败", 10f));
            }
            // 如果是HealingBoss但不在第二阶段，不做任何操作，让战斗继续
        }
    }

    void HandleBurnEffects(Monster.BurnInfo burnInfo)
    {
        if (burnInfo.damage > 0)
        {
            LogMessage($"{monster.monsterName}剩余燃烧回合：{burnInfo.turnsRemaining}");
        }

        if (monster.HasBurnJustEnded())
        {
            LogMessage($"{monster.monsterName}的燃烧状态结束了。");
        }
    }

    // 新增方法来更新技能按钮
    void UpdateSkillButtons()
    {
        List<Skill> unlockedSkills = hero.GetUnlockedSkills();
        battleUI.UpdateSkillButtons(unlockedSkills);
    }

    // 在适当的地方（比如当玩家点击"技能"按钮时）
    void OnSkillButtonClicked()
    {
        List<Skill> unlockedSkills = hero.GetUnlockedSkills();
        bool canUseUltimate = hero.CanUseMagicSurge();
        battleUI.skillSelectionUI.ShowSkillSelection(unlockedSkills, canUseUltimate);
    }

    public void InitializeBattle()
    {
        // 初始化战斗
        StartBattle();
    }

    public void UpdateHeroStatus()
    {
        if (battleUI != null)
        {
            battleUI.UpdateHeroStatus();
        }
        else
        {
            Debug.LogWarning("BattleUI is null in BattleManager");
        }
    }
}