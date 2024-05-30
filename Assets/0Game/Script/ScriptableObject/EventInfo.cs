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
    public int change;
    //[ShowIf("@change != 0")] public CostType cost;
    //[ShowIf("@change != 0 && cost == CostType.Hp"), Unit(Units.Percent)] public int costHp;
    //[ShowIf("@change != 0 && cost == CostType.Gold")] public int costGold;

    [Title("Required")]
    public bool required;
    [ShowIf("@required")]public RewardType requiredType;
    [ShowIf("@required && requiredType == RewardType.Gold")]public int useGold;
    [ShowIf("@required && requiredType == RewardType.Skill")]public SkillAction useSkill;
    [ShowIf("@required && requiredType == RewardType.Relic")]public Relic useRelic;
    [ShowIf("@required && requiredType == RewardType.Hp"), Unit(Units.Percent)] public int useHp;

    [Title("Choice")]
    [ShowIf("@type == ChoiceType.Reward")] public RewardDetail reward;
    [ShowIf("@type == ChoiceType.Heal")] public HealDetail heal;
    [ShowIf("@type == ChoiceType.BuffDebuff")] public List<AddStatus> status;
    [ShowIf("@type == ChoiceType.Remove")] public RewardType removeType;
    [ShowIf("@type == ChoiceType.Remove && removeType == RewardType.Gold")] public int value;
    [ShowIf("@type == ChoiceType.Enemy")] public EnemyGroup enemyGroup;

    [Title("After")]
    public bool next;
    [ShowIf("@next")] public EventInfo nextEvent;
    [ShowIf("@!next"), PreviewField(50)] public Sprite afterEventImg;
    [ShowIf("@!next"), TextArea(2,4)]public string afterEventDes;

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
    Enemy,
    Change
}

public enum RewardType
{
    Gold,
    Skill,
    Relic,
    Hp
}

public enum CostType
{
    None,
    Hp,
    Gold,
    skill
}

