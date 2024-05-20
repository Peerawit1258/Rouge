using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class GaugeHpWidget : MonoBehaviour
{
    [SerializeField] Image hpGauge;
    [SerializeField] RectTransform statusPlace;

    [ReadOnly] public List<StatusWidget> statusWidgets;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void HpGaugeChange(float value)
    {
        hpGauge.DOFillAmount(value, 0.5f).SetEase(Ease.InOutQuart);
    }

    public bool CheckSameStatus(StatusEffect status)
    {
        if (status == null) return false;
        foreach (StatusWidget statusWidget in statusWidgets)
        {
            if (statusWidget.GetStatus().id == status.id)
                return true;
        }
        return false;
    }
    public List<StatusWidget> GetStatusWithTrigger(TriggerStatus trigger)
    {
        List<StatusWidget> list = new List<StatusWidget>();
        for(int i = 0; i < statusWidgets.Count; i++)
        {
            if(statusWidgets[i].GetTriggerStatus() == trigger)
                list.Add(statusWidgets[i]);
        }

        return list;

    }

    public StatusWidget GetStatuswithType(StatusType type, Effect effect = Effect.None)
    {
        if (statusWidgets.Count == 0) return null;
        foreach(StatusWidget widget in statusWidgets)
        {
            if (widget.GetStatus().type == type)
                if (type == StatusType.Debuff)
                    return widget;
                else
                {
                    if (effect == Effect.None)
                        return widget;
                    else
                        if(widget.GetStatus().effect == effect)
                            return widget;
                }
            
        }

        return null;
    }

    public StatusWidget GetStatuswithStatus(StatusEffect status)
    {
        if (statusWidgets.Count == 0) return null;
        foreach (StatusWidget widget in statusWidgets)
        {
            if (widget.GetStatus().id == status.id)
                return widget;
        }

        return null;
    }

    public bool CheckStatuswithID(string id, ref StatusWidget widget)
    {
        if (statusWidgets.Count == 0) return false;
        foreach (StatusWidget wid in statusWidgets)
        {
            if (wid.GetStatus().id == id)
            {
                widget = wid;
                return true;
            } 
        }

        return false;
    }

    public int GetAmountBuffDebuff(StatusType type)
    {
        int num = 0;
        foreach (StatusWidget widget in statusWidgets)
            if (widget.GetStatus().type == type)
                num++;
        return num;
    }


    public RectTransform GetStatusPlace() => statusPlace;
}
