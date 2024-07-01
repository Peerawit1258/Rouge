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

    [TabGroup("Setting"), SerializeField] float time;
    // Start is called before the first frame update
    void Start()
    {
        upgradePos.gameObject.SetActive(false);
        upgradeCanvas.alpha = 0;
        upgradeCanvas.interactable = false;
        upgradeCanvas.blocksRaycasts = false;
    }

    bool isOpen;
    public void ActiveUpgradeUI()
    {
        if (!isOpen)
        {
            upgradePos.DOAnchorPosY(260, time).From().SetEase(Ease.InOutQuart);
            upgradeCanvas.DOFade(1, time).OnComplete(() =>
            {
                upgradeCanvas.interactable = true;
                upgradeCanvas.blocksRaycasts = true;
            });
        }
        else
        {

        }
        
    }

    public void UpgradePower(bool isSlot)
    {
        if (!isSlot)
        {

        }
        else
        {

        }
    }
}
