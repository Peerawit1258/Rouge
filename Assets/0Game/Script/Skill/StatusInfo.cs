using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusInfo : MonoBehaviour
{
    [SerializeField] TMP_Text statusName;
    [SerializeField] TMP_Text statusDescription;
    [SerializeField] RectTransform infoPos;
    
    public void SetInfo(StatusEffect status)
    {
        statusName.text = status.statusName;
        statusDescription.text = status.descrition;

        if(statusDescription.preferredHeight > 51)
            infoPos.sizeDelta = new Vector2(infoPos.sizeDelta.x, 80 + (statusDescription.preferredHeight - 51));
    }
}
