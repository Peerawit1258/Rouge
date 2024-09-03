using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RelicManagerSystem : MonoBehaviour
{
    [TabGroup("Game"), Unit(Units.Percent), ReadOnly] public int increaseMoney;
    [TabGroup("Game"), Unit(Units.Percent), ReadOnly] public int discount;
    [TabGroup("Game"), Unit(Units.Percent), ReadOnly] public int increaseExp;
    [TabGroup("Game"), ReadOnly] public int randomSkill;
    [TabGroup("Game"), ReadOnly] public int dropSkill;

    [TabGroup("Turn"), ReadOnly] public List<AddStatus> startPlayerStatus;
    [TabGroup("Turn"), ReadOnly] public List<AddStatus> endPlayerStatus;
    [TabGroup("Turn"), ReadOnly] public List<AddStatus> startEnemyStatus;
    [TabGroup("Turn"), ReadOnly] public int startHeal;
    [TabGroup("Turn"), ReadOnly] public int endHeal;

    [TabGroup("DOT"), SerializeField, ReadOnly] int increaseStack;
    [Header("Damage")]
    [TabGroup("DOT"), SerializeField, Unit(Units.Percent), ReadOnly] float posionDmg;
    [TabGroup("DOT"), SerializeField, Unit(Units.Percent), ReadOnly] float burnDmg;
    [TabGroup("DOT"), SerializeField, Unit(Units.Percent), ReadOnly] float bleedDmg;
    [TabGroup("DOT"), SerializeField, Unit(Units.Percent), ReadOnly] float curseDmg;
    [TabGroup("DOT"), SerializeField, Unit(Units.Percent), ReadOnly] float dotDmg;
    [Header("Cap Stack")]
    [TabGroup("DOT"), SerializeField, ReadOnly] int posionCap;
    [TabGroup("DOT"), SerializeField, ReadOnly] int burnCap;
    [TabGroup("DOT"), SerializeField, ReadOnly] int bleedCap;
    [TabGroup("DOT"), SerializeField, ReadOnly] int curseCap;

    public GameObject relicPrefab;


    PlayerController player;
    StatusEffectSystem statusEffectSystem;
    DetailPanel detailPanel;
    // Start is called before the first frame update
    void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        statusEffectSystem = GameManager.instance.statusEffectSystem;
        detailPanel = GameManager.instance.detailPanel;
        //Debug.Log(10 * ((posionDmg + dotDmg) / 100) + " " + (int)(10 * ((posionDmg + dotDmg) / 100)));
    }

    public void AddRelic(List<Relic> relics) // Triger when start
    {
        if(detailPanel == null) detailPanel = GameManager.instance.detailPanel;
        foreach (Relic relic in relics)
        {
            RelicWidget widget = Instantiate(relicPrefab, detailPanel.GetRelicPlace()).GetComponent<RelicWidget>();
            widget.SetupRelic(relic);
            detailPanel.GetRelicWidgets().Add(widget);
            widget.SetRelicInfo(detailPanel.CreateInfo(relic));
            AddRelic(relic);
        }
        detailPanel.OrderRelic();
    }

    public void AddRelic(Relic relic) // Add new skill
    {
        if(player == null) player = FindAnyObjectByType<PlayerController>();
        foreach(var detail in relic.relicDetails)
        {
            switch (detail.type)
            {
                case RelicEffectType.Stat:
                    if (detail.isSlot)
                    {
                        foreach (var slot in detail.slots)
                        {
                            if (slot.bonus >= 0)
                                GameManager.instance.skillOrderSystem.GetAllSlot()[slot.box].IncreaseValueBonusDMG(slot.bonus);
                            else
                                GameManager.instance.skillOrderSystem.GetAllSlot()[slot.box].DecreaseValueBonusDMG(slot.bonus);
                        }
                    }
                    else
                    {
                        if (!detail.hpCompare)
                        {
                            StatValue set = new StatValue();
                            foreach (var stat in detail.statValues)
                            {
                                //set.type = stat.type;
                                //set.value = stat.value;
                                player.StatUp(stat);
                            }
                            
                        }

                    }
                    break;
                case RelicEffectType.Skill:
                    randomSkill += detail.increaseRandomSkill;
                    break;
                case RelicEffectType.DOT:
                    if (detail.isStack)
                        increaseStack += detail.stack;
                    else
                    {
                        switch (detail.dotID)
                        {
                            case "DD001":
                                posionDmg += detail.dotDamage;
                                break;
                            case "DD002":
                                burnDmg += detail.dotDamage;
                                break;
                            case "DD003":
                                bleedDmg += detail.dotDamage;
                                break;
                            case "DD004":
                                curseDmg += detail.dotDamage;
                                break;
                            default:
                                dotDmg += detail.dotDamage;
                                break;
                        }
                    }
                    break;
                case RelicEffectType.TurnTrigger:
                    if (detail.mainTrigger == SkillType.Buff || detail.mainTrigger == SkillType.Debuff)
                        AddStatusEffect(detail.main, detail.trigger, detail.status);
                    else if (detail.mainTrigger == SkillType.Heal)
                    {
                        if (detail.trigger == TriggerStatus.Start)
                            startHeal += detail.healTrigger;
                        else
                            endHeal += detail.healTrigger;
                    }
                        
                    break;
                case RelicEffectType.Money:
                    if (detail.isShop)
                        discount += detail.discount;
                    else
                        increaseMoney += detail.extraMoney;
                    break;
            }
        }

        GameManager.instance.playerData.currentRelics.Add(relic);
        //GameManager.instance.detailPanel.CreateInfo(relic);
    }

    public void RemoveRelic(Relic relic)
    {
        if (player == null) player = FindAnyObjectByType<PlayerController>();
        foreach (var detail in relic.relicDetails)
        {
            switch (detail.type)
            {
                case RelicEffectType.Stat:
                    if (detail.isSlot)
                    {
                        foreach (var slot in detail.slots)
                        {
                            if (slot.bonus >= 0)
                                GameManager.instance.skillOrderSystem.GetAllSlot()[slot.box].DecreaseValueBonusDMG(slot.bonus);
                            else
                                GameManager.instance.skillOrderSystem.GetAllSlot()[slot.box].IncreaseValueBonusDMG(slot.bonus);
                        }
                    }
                    else
                    {
                        StatValue set = new StatValue();
                        foreach (var stat in detail.statValues)
                        {
                            player.StatUp(stat, false);
                        }
                        
                    }
                    
                    break;
                case RelicEffectType.Skill:
                    randomSkill -= detail.increaseRandomSkill;
                    break;
                case RelicEffectType.DOT:
                    if (detail.isStack)
                        increaseStack -= detail.stack;
                    else
                    {
                        switch (detail.dotID)
                        {
                            case "DD001":
                                posionDmg -= detail.dotDamage;
                                break;
                            case "DD002":
                                burnDmg -= detail.dotDamage;
                                break;
                            case "DD003":
                                bleedDmg -= detail.dotDamage;
                                break;
                            case "DD004":
                                curseDmg -= detail.dotDamage;
                                break;
                            default:
                                dotDmg -= detail.dotDamage;
                                break;
                        }
                    }
                    break;
                case RelicEffectType.TurnTrigger:
                    if (detail.mainTrigger == SkillType.Buff || detail.mainTrigger == SkillType.Debuff)
                        AddStatusEffect(detail.main, detail.trigger, detail.status, false);
                    else if (detail.mainTrigger == SkillType.Heal)
                    {
                        if (detail.trigger == TriggerStatus.Start)
                            startHeal -= detail.healTrigger;
                        else
                            endHeal -= detail.healTrigger;
                    }
                    break;
                case RelicEffectType.Money:
                    if (detail.isShop)
                        discount -= detail.discount;
                    else
                        increaseMoney -= detail.extraMoney;
                    break;
                default:
                    break;
            }
        }

        foreach(var re in GameManager.instance.playerData.currentRelics)
            if(re.id == relic.id)
            {
                GameManager.instance.playerData.currentRelics.Remove(re);
                break;
            }

        foreach (var re in detailPanel.GetRelicWidgets())
        {
            if(re.GetRelic().id == relic.id)
            {
                detailPanel.RemoveRelicInfo(re);
                detailPanel.GetRelicWidgets().Remove(re);
                Destroy(re.gameObject);
                if (detailPanel.GetRelicWidgets().Count > 0) detailPanel.OrderRelic();
                break;
            }
        }
                
    }

    public void TriggerRelicEffect(TriggerStatus trigger)
    {
        //if (GameManager.instance.playerData.currentRelics.Count == 0) return;
        if(trigger == TriggerStatus.Start)
        {
            if(startPlayerStatus.Count > 0)
                foreach (var add in startPlayerStatus)
                    statusEffectSystem.GetStatusInPlayer(add);
            if(startHeal > 0)
            {
                int value = (int)GameManager.instance.turnManager.player.maxHpValue * startHeal / 100;
                GameManager.instance.turnManager.player.StartHealHP(value, 0);
            }
                
            if (startEnemyStatus.Count > 0)
                foreach (var enemy in GameManager.instance.turnManager.enemies)
                    foreach (var add in startEnemyStatus)
                        statusEffectSystem.GetStatusInEnemy(enemy, add);
        }
        else
        {
            if (endPlayerStatus.Count > 0)
                foreach (var add in endPlayerStatus)
                    statusEffectSystem.GetStatusInPlayer(add);
            if (endHeal > 0)
            {
                int value = (int)GameManager.instance.turnManager.player.maxHpValue * startHeal / 100;
                GameManager.instance.turnManager.player.StartHealHP(value, 0);
            }
        }

        //foreach (var relic in GameManager.instance.playerData.currentRelics)
        //    foreach (var detail in relic.relicDetails)
        //        if (detail.type == RelicEffectType.BuffDebuff)
        //            if (detail.trigger == trigger)
        //                statusEffectSystem.GetStatusInPlayer(detail.status);
    }

    public int GetBonusStatFromHpCompare(float ratio, StatType type)
    {
        int stat = 0;
        foreach(var relic in GameManager.instance.playerData.currentRelics)
        {
            foreach(var detail in relic.relicDetails)
            {
                if (detail.hpCompare && detail.CheckHpCompare(ratio))
                {
                    foreach(var st in detail.statValues)
                    {
                        if(st.type == type)
                            stat += st.value;
                    }
                }
            }
        }
        return stat;
    }

    void AddStatusEffect(ActionTurn action, TriggerStatus trigger, AddStatus status, bool isAdd = true)
    {
        if (action == ActionTurn.player)
        {
            if(trigger == TriggerStatus.Start)
            {
                foreach(AddStatus add in startPlayerStatus)
                {
                    if (add.statusEffect.id == status.statusEffect.id)
                    {
                        if(isAdd)
                            add.count += status.count;
                        else
                        {
                            add.count -= status.count;
                            if(add.count <= 0)
                                startPlayerStatus.Remove(add);
                        }
                        return;
                    }
                }
                
                if(isAdd)
                    startPlayerStatus.Add(status);      
            }else if(trigger == TriggerStatus.End)
            {
                foreach (AddStatus add in endPlayerStatus)
                {
                    if (add.statusEffect.id == status.statusEffect.id)
                    {
                        if (isAdd)
                            add.count += status.count;
                        else
                        {
                            add.count -= status.count;
                            if (add.count <= 0)
                                endPlayerStatus.Remove(add);
                        }
                        return;
                    }
                }

                if (isAdd)
                    endPlayerStatus.Add(status);
            }
        }
        else
        {
            if (trigger == TriggerStatus.Start)
            {
                foreach (AddStatus add in startEnemyStatus)
                {
                    if (add.statusEffect.id == status.statusEffect.id)
                    {
                        if (isAdd)
                            add.count += status.count;
                        else
                        {
                            add.count -= status.count;
                            if (add.count <= 0)
                                startEnemyStatus.Remove(add);
                        }
                        return;
                    }
                }

                if (isAdd)
                    startEnemyStatus.Add(status);
            }
        }
    }
    #region Dot
    public int DamageDOTIncrease(string dotID, int damage)
    {
        if (dotID == "DD001")
            return damage + (int)(damage * ((posionDmg + dotDmg) / 100));
        else if (dotID == "DD002")
            return damage + (int)(damage * ((burnDmg + dotDmg) / 100));
        else if (dotID == "DD003")
            return damage + (int)(damage * ((bleedDmg + dotDmg) / 100));
        else if (dotID == "DD004")
            return damage + (int)(damage * ((curseDmg + dotDmg) / 100));
        else
            return damage;
    }

    public int IncreaseCapStack(string dotID)
    {
        if (dotID == "DD001")
            return posionCap;
        else if (dotID == "DD002")
            return burnCap;
        else if (dotID == "DD003")
            return bleedCap;
        else if (dotID == "DD004")
            return curseCap;
        else
            return 0;
    }

    public int GetIncreaseStack() => increaseStack;

    #endregion
}
