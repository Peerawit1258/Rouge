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
    public List<string> unlockEvent;

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
    [Range(0, 100), ShowIf("@type != ChoiceType.Nothing")] public int rate;
    public bool exit;
    //[ShowIf("@change != 0")] public CostType cost;
    //[ShowIf("@change != 0 && cost == CostType.Hp"), Unit(Units.Percent)] public int costHp;
    //[ShowIf("@change != 0 && cost == CostType.Gold")] public int costGold;

    [TitleGroup("Required")] public bool required;
    [TitleGroup("Required"), ShowIf("@required && (requiredType == RewardType.Skill || requiredType == RewardType.Relic)")] public bool disappear;
    [TitleGroup("Required"), ShowIf("@required")]public RewardType requiredType;
    [TitleGroup("Required"), ShowIf("@required && requiredType == RewardType.Gold")]public int useGold;
    [TitleGroup("Required"), ShowIf("@required && requiredType == RewardType.Skill")]public SkillAction useSkill;
    [TitleGroup("Required"), ShowIf("@required && requiredType == RewardType.Relic")]public Relic useRelic;
    [TitleGroup("Required"), ShowIf("@required && requiredType == RewardType.Hp"), Unit(Units.Percent)] public int useHp;

    [TitleGroup("Choice"), ShowIf("@type == ChoiceType.Reward")] public RewardDetail reward;
    [TitleGroup("Choice"), ShowIf("@type == ChoiceType.Heal")] public HealDetail heal;
    [TitleGroup("Choice"), ShowIf("@type == ChoiceType.BuffDebuff")] public List<AddStatus> status;
    [TitleGroup("Choice"), ShowIf("@type == ChoiceType.Enemy")] public EnemyGroup enemyGroup;
    [TitleGroup("Choice"), ShowIf("@type == ChoiceType.BaseStat")] public StatValue statValue;

    [TitleGroup("After"), ShowIf("@rate == 0")] public bool next;
    [TitleGroup("After"), ShowIf("@next || rate > 0")] public EventInfo nextEvent;
    [TitleGroup("After"), PreviewField(50)] public Sprite afterEventImg;
    [TitleGroup("After"), TextArea(2,4)]public string afterEventDes;
    //[TitleGroup("After"), ShowIf("@rate > 0")] public EventInfo passEvent;
    [TitleGroup("After"), ShowIf("@rate > 0 && rate != 100")] public EventInfo failEvent;

}

[Serializable]
public class RewardDetail
{
    public RewardType rewardType;
    
    [ShowIf("@rewardType == RewardType.Gold")] public int min;
    [ShowIf("@rewardType == RewardType.Gold")] public int max;
    [ShowIf("@rewardType == RewardType.Skill")] public List<SkillAction> skills;
    [ShowIf("@rewardType == RewardType.Relic")] public List<Relic> relics;
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
    Enemy,
    BaseStat
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

