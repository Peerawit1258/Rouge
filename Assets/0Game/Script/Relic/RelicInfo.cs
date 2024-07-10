using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicInfo : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text relicName;
    [SerializeField] TMP_Text relicDesc;
    [SerializeField] Color curse;

    public void AssignInfo(Relic relic)
    {
        icon.sprite = relic.icon;
        relicName.text = relic.relicName;
        if (relic.relicType == RelicType.Curse) relicName.color = curse;
        relicDesc.text = relic.description;
    }
}
