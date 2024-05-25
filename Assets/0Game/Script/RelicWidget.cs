using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelicWidget : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private RectTransform widgetPos;
    [SerializeField] private CanvasGroup canvasGroup;

    Relic relic;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetupRelic(Relic relic)
    {
        icon.sprite = relic.icon;
        this.relic = relic;
    }

    public RectTransform GetWidgetPos() => widgetPos;
    public CanvasGroup GetCanvasGroup() => canvasGroup;
}
