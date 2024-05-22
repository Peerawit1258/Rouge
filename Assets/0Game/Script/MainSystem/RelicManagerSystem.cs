using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RelicManagerSystem : MonoBehaviour
{
    [TabGroup("Game"), Unit(Units.Percent), ReadOnly] public int increaseMoney;
    [TabGroup("Game"), Unit(Units.Percent), ReadOnly] public int discount;
    [TabGroup("Game"), Unit(Units.Percent), ReadOnly] public int increaseExp;
    [TabGroup("Game"), Unit(Units.Percent), ReadOnly] public int randomSkill;
    [TabGroup("Game"), Unit(Units.Percent), ReadOnly] public int dropSkill;

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
    // Start is called before the first frame update
    void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        statusEffectSystem = GameManager.instance.statusEffectSystem;
        //Debug.Log(10 * ((posionDmg + dotDmg) / 100) + " " + (int)(10 * ((posionDmg + dotDmg) / 100)));
    }

    public void AddRelic(Relic relic)
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
                                set.type = stat.type;
                                set.value = stat.value;
                            }
                            player.StatUp(set);
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
                //case RelicEffectType.BuffDebuff:

                //    break;
                case RelicEffectType.Money:
                    if (detail.isShop)
                        discount += detail.discount;
                    else
                        increaseMoney += detail.extraMoney;
                    break;
            }
        }

        GameManager.instance.allData.r.Add(relic);
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
                            set.type = stat.type;
                            set.value = stat.value;
                        }
                        player.StatUp(set, false);
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
                //case RelicEffectType.BuffDebuff:

                //    break;
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
                GameManager.instance.allData.r.Remove(re);
                break;
            }
                
    }

    public bool CheckRelicWithID(string id)
    {
        if (GameManager.instance.allData.r.Count == 0) return false;

        foreach(var relic in GameManager.instance.playerData.currentRelics)
        {
            if(relic.id == id)
                return true;
        }

        return false;
    }
    public void TriggerRelicEffect(TriggerStatus trigger)
    {
        if (GameManager.instance.playerData.currentRelics.Count == 0) return;

        foreach (var relic in GameManager.instance.playerData.currentRelics)
            foreach (var detail in relic.relicDetails)
                if (detail.type == RelicEffectType.BuffDebuff)
                    if (detail.trigger == trigger)
                        statusEffectSystem.GetStatusInPlayer(detail.status);
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
