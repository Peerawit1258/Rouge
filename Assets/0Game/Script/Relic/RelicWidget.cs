using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelicWidget : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private RectTransform widgetPos;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] public RelicInfo relicInfo;

    Relic relic;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetupRelic(Relic relic)
    {
        if(relic.icon != null) icon.sprite = relic.icon;
        this.relic = relic;
        gameObject.name = relic.relicName + "_Relic";
    }

    public void SetRelicInfo(RelicInfo info)=> relicInfo = info;
    //public RelicInfo GetInfo()=> relicInfo;

    public RectTransform GetWidgetPos() => widgetPos;
    public CanvasGroup GetCanvasGroup() => canvasGroup;
    public Relic GetRelic() => relic;

    public int GetPrice()
    {
        int price = 0;
        switch (relic.rarity)
        {
            case Rarity.Common: price = GameManager.instance.shopSystem.relicPrice[0]; break;
            case Rarity.Epic: price = GameManager.instance.shopSystem.relicPrice[1]; break;
            case Rarity.Legendary: price = GameManager.instance.shopSystem.relicPrice[2]; break;
        }
        return price;
    }
}
