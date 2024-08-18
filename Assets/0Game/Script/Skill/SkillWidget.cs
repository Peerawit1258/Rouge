using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SkillWidget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] RectTransform widgetPos;
    [SerializeField] RectTransform iconFrame;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Image icon;
    [SerializeField] Image bgFrame;
    [SerializeField] public SkillNumSlot skillNumSlot;

    [ReadOnly] public SkillAction skill;
    [Header("Move")]
    [TabGroup("Setup"), SerializeField] float time;
    [TabGroup("Setup"), SerializeField] Ease ease;
    [Header("Shake")]
    [TabGroup("Setup"), SerializeField] float strength;
    [TabGroup("Setup"), SerializeField] int vibrato;
    [TabGroup("Setup"), SerializeField] float random;

    bool inSlot = false;
    List<int> specific;
    [ReadOnly] public SkillShow skillShow;
    SlotSkill slotSkill;
    SkillOrderSystem skillOrderSystem;
    // Start is called before the first frame update
    void Start()
    {
        skillOrderSystem = GameManager.instance.skillOrderSystem;
    }

    public void SetDetail(SkillAction skill)
    {
        this.skill = skill;
        icon.sprite = skill.skillIcon;
        bgFrame.sprite = skill.frame;
        skillNumSlot.SetNumber(skill.GetConditionDetail());
        if (skill.GetHaveCondition())
            specific = skill.GetLockNum();

        //skillDesc.SetDetailBox(skill);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (inSlot) return;
        iconFrame.DOAnchorPosY(50, time).SetEase(Ease.InOutQuart);
        iconFrame.DOScale(1.3f, time);

        GameManager.instance.skillDesc.SetDetailBox(skill);
        GameManager.instance.skillDesc.FadeIn(widgetPos, 250);
        //skillDesc.FadeIn();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (inSlot) return;
        if(skillOrderSystem.selectWidget != null)
            if (skillOrderSystem.selectWidget == this) return;
        iconFrame.DOAnchorPosY(0, time).SetEase(Ease.OutBounce);
        iconFrame.DOScale(1, time);

        GameManager.instance.skillDesc.FadeOut();
        //skillDesc.FadeOut();
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (skillOrderSystem.isOrder) return;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
        GameManager.instance.skillDesc.FadeOut();
        //skillDesc.FadeOut();
        if (slotSkill != null) ExitSlot();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (skillOrderSystem.isOrder) return;
        widgetPos.anchoredPosition += eventData.delta;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (skillOrderSystem.isOrder) return;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
        if (!inSlot)
        {
            skillOrderSystem.OrderDistanceSkill();
            if (!skillNumSlot.gameObject.activeSelf && skill.GetHaveCondition())
                skillNumSlot.gameObject.SetActive(true);
        }
        //else if(slotSkill != null) ExitSlot()
    }

        public void AddSkilltoSlotBar(SlotSkill slot,bool allEnemy = false)
    {

        slotSkill = slot;
        slotSkill.AssignSkill(this);
        skillNumSlot.gameObject.SetActive(false);
        widgetPos.parent = slotSkill.gameObject.transform;
        skillOrderSystem.currentSlot++;
        skillOrderSystem.GetAllSkillWidget().Remove(this);
        inSlot = true;
        skillOrderSystem.selectWidget = null;
        OnPointerExit(null);
        
        widgetPos.DOAnchorPos(Vector2.zero, 0.1f).SetEase(Ease.InOutQuart).OnComplete(() =>
        {
            skillOrderSystem.OrderDistanceSkill();
        });
    }

    public void ExitSlot()
    {
        widgetPos.parent = skillOrderSystem.skillPlace;
        if(slotSkill != null)
        {
            slotSkill.ClearData();
            slotSkill = null;
        }
        
        //skillOrderSystem.currentSlot--;
        skillOrderSystem.GetAllSkillWidget().Add(this);
        //skillOrderSystem.OrderDistanceSkill();
        inSlot = false;
    }

    public bool CheckCondition(int slot)
    {
        if (!skill.GetHaveCondition() || skill.GetConditonType() != ConditionType.SpecificOrder) return true;
        if(specific.Contains(slot))
            return true;

        iconFrame.DOShakeAnchorPos(0.2f, strength, vibrato, random, true).SetEase(Ease.InOutQuart).OnComplete(()=>
        {
            iconFrame.anchoredPosition = new Vector2(0, 50);
        });
        return false;
    }

    public RectTransform GetWidgetPos() => widgetPos;
    public bool GetInSlot() => inSlot;
    public void SetSlot(bool b) => inSlot = b;
}
