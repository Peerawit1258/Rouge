using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class SkillShow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    [ReadOnly, SerializeField] string id;
    [SerializeField] public CanvasGroup skillCanvas;
    [SerializeField] RectTransform objPos;
    [SerializeField] RectTransform skillPos;
    [SerializeField] Image frame;
    [SerializeField] Image icon;
    [SerializeField] Image removeIcon;
    
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
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inventory)
        {
            removeIcon.DOFade(0, 0.2f);
        }
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
    }

    Vector2 currentPos;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventory) return;
        skillCanvas.blocksRaycasts = false;
        currentPos = objPos.anchoredPosition;
        //objPos.parent = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (inventory) return;
        objPos.anchoredPosition += eventData.delta;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        skillCanvas.blocksRaycasts = true;
        if (inventory) return;
        if (!inventory)
        {
            objPos.parent = GameManager.instance.resultBattle.GetSkillPlace();
            objPos.DOAnchorPos(currentPos, 0.1f);
        }
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
    }

    public void SetSkillShow(SkillAction skill, bool isInventory = true)
    {
        frame.sprite = skill.frame;
        icon.sprite = skill.skillIcon;

        id = skill.id;
        name = skill.name;
        skillAction = skill;
        inventory = isInventory;
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
