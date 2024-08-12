using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using TMPro;

public class SkillShow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    [ReadOnly, SerializeField] string id;
    [SerializeField] public CanvasGroup skillCanvas;
    [SerializeField] CanvasGroup descriptionFade;
    [SerializeField] RectTransform objPos;
    [SerializeField] RectTransform skillPos;
    [SerializeField] Image frame;
    [SerializeField] Image icon;
    [SerializeField] Image removeIcon;
    [SerializeField] GameObject cooldownObj;
    [SerializeField] TMP_Text cooldownText;
    [SerializeField] SkillDesc skillDesc;

    SkillAction skillAction;
    InventoryManager inventoryManager;
    [ReadOnly] public bool inventory = false;
    bool interact = false;
    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = GameManager.instance.inventoryManager;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (inventory)
        {
            if(inventoryManager.removeCount > 0)
                removeIcon.DOFade(1, 0.2f);
            
        }

        GameManager.instance.skillDesc.SetDetailBox(skillAction);
        GameManager.instance.skillDesc.FadeIn(objPos, 175);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inventory)
        {
            if (inventoryManager.removeCount > 0)
                removeIcon.DOFade(0, 0.2f);
        }
        GameManager.instance.skillDesc.FadeOut();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (inventory)
        {
            if (inventoryManager.removeCount > 0)
            {
                Debug.Log("Remove");
            }
        }
        else
        {
            if (!GameManager.instance.resultBattle.getReward) return;
            GameManager.instance.inventoryManager.GetSkillShows().Add(this);
            if(GameManager.instance.resultBattle.GetSkillShows().Contains(this))
                GameManager.instance.resultBattle.GetSkillShows().Remove(this);
            objPos.parent = GameManager.instance.inventoryManager.iconPos;

            if (GameManager.instance.inventoryManager.GetSkillShows().Count <= 20)
            {
                objPos.DOAnchorPos(Vector2.zero, 1).SetEase(Ease.OutQuart);
                objPos.DOScale(1, 1);
                objPos.DOSizeDelta(Vector2.zero, 1).SetEase(Ease.OutQuart).OnComplete(() =>
                {
                    objPos.sizeDelta = new Vector2(100, 100);
                    GameManager.instance.inventoryManager.SkillMoveToPlace(this);
                    GameManager.instance.inventoryManager.recieveCount--;
                    

                    if (GameManager.instance.resultBattle.result)
                    {
                        if ((GameManager.instance.inventoryManager.recieveCount == 0 && 
                        GameManager.instance.inventoryManager.removeCount == 0) || 
                        GameManager.instance.resultBattle.GetSkillShows().Count == 0)
                            GameManager.instance.resultBattle.CloseResultPanel();
                    }
                    else
                    {
                        if (GameManager.instance.inventoryManager.recieveCount == 0 && GameManager.instance.inventoryManager.removeCount == 0)
                        {

                        }
                    }
                });
            }
        }
    }

    Vector2 currentPos;
    public void OnBeginDrag(PointerEventData eventData)
    {
        //if (inventory) return;
        //skillCanvas.blocksRaycasts = false;
        //currentPos = objPos.anchoredPosition;
        ////objPos.parent = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //if (inventory) return;
        //objPos.anchoredPosition += eventData.delta;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        //skillCanvas.blocksRaycasts = true;
        //if (inventory) return;
        //if (!inventory)
        //{
        //    objPos.parent = GameManager.instance.resultBattle.GetSkillPlace();
        //    objPos.DOAnchorPos(currentPos, 0.1f);
        //}
        //else
        //{
        //    objPos.DOAnchorPos(currentPos, 0.1f);
        //}
    }

    public void SetSkillShowAnimation(SkillAction skill, float delay)
    {
        interact = false;
        frame.sprite = skill.frame;
        icon.sprite = skill.skillIcon;

        skillAction = skill;
        id = skill.id;
        name = skill.name;

        delay *= 0.7f;
        skillPos.DOAnchorPosX(100, 1.5f).From().SetEase(Ease.OutQuart).SetDelay(delay).OnComplete(()=> skillCanvas.blocksRaycasts = true);
        skillCanvas.DOFade(0, 1f).From().SetDelay(delay);
        gameObject.name = skill.skillName + "_Show";

        skillDesc.SetDetailBox(skill);
    }

    public void SetSkillShow(SkillAction skill, bool isInventory = true)
    {
        frame.sprite = skill.frame;
        icon.sprite = skill.skillIcon;

        id = skill.id;
        name = skill.name;
        skillAction = skill;
        inventory = isInventory;

        skillDesc.SetDetailBox(skill);
    }

    int cooldownValue;
    public void SetCoolDown()
    {
        if (skillAction.cooldown == 0)
        {
            inventoryManager.GetSkillUsable().Remove(this);
            inventoryManager.GetSkillActive().Add(this);
            return;
        }

        inventoryManager.GetSkillUsable().Remove(this);
        inventoryManager.GetSkillCoolDown().Add(this);
        cooldownObj.SetActive(true);
        cooldownValue = skillAction.cooldown + 1;
        cooldownText.text = (cooldownValue - 1).ToString();
    }

    public void DecreaseCooldown(int value)
    {
        cooldownValue--;
        cooldownText.text = cooldownValue.ToString();
        if(cooldownValue <= 0)
            ReadyToActionSkill();
    }

    public void ReadyToActionSkill()
    {
        if (cooldownValue > 0) cooldownValue = 0;
        inventoryManager.GetSkillCoolDown().Remove(this);
        inventoryManager.GetSkillActive().Add(this);

        cooldownObj.SetActive(false);
    }

    public void ResetCurrentPos() => currentPos = objPos.anchoredPosition;
    public RectTransform GetObjPos() => objPos;
    public string GetId() => id;
    public SkillAction GetSkillAction() => skillAction;
    public int GetPrice()
    {
        int price = 0;
        switch (skillAction.rarity)
        {
            case Rarity.Common: price = GameManager.instance.shopSystem.skillPrice[0]; break;
            case Rarity.Epic: price = GameManager.instance.shopSystem.skillPrice[1]; break;
            case Rarity.Legendary: price = GameManager.instance.shopSystem.skillPrice[2]; break;
        }
        return price;
    }
}
