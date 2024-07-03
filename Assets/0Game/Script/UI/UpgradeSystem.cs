using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using DG.Tweening;

public class UpgradeSystem : MonoBehaviour
{
    [TabGroup("Value"), SerializeField] List<int> requiredValue;

    [TabGroup("UI"), SerializeField] CanvasGroup upgradeCanvas;
    [TabGroup("UI"), SerializeField] RectTransform upgradePos;
    [TabGroup("UI"), SerializeField] TMP_Text priceText;
    [TabGroup("UI"), SerializeField] GameObject slotMax;
    [TabGroup("UI"), SerializeField] GameObject skillMax;

    [TabGroup("Setting"), SerializeField] float time;
    // Start is called before the first frame update
    void Start()
    {
        upgradePos.gameObject.SetActive(false);
        upgradePos.anchoredPosition = new Vector2(0, 260);
        upgradeCanvas.alpha = 0;
        upgradeCanvas.interactable = false;
        upgradeCanvas.blocksRaycasts = false;
    }

    bool isOpen;
    int rank = 0;
    public void ActiveUpgradeUI()
    {
        if (!isOpen)
        {
            isOpen = true;
            upgradePos.gameObject.SetActive(true);
            priceText.text = requiredValue[rank].ToString();
            if(GameManager.instance.playerData.gold < requiredValue[rank]) priceText.color = Color.red;
            else priceText.color = Color.white;

            if (GameManager.instance.skillOrderSystem.slotCount == 7) slotMax.SetActive(true);
            else slotMax.SetActive(false);

            if (GameManager.instance.skillOrderSystem.skillCount == 10) skillMax.SetActive(true);
            else skillMax.SetActive(false);

            upgradePos.DOAnchorPosY(180, time).SetEase(Ease.InOutQuart);
            upgradeCanvas.DOFade(1, time).OnComplete(() =>
            {
                upgradeCanvas.interactable = true;
                upgradeCanvas.blocksRaycasts = true;
            });
        }
        else
        {
            isOpen = false;
            upgradePos.DOAnchorPosY(260, time).SetEase(Ease.InOutQuart);
            upgradeCanvas.DOFade(0, time).OnComplete(() =>
            {
                upgradeCanvas.interactable = false;
                upgradeCanvas.blocksRaycasts = false;
                upgradePos.gameObject.SetActive(false);
            });
        }
    }

    public void UpgradePower(bool isSlot)
    {   
        if (!isSlot)
        {
            if (skillMax.activeSelf) return;
            GameManager.instance.skillOrderSystem.skillCount++;
            if (GameManager.instance.skillOrderSystem.skillCount == 10)
                Debug.Log("sd");
        }
        else
        {
            if (slotMax.activeSelf) return;
            GameManager.instance.skillOrderSystem.slotCount++;
            if (GameManager.instance.skillOrderSystem.slotCount == 7)
                Debug.Log("sd");
        }
        GameManager.instance.detailPanel.ChangeGoldValue(requiredValue[rank]);
        rank++;
        CloseButton();
    }

    public void CloseButton()
    {
        ActiveUpgradeUI();
        GameManager.instance.encounterManagementSystem.CreateNextDoorNode();
    }

    public void HoverEnter(RectTransform imgPos)
    {
        imgPos.DOScale(1.2f, 0.3f);
    }

    public void HoverExit(RectTransform imgPos)
    {
        imgPos.DOScale(1, 0.3f);
    }
}
