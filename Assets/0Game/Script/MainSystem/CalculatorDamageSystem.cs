using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CalculatorDamageSystem : MonoBehaviour
{
    [TabGroup("1","Attacker"),SerializeField] int atk;
    [TabGroup("1", "Attacker"), SerializeField] List<int> atkBonus;
    [TabGroup("1", "Attacker"), SerializeField] List<int> percentSkill;
    [TabGroup("1", "Attacker"), SerializeField] List<int> damageBonus;

    [TabGroup("2", "Defender"), SerializeField] int def;
    [TabGroup("2", "Defender"), SerializeField] List<int> defBonus;
    [TabGroup("2", "Defender"), SerializeField] List<int> damageReduce;

    [Title("Result")]
    [TextArea(4,20), SerializeField] string result = "";

    [ButtonGroup, GUIColor("blue")]
    public void Calculate()
    {
        result = "";

        int increaseAtk = 0;
        foreach (int atkB in atkBonus)
        {
            increaseAtk += atk * atkB / 100;
        }
        result += "Atk : " + (increaseAtk + atk).ToString() + "\n";
        result += "DamageBonus : " + TotalDamageBonus() + "%\n";

        int increaseDef = 0;
        foreach (int defB in defBonus)
        {
            increaseDef += def * defB/100;
        }
        result += "Def : " + (increaseDef + def).ToString() + "\n";
        result += "DamageReduce : " + TotalDamageReduce() + "%\n";

        for(int i = 0; i < percentSkill.Count; i++)
        {
            int damage = DamageResult(increaseAtk + atk, percentSkill[i],
                            increaseDef + def, TotalDamageBonus(), TotalDamageReduce());

            result += "\nHit" + (i + 1) + ": " + damage.ToString();
        }
        
    }
    [ButtonGroup, GUIColor("red")]
    public void Clear() => result = "";

    [Button]
    public void Change(int value)
    {
        if (value == 0) return;
        int a = 0;
        int b = 0;
        for(int i = 0; i < 100; i++)
        {
            int target = Random.Range(0, 100);
            if (target < value) a++;
            else b++;
        }

        Debug.Log(a + ":" + b);
    }

    [Button]
    public void CheckChange(int value)
    {
        if(value == 0) return;
        int a = 0;
        int b = 0;
        for (int i = 0; i < 100; i++)
        {
            if (i % (100 / value) == 0) a++;
            else b++;
        }

        Debug.Log(a + ":" + b);
    }

    int TotalDamageBonus()
    {
        int total = 0;
        if(damageBonus.Count == 0) return total;
        foreach(int bonus in damageBonus)
        {
            total += bonus;
        }

        return total;
    }

    int TotalDamageReduce()
    {
        int total = 0;
        if (damageReduce.Count == 0) return total;
        foreach (int reduce in damageReduce)
        {
            total += reduce;
        }

        return total;
    }

    public int DamageResult(float atk, int skillDMG, int targetDef, float damageBonus = 0, float targetDMGRe = 0)
    {
        int damage = (int)Mathf.Floor((((atk * skillDMG / 100) * ((100 + damageBonus) / 100)) - targetDef) * (100 - targetDMGRe) / 100);
        //if (damage <= 0) damage = 1;
        //Debug.Log(damage);
        return damage;
    }

}
