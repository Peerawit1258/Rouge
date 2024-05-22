using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AllData : MonoBehaviour
{
    [TabGroup("PlayerData")] public List<SkillAction> allSkill;
    [TabGroup("PlayerData")] public List<SkillAction> s;
    [TabGroup("PlayerData")] public List<Relic> allRelics;
    [TabGroup("PlayerData")] public List<Relic> r;

    [TabGroup("EnemyData")] public List<CharacterDetail> allEnemy;

    [TabGroup("GameData")] public List<StatusEffect> allStatus;

    private void Start()
    {
        if (GameManager.instance.isTest)
        {
            foreach(SkillAction action in s)
                GameManager.instance.playerData.currentSkills.Add(action);

            foreach(Relic relic in r)
                GameManager.instance.playerData.currentRelics.Add(relic);
        }
    }

    public AddStatus GetStatusWithID(string id)
    {
        AddStatus statusEffect = new AddStatus();

        foreach(var status in allStatus)
            if(status.id == id)
                statusEffect.statusEffect = status;
        statusEffect.count = 1;

        return statusEffect;
    }
}
