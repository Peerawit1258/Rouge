using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

public class SkillNumSlot : MonoBehaviour
{
    [TabGroup("Sprite"), SerializeField] Sprite lockImg;
    [TabGroup("Sprite"), SerializeField] Sprite bonusImg;

    [SerializeField] Image iconImg;
    [SerializeField] TMP_Text numText;

    public void SetNumber(ConditionDetail condition)
    {
        if (condition.isCondition)
        {
            switch (condition.condition)
            {
                case ConditionType.SpecificOrder:
                    break;
                case ConditionType.SlotDamageOrder:
                    break;
                case ConditionType.EffectOrder:
                    break;
            }

            foreach (int num in condition.lockNum)
                numText.text += num + " ";
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
