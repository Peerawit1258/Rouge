using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName ="NewEvent",menuName ="Event")]
public class EventInfo : ScriptableObject
{
    [ReadOnly] public string eventId;
    public string eventName;
    [PreviewField(100)] public Sprite eventImage;
    [TextArea(2,4)] public string description;

    [Title("Choice")]
    public List<ChoiceDetail> choices;

    [Button]
    public void SetID()
    {
        string[] n = name.Split('_');
        eventId = n[0];
        if(n.Length > 1) eventName = n[1];
        //var d = choices[0].reward.RandomReward<int>();
        //Debug.Log(d);
    }
}

[Serializable]
public class ChoiceDetail 
{
    public ChoiceType type;
    public string sentence;
    
    [ShowIf("@type == ChoiceType.Reward")] public RewardDetail reward;
    [ShowIf("@type == ChoiceType.Heal")] public HealDetail heal;
    [ShowIf("@type == ChoiceType.BuffDebuff")] public List<AddStatus> status;
    [ShowIf("@type == ChoiceType.Remove")] public RewardType removeType;
    [ShowIf("@type == ChoiceType.Remove && removeType == RewardType.Gold")] public int value;
    [ShowIf("@type == ChoiceType.Enemy")] public EnemyGroup enemyGroup;

    [Title("After")]
    [PreviewField(50)] public Sprite afterEventImg;
    [TextArea(2,4)]public string afterEventDes;

    [Title("Condition")]
    public bool haveCondition;
    [ShowIf("@haveCondition")] public RewardType conditionType;
    [ShowIf("@haveCondition && conditionType == RewardType.Gold")] public int requiredMoney;
    [ShowIf("@haveCondition && conditionType == RewardType.Relic")] public Relic requiredRelic;
}

[Serializable]
public class RewardDetail
{
    public bool isRandom;
    [ShowIf("@!isRandom")] public RewardType rewardType;
    
    //[ShowIf("@type == RewardType.Gold && !isRandom")] public int gold;
    [ShowIf("@rewardType == RewardType.Gold || isRandom")] public int min;
    [ShowIf("@rewardType == RewardType.Gold || isRandom")] public int max;
    [ShowIf("@rewardType == RewardType.Skill || isRandom")] public List<SkillAction> skills;
    [ShowIf("@rewardType == RewardType.Relic || isRandom")] public List<Relic> relics;
    //[ShowIf("@type == Relic")]

    //public R RandomReward<R>()
    //{
    //    R target;
    //    if (typeof(R) == typeof(int))
    //        target = (R)(object)RandomGold();
    //    else if(typeof(R) == typeof(SkillAction))
    //        target = (R)(object)RandomSkill();
    //    else // ----------------------------------
    //        target = (R)(object)RandomSkill();

    //    return target;
    //}
    public int RandomGold() => Random.Range(min, max + 1);
    public SkillAction RandomSkill() => skills[Random.Range(0, skills.Count)];
    public Relic RandomRelic() => relics[Random.Range(0, relics.Count)];
    
}

[Serializable]
public class HealDetail
{
    public bool isAll;
    public HealType healType;
    [ShowIf("@healType == HealType.Heal")] public int percentHeal;
    [ShowIf("@healType == HealType.RemoveSpecific")] public StatusEffect removeEffect;
}

public enum ChoiceType
{
    Nothing,
    Reward,
    Heal,
    BuffDebuff,
    Remove,
    Enemy
}

public enum RewardType
{
    Gold,
    Skill,
    Relic
}

