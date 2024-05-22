using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[CreateAssetMenu(fileName = "New Skill",menuName = "Battle/Skill")]
public class SkillAction : ScriptableObject
{
    [ReadOnly]public string id;
    public string skillName;
    [PreviewField] public Sprite skillIcon;
    [PreviewField] public Sprite frame;
    public Rarity rarity;
    public TargetType targetType;
    public GameObject particleEffect;
    [TextArea(4,10)] public string descrition;

    [Title("Skill Detail")]
    [SerializeField] ActionDetail actionDetail;
    
    [Title("Condition")]
    [SerializeField] ConditionDetail conditionDetail;

    [Button]
    public void SetID()
    {
        string[] sk = name.Split("_");
        id = sk[0];
        skillName = sk[1];
    }

    #region ActionDetail
    public SkillType GetSkillType() => actionDetail.skillType;
    public List<int> GetPercentDamage() => actionDetail.percentDMG;
    public bool GetCheckRemoveBuff() => actionDetail.isRemoveBuff;
    public List<AddStatus> GetStatusTarget() => actionDetail.statusEnemy;
    public List<AddStatus> GetStatusSelf() => actionDetail.statusSelf;
    public Effect GetRemoveEffectType() => actionDetail.removeBuff;
    public DamageSpecific GetDamageSpecific() => actionDetail.damageSpecific;
    public bool GetIsHeal() => actionDetail.isHeal;
    public HealType GetHealType() => actionDetail.healDetail.healType;
    public int GetPercentHeal(float ratio) => actionDetail.healDetail.percentHeal + GetHpPercentHeal(ratio);
    public StatusEffect GetRemoveDebuff() => actionDetail.healDetail.removeEffect;
    public bool GetCheckTakeDamage() => actionDetail.isTakeDamage;
    public int GetTakeDamagePercent() => actionDetail.percentHp;
    public bool GetCheckHpCompare() => actionDetail.hpCompare;
    public HpAbility GetHpAbility() => actionDetail.hpAbility;
    public int GetHpPercentSkill(float ratio)
    {
        if (actionDetail.hpAbility.type != SkillType.Attack || !actionDetail.hpCompare) return 0;
        switch (actionDetail.hpAbility.hpCompare)
        {
            case Compare.Lower:
                if (ratio < actionDetail.hpAbility.hpPercent / 100)
                    return actionDetail.hpAbility.percentSkill;
                break;
            case Compare.Greater:
                if (ratio > actionDetail.hpAbility.hpPercent / 100)
                    return actionDetail.hpAbility.percentSkill;
                break;
            default:
                if (ratio == actionDetail.hpAbility.hpPercent / 100)
                    return actionDetail.hpAbility.percentSkill;
                break;
        }
        return 0;
    }

    int GetHpPercentHeal(float ratio)
    {
        if (actionDetail.hpAbility.type != SkillType.Heal || !actionDetail.hpCompare) return 0;
        switch (actionDetail.hpAbility.hpCompare)
        {
            case Compare.Lower:
                if (ratio < actionDetail.hpAbility.hpPercent / 100)
                    return actionDetail.hpAbility.percentHeal;
                break;
            case Compare.Greater:
                if (ratio > actionDetail.hpAbility.hpPercent / 100)
                    return actionDetail.hpAbility.percentHeal;
                break;
            default:
                if (ratio == actionDetail.hpAbility.hpPercent / 100)
                    return actionDetail.hpAbility.percentHeal;
                break;
        }
        return 0;
    }

    public AddStatus GetHpStatus(float ratio)
    {
        if (actionDetail.hpAbility.type != SkillType.Buff || !actionDetail.hpCompare) return null;
        switch (actionDetail.hpAbility.hpCompare)
        {
            case Compare.Lower:
                if (ratio < actionDetail.hpAbility.hpPercent / 100)
                    return actionDetail.hpAbility.status;
                break;
            case Compare.Greater:
                if (ratio > actionDetail.hpAbility.hpPercent / 100)
                    return actionDetail.hpAbility.status;
                break;
            default:
                if (ratio == actionDetail.hpAbility.hpPercent / 100)
                    return actionDetail.hpAbility.status;
                break;
        }
        return null;
    }
    #endregion
    #region ConditionDetail
    public ConditionDetail GetConditionDetail() => conditionDetail;
    public bool GetHaveCondition() => conditionDetail.isCondition;
    public ConditionType GetConditonType() => conditionDetail.condition;
    public List<int> GetLockNum() => conditionDetail.lockNum;
    public List<int> GetSpecificSlotBonus() => conditionDetail.slots;
    public int GetConditionBonusDamage() => conditionDetail.bonusSlotDamage;
    public SkillType GetConditionSkillType() => conditionDetail.c_skillType;
    public bool GetConditionRemoveBuff() => conditionDetail.isRemove;
    public Effect GetConditonEffect() => conditionDetail.c_removeBuff;
    public AddStatus GetConditionStatus() => conditionDetail.c_status;
    public HealType GetConditionHealType() => conditionDetail.healType;
    public int GetConditionHealValue() => conditionDetail.c_heal;
    public StatusEffect GetConditionRemoveDebuff() => conditionDetail.removeEffect;
    #endregion
}
[Serializable]
public class DamageSlot{
    public int box;
    [Unit(Units.Percent)]public int bonus;
}

[Serializable]
public class ActionDetail
{
    [EnumToggleButtons, HideLabel] public SkillType skillType;
    [ShowIf("@skillType == SkillType.Attack")] public List<int> percentDMG;
    [ShowIf("@skillType == SkillType.Debuff")] public bool isRemoveBuff;
    [ShowIf("@skillType == SkillType.Attack || (skillType == SkillType.Debuff && !isRemoveBuff)")]public List<AddStatus> statusEnemy;
    [ShowIf("@!isRemoveBuff")]public List<AddStatus> statusSelf;
    [ShowIf("@skillType == SkillType.Debuff && isRemoveBuff")] public Effect removeBuff;

    [ShowIf("@skillType == SkillType.Attack")] public DamageSpecific damageSpecific;

    [Title("Heal")]
    [ShowIf("@skillType == SkillType.Attack")] public bool isHeal;
    [ShowIf("@(skillType == SkillType.Attack && isHeal)|| skillType == SkillType.Heal")] public HealDetail healDetail;
 
    [Title("TakeDamage")]
    [ShowIf("@skillType != SkillType.Heal")] public bool isTakeDamage;
    [ShowIf("@isTakeDamage && skillType != SkillType.Heal")] public int percentHp;

    [Title("HpCompare")]
    [ShowIf("@skillType != SkillType.Debuff")] public bool hpCompare;
    [ShowIf("@skillType != SkillType.Debuff && hpCompare")] public HpAbility hpAbility;

}

[Serializable]
public class HpAbility
{
    public Compare hpCompare;
    public int hpPercent;
    public SkillType type;
    [ShowIf("@type == SkillType.Attack")] public int percentSkill;
    [ShowIf("@type == SkillType.Buff")] public AddStatus status;
    [ShowIf("@type == SkillType.Heal")] public int percentHeal;

}

[Serializable]
public class ConditionDetail
{
    public bool isCondition;
    [ShowIf("@isCondition")] public List<int> lockNum;
    [ShowIf("@isCondition")] public ConditionType condition;
    [ShowIf("@condition == ConditionType.SlotDamageOrder && isCondition")] public List<int> slots;
    [ShowIf("@condition == ConditionType.SlotDamageOrder && isCondition")] public int bonusSlotDamage;
    [ShowIf("@condition == ConditionType.EffectOrder && isCondition"), EnumToggleButtons, HideLabel] public SkillType c_skillType;

    [ShowIf("@isCondition && condition == ConditionType.EffectOrder && c_skillType == SkillType.Attack")] public int percentDmg;

    [ShowIf("@isCondition && condition == ConditionType.EffectOrder && " +
        "c_skillType == SkillType.Debuff")] public bool isRemove;
    [ShowIf("@isCondition && condition == ConditionType.EffectOrder && " +
        "(c_skillType == SkillType.Debuff && isRemove)")] public Effect c_removeBuff;
    [ShowIf("@isCondition && condition == ConditionType.EffectOrder && " +
        "(c_skillType == SkillType.Buff || c_skillType == SkillType.Debuff && !isRemove)")] public AddStatus c_status;

    [ShowIf("@c_skillType == SkillType.Heal && isCondition && condition == ConditionType.EffectOrder")] public HealType healType;
    [ShowIf("@c_skillType == SkillType.Heal && isCondition && condition == ConditionType.EffectOrder && healType == HealType.Heal")] public bool isPercent;
    [ShowIf("@c_skillType == SkillType.Heal && isCondition && condition == ConditionType.EffectOrder && healType == HealType.Heal")] public int c_heal;
    [ShowIf("@c_skillType == SkillType.Heal && isCondition && condition == ConditionType.EffectOrder && healType == HealType.RemoveSpecific")] public StatusEffect removeEffect;
}

[Serializable]
public class DamageSpecific
{
    public StatusEffect damageForEffect;
    [ShowIf("@damageForEffect != null")] public int percentEffect;
    public int CheckStatus(bool check)
    {
        if (check) return percentEffect;

        return 0;
    }
}

public enum SkillType
{
    Attack,
    Buff,
    Debuff,
    Heal,
    Wait
}

public enum TargetType
{
    SingleTarget,
    AllTarget,
    Random,
    Self,
    Team
    
}

public enum ConditionType
{
    SpecificOrder,
    SlotDamageOrder,
    EffectOrder
}

public enum Rarity
{
    Common,
    Epic,
    Legendary

}

public enum HealType
{
    Heal,
    RemoveDebuff,
    RemoveSpecific
}

public enum Compare
{
    Lower,
    Equal,
    Greater
}
