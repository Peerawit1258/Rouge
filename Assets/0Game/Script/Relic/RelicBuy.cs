using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class RelicBuy : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] RectTransform relicPos;
    [SerializeField] RelicWidget relicWidget;
    [SerializeField] CanvasGroup descriptionFade;
    [SerializeField] TMP_Text relicName;
    [SerializeField] TMP_Text relicDesc;
    [SerializeField] TMP_Text costText;
    [SerializeField] float time;

    int cost;
    RelicManagerSystem relicManagerSystem;
    // Start is called before the first frame update
    void Start()
    {
        relicManagerSystem = GameManager.instance.relicManagerSystem;

        descriptionFade.alpha = 0;
    }

    public void SetupMerchandise(Relic relic)
    {
        relicWidget.SetupRelic(relic);

        relicName.text = relic.relicName;
        relicDesc.text = relic.description;

        cost = relicWidget.GetPrice();
        costText.text = cost.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionFade.DOFade(1, time);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionFade.DOFade(0, time);
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void GetWidgettoInventory()
    {
        if (relicManagerSystem == null) relicManagerSystem = GameManager.instance.relicManagerSystem;
        relicWidget.GetWidgetPos().parent = GameManager.instance.detailPanel.GetRelicPlace();
        GameManager.instance.detailPanel.GetRelicWidgets().Insert(0, relicWidget);
        GameManager.instance.detailPanel.OrderRelic();
        relicManagerSystem.AddRelic(relicWidget.GetRelic());
        descriptionFade.DOFade(0, time).OnComplete(() => Destroy(gameObject));
        //canvasGroup.DOFade(0, 0.5f).OnComplete(() => Destroy(gameObject));
    }
}
