using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using DG.Tweening;

public class UpgradeSystem : MonoBehaviour
{
    [TabGroup("Value"), SerializeField] List<int> slotPrice;
    [TabGroup("Value"), SerializeField] List<int> statPrice;
    [TabGroup("Value"), SerializeField] int hpIncrease;
    [TabGroup("Value"), SerializeField] int atkIncrease;
    [TabGroup("Value"), SerializeField] int defIncrease;
    [TabGroup("Rank"), SerializeField] int slotRank;
    [TabGroup("Rank"), SerializeField] int skillRank;
    [TabGroup("Rank"), SerializeField] int hpRank;
    [TabGroup("Rank"), SerializeField] int atkRank;
    [TabGroup("Rank"), SerializeField] int defRank;

    [TabGroup("UI"), SerializeField] CanvasGroup upgradeCanvas;
    [TabGroup("UI"), SerializeField] RectTransform upgradePos;
    [TabGroup("UI"), SerializeField] GameObject slotMax;
    [TabGroup("UI"), SerializeField] TMP_Text slotPriceText;
    [TabGroup("UI"), SerializeField] GameObject skillMax;
    [TabGroup("UI"), SerializeField] TMP_Text skillPriceText;
    [TabGroup("UI"), SerializeField] GameObject hpMax;
    [TabGroup("UI"), SerializeField] TMP_Text hpPriceText;
    [TabGroup("UI"), SerializeField] GameObject atkMax;
    [TabGroup("UI"), SerializeField] TMP_Text atkPriceText;
    [TabGroup("UI"), SerializeField] GameObject defMax;
    [TabGroup("UI"), SerializeField] TMP_Text defPriceText;

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
    [Button]
    public void ActiveUpgradeUI()
    {
        if (!isOpen)
        {
            isOpen = true;
            upgradePos.gameObject.SetActive(true);
            //priceText.text = requiredValue[rank].ToString();
            //if(GameManager.instance.playerData.gold < requiredValue[rank]) priceText.color = Color.red;
            //else priceText.color = Color.white;
            SetRequiredGoldUpgrade();

            upgradePos.DOAnchorPosY(245, time).SetEase(Ease.InOutQuart);
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

    private void SetRequiredGoldUpgrade()
    {
        if (GameManager.instance.skillOrderSystem.slotCount == 7)
        {
            slotMax.SetActive(true);
            slotPriceText.text = "0";
        }
        else
        {
            slotPriceText.text = slotPrice[slotRank].ToString();
            slotMax.SetActive(false);
            if (GameManager.instance.playerData.gold < slotPrice[slotRank])
                slotPriceText.color = Color.red;
            else
                slotPriceText.color = Color.white;
        }

        if (GameManager.instance.skillOrderSystem.skillCount + GameManager.instance.relicManagerSystem.randomSkill >= 10)
        {
            skillMax.SetActive(true);
            skillPriceText.text = "0";
        }
        else
        {
            skillPriceText.text = slotPrice[skillRank].ToString();
            skillMax.SetActive(false);
            if (GameManager.instance.playerData.gold < slotPrice[skillRank])
                skillPriceText.color = Color.red;
            else
                skillPriceText.color = Color.white;
        }

        if (hpRank > statPrice.Count - 1)
        {
            hpMax.SetActive(true);
            hpPriceText.text = "0";
        }
        else
        {
            hpPriceText.text = statPrice[hpRank].ToString();
            hpMax.SetActive(false);
            if (GameManager.instance.playerData.gold < statPrice[hpRank])
                hpPriceText.color = Color.red;
            else
                hpPriceText.color = Color.white;
        }

        if(atkRank > statPrice.Count - 1)
        {
            atkMax.SetActive(true);
            atkPriceText.text = "0";
        }
        else
        {
            atkPriceText.text = statPrice[atkRank].ToString();
            atkMax.SetActive(false);
            if(GameManager.instance.playerData.gold < statPrice[atkRank])
                atkPriceText.color = Color.red;
            else
                atkPriceText.color = Color.white;
        }

        if(defRank > statPrice.Count - 1)
        {
            defMax.SetActive(true);
            defPriceText.text = "0";
        }
        else
        {
            defPriceText.text = statPrice[defRank].ToString();
            defMax.SetActive(false);
            if(GameManager.instance.playerData.gold < statPrice[defRank])
                defPriceText.color = Color.red;
            else
                defPriceText.color = Color.white;
        }
    }

    private bool CheckGoldUpgrade(int current, int target)
    {
        if (current < target)
        {

            return false;
        }
        else
            return true;
    }

    StatValue stat = new StatValue();
    public void UpgradePower(int upgrade)
    {   
        switch(upgrade)
        {
            case 0://Slot
                if (slotMax.activeSelf || !CheckGoldUpgrade(GameManager.instance.playerData.gold, slotPrice[slotRank])) return;
                GameManager.instance.skillOrderSystem.slotCount++;
                GameManager.instance.detailPanel.ChangeGoldValue(GameManager.instance.playerData.gold - slotPrice[slotRank]);
                slotRank++;
                break;
            case 1://Hand
                if (skillMax.activeSelf || !CheckGoldUpgrade(GameManager.instance.playerData.gold, slotPrice[skillRank])) return;
                GameManager.instance.skillOrderSystem.skillCount++;
                GameManager.instance.detailPanel.ChangeGoldValue(GameManager.instance.playerData.gold - slotPrice[skillRank]);
                skillRank++;
                break;
            case 2://Hp
                if (hpMax.activeSelf || !CheckGoldUpgrade(GameManager.instance.playerData.gold, statPrice[hpRank])) return;
                stat.type = StatType.Hp;
                stat.value = hpIncrease;
                GameManager.instance.turnManager.player.BaseStatUpdate(stat);
                GameManager.instance.detailPanel.ChangeGoldValue(GameManager.instance.playerData.gold - statPrice[hpRank]);
                hpRank++;
                break;
            case 3://Atk
                if (atkMax.activeSelf || !CheckGoldUpgrade(GameManager.instance.playerData.gold, statPrice[atkRank])) return;
                stat.type = StatType.Atk;
                stat.value = atkIncrease;
                GameManager.instance.turnManager.player.BaseStatUpdate(stat);
                GameManager.instance.detailPanel.ChangeGoldValue(GameManager.instance.playerData.gold - statPrice[atkRank]);
                atkRank++;
                break;
            case 4://Def
                if (defMax.activeSelf || !CheckGoldUpgrade(GameManager.instance.playerData.gold, statPrice[defRank])) return;
                stat.type = StatType.Def;
                stat.value = defIncrease;
                GameManager.instance.turnManager.player.BaseStatUpdate(stat);
                GameManager.instance.detailPanel.ChangeGoldValue(GameManager.instance.playerData.gold - statPrice[defRank]);
                defRank++;
                break;
        }
        SetRequiredGoldUpgrade();
        //.instance.detailPanel.ChangeGoldValue(requiredValue[rank]);
        //++;
        //CloseButton();
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

public enum RestUpgrade
{
    Slot,
    Hand,
    Hp,
    Atk,
    Def
}
