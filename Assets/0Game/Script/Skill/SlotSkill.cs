using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Sirenix.OdinInspector;
using DG.Tweening;

public class SlotSkill : MonoBehaviour ,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler, IDropHandler
{
    [ReadOnly] public int numSlot;
    [ReadOnly] public GameObject skillWidgetOBJ;
    [ReadOnly] public SkillAction skill;
    //[ReadOnly] public List<EnemyController> enemies;
    [ReadOnly, SerializeField] int damageBonus;
    [ReadOnly] public bool sameConditon = false;

    [SerializeField] TMP_Text percentText;
    SkillOrderSystem skillOrderSystem;
    SkillWidget skillWidget;
    // Start is called before the first frame update
    void Start()
    {
        skillOrderSystem = GameManager.instance.skillOrderSystem;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }
    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if (skill != null) return;
        //if(skillOrderSystem.selectWidget != null)
        //{
        
        //}
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            SkillWidget skill = eventData.pointerDrag.GetComponent<SkillWidget>();
            //if (skill.skill.targetType == TargetType.SingleTarget)
            //{
            //    //if(GameManager.instance.turnManager.targetEnemy == null)

            //}
            //else if (skill.skill.targetType == TargetType.AllTarget || skillOrderSystem.selectWidget.skill.targetType == TargetType.Random)
            //    skillOrderSystem.selectWidget.AddSkilltoSlotBar(this, true);
            //else
            //    skillOrderSystem.selectWidget.AddSkilltoSlotBar(this);
            if (skill.CheckCondition(numSlot))
                skill.AddSkilltoSlotBar(this);
            else
                skillOrderSystem.OrderDistanceSkill();
        }
    }

    public void AssignSkill(SkillWidget widget)
    {
        skillWidgetOBJ = widget.gameObject;
        skill = widget.skill;
        skillWidget = widget;
        if (skill.GetHaveCondition() && skill.GetLockNum().Contains(numSlot) )
        {
            if (skill.GetConditonType() == ConditionType.SlotDamageOrder)
            {
                foreach (int num in skill.GetSpecificSlotBonus())
                {
                    if (skill.GetConditionBonusDamage() >= 0)
                        skillOrderSystem.GetAllSlot()[num-1].IncreaseValueBonusDMG(skill.GetConditionBonusDamage());
                    else
                        skillOrderSystem.GetAllSlot()[num-1].DecreaseValueBonusDMG(skill.GetConditionBonusDamage());
                }
                
            }
                
            sameConditon = true;
        }
            
                
    }

    public void ClearData(bool isDestroy = false)
    {
        if(isDestroy) Destroy(skillWidgetOBJ);
        skillWidgetOBJ = null;
        if(skill != null)
        {
            if (skill.GetHaveCondition() && skill.GetConditonType() == ConditionType.SlotDamageOrder)
            {
                foreach (int num in skill.GetSpecificSlotBonus())
                {
                    if (skill.GetConditionBonusDamage() >= 0)
                        skillOrderSystem.GetAllSlot()[num - 1].DecreaseValueBonusDMG(skill.GetConditionBonusDamage());
                    else
                        skillOrderSystem.GetAllSlot()[num - 1].IncreaseValueBonusDMG(skill.GetConditionBonusDamage());
                }
            }
            skill = null;
        }
        //damageBonus = 0;
        sameConditon = false;
    }

    public int GetBonusDMG()
    {
        if(damageBonus <= 0) return 0;
        return damageBonus;
    }

    Tween perTween;
    int current, target;
    public void IncreaseValueBonusDMG(int value)
    {
        if (perTween != null) perTween.Kill();
        current = damageBonus;
        target = damageBonus + value;
        damageBonus += value;
        
        perTween = DOVirtual.Int(current, target, 0.5f, (x) =>
        {
            if (x > 0)
                percentText.text = x.ToString() + "%";
            else
                percentText.text = "0%";
        });

        
    }
    public void DecreaseValueBonusDMG(int value)
    {
        if (perTween != null) perTween.Kill();
        current = damageBonus;
        target = damageBonus - value;
        damageBonus -= value;

        perTween = DOVirtual.Int(current, target, 0.5f, (x) =>
        {
            if (x > 0)
                percentText.text = x.ToString() + "%";
            else
                percentText.text = "0%";
        });
    }

    public SkillWidget GetSkillWidget() => skillWidget;
}
