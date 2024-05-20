using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllData : MonoBehaviour
{
    public List<SkillAction> allSkill;
    public List<SkillAction> s;
    public List<Relic> allRelics;
    public List<Relic> r;

    public List<StatusEffect> allStatus;

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
