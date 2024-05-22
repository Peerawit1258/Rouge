using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class PlayerController : CharacterValue
{
    [Title("PlayerController")]

    [SerializeField] GameObject playerObj;
    public GaugeHpWidget gaugeHp;
    public AnimationAction animationAction;
    [SerializeField, ReadOnly] Vector3 playerPos;
    // Start is called before the first frame update
    void Start()
    {
        //hpValue = 90;
        playerPos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1))
            atkValue = 9999;
    }

    public float GetPlayerDef()
    {
        float currentDef = defValue + 
            ((defValue * GameManager.instance.relicManagerSystem.GetBonusStatFromHpCompare(hpValue/maxHpValue, StatType.Def))/100);
        return currentDef;
    }

    float ratio;
    public void StartDamageTaken(int damage, float delay = 0, bool notDie = false, GameObject effect = null) => StartCoroutine(DamageTaken(damage, delay, notDie, effect));

    IEnumerator DamageTaken(int damage, float delay, bool notDie, GameObject effect)
    {
        yield return new WaitForSeconds(delay);
        hpValue -= damage;
        if (effect != null) Instantiate(effect, gameObject.transform);
        yield return new WaitForSeconds(0.2f);
        GameManager.instance.numberDamageSystem.CreateDamageNumber(gameObject.transform, damage);
        if (hpValue <= 0)
        {
            if (notDie)
            {
                hpValue = 1;
                ratio = (float)hpValue / maxHpValue;
                gaugeHp.HpGaugeChange(ratio);

            }
            else
            {
                hpValue = 0;
                yield return new WaitForSeconds(0.2f);
                
                ratio = (float)hpValue / maxHpValue;
                gaugeHp.HpGaugeChange(ratio);
                yield return new WaitForSeconds(0.5f);
                //DestroySelf();
            }
        }
        else
        {
            ratio = (float)hpValue / maxHpValue;
            gaugeHp.HpGaugeChange(ratio);
        }
        

        yield return new WaitForSeconds(0.2f);
        
    }

    public void StartDamageFromDebuff(int damage, float delay, StatusEffect status, GameObject effect = null) => StartCoroutine(DamageFromDebuff(damage, delay, status, effect));
    IEnumerator DamageFromDebuff(int damage, float delay, StatusEffect status, GameObject effect)
    {
        yield return new WaitForSeconds(delay);

        hpValue -= damage;
        GameManager.instance.numberDamageSystem.CreateDamageNumber(gameObject.transform, damage, status);
        if (hpValue <= 0) hpValue = 0;
        if (effect != null) Instantiate(effect, gameObject.transform);

        yield return new WaitForSeconds(0.2f);
        float ratio = (float)hpValue / maxHpValue;
        gaugeHp.HpGaugeChange(ratio);
        //if (hpValue <= 0) DestroySelf();
    }

    public void StartHealHP(int value, float delay, GameObject effect = null) => StartCoroutine(HealHP(value, delay, effect));

    IEnumerator HealHP(int value, float delay, GameObject effect)
    {
        yield return new WaitForSeconds(delay);
        StatusWidget widget = new StatusWidget();
        if (gaugeHp.CheckStatuswithID("BN002", ref widget))
        {
            widget.DestroyStatus();
        }else if(gaugeHp.CheckStatuswithID("DN001", ref widget))
        {
            if(widget.GetCount() - value <= 0)
            {
                value -= widget.GetCount();
                widget.DestroyStatus();
                if (value == 0) yield break;
            }
            else
            {
                widget.DecreaseCount(value);
                yield break;
            }

        }

        if (hpValue + value > maxHpValue)
            value = (int)maxHpValue - hpValue;
        hpValue += value;

        if (effect != null) Instantiate(effect, gameObject.transform);

        yield return new WaitForSeconds(0.2f);
        GameManager.instance.numberDamageSystem.CreateHealNumber(gameObject.transform, value.ToString());

        float ratio = (float)hpValue / maxHpValue;
        gaugeHp.HpGaugeChange(ratio);

    }

    public List<StatusWidget> GetStatusWidgets() => gaugeHp.statusWidgets;
}
