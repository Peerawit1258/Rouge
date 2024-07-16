using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using TMPro;

public class NumberDamageSystem : MonoBehaviour
{
    public GameObject damageNumber;
    public GameObject statusName;
    public RectTransform damagePlace;

    [TabGroup("Color"), SerializeField] Color damage;
    [TabGroup("Color"), SerializeField] Color buff;
    [TabGroup("Color"), SerializeField] Color debuff;
    [TabGroup("Color"), SerializeField] Color heal;
    [Header("Dot")]
    [TabGroup("Color"), SerializeField] Color posion;
    [TabGroup("Color"), SerializeField] Color burn;
    [TabGroup("Color"), SerializeField] Color bleed;
    [TabGroup("Color"), SerializeField] Color curse;

    [Button]
    public void CreateDamageNumber(Transform pos, int info, StatusEffect status = null) //number only
    {
        TMP_Text damageText = Instantiate(damageNumber, pos.position, Quaternion.identity).GetComponent<TMP_Text>();
        damageText.transform.parent = damagePlace;
        damageText.text = info.ToString();
        if(status != null)
        {
            Debug.Log(status.id);
            //switch (status.effect)
            //{
            //    case Effect.DOT:
            //        if (status.statusName == "Posion")
            //            damageText.color = posion;
            //        else if (status.statusName == "Burn")
            //            damageText.color = burn;
            //        else if (status.statusName == "Bleed")
            //            damageText.color = bleed;
            //        else if (status.statusName == "Curse")
            //            damageText.color = curse;
            //        break;
            //    default:
            //        damageText.color = damage;
            //        break;
            //}
            damageText.color = status.color;
        }
        

        if(pos.name.Contains("Player"))
            damageText.transform.DOLocalMove(new Vector2(-0.1f,0.4f), 0.8f).SetRelative().SetEase(Ease.InOutQuart);
        else
            damageText.transform.DOLocalMove(new Vector2(0.1f, 0.4f), 0.8f).SetRelative().SetEase(Ease.InOutQuart);
        damageText.DOFade(0, 0.4f).SetDelay(0.4f);
    }

    public void CreateStatusText(Transform pos, StatusEffect status) //status only
    {
        TMP_Text statusText = Instantiate(statusName, pos.position, Quaternion.identity).GetComponent<TMP_Text>();
        statusText.transform.parent = damagePlace;
        statusText.text = status.statusName;
        //switch (status.effect)
        //{
        //    case Effect.DOT:
        //        if (status.statusName == "Posion")
        //            statusText.color = posion;
        //        else if (status.statusName == "Burn")
        //            statusText.color = burn;
        //        else if (status.statusName == "Bleed")
        //            statusText.color = bleed;
        //        else if (status.statusName == "Curse")
        //            statusText.color = curse;
        //        break;
        //    case Effect.Stat:
        //        statusText.color = buff;
        //        break;
        //    case Effect.Regen:
        //        statusText.color = heal;
        //        break;
        //}
        statusText.color = status.color;

        statusText.transform.DOLocalMoveY(0.4f, 0.8f).SetRelative().SetEase(Ease.InOutQuart);
        statusText.DOFade(0, 0.4f).SetDelay(0.4f);
    }

    public void CreateHealNumber(Transform pos, string info) //number only
    {
        TMP_Text damageText = Instantiate(damageNumber, pos.position, Quaternion.identity).GetComponent<TMP_Text>();
        damageText.transform.parent = damagePlace;
        damageText.text = "+" + info;
        damageText.color = heal;

        damageText.transform.DOLocalMoveY(0.4f, 0.8f).SetRelative().SetEase(Ease.InOutQuart);
        damageText.DOFade(0, 0.4f).SetDelay(0.4f);
    }

    public void CreateFixText(Transform pos, string text) //status only
    {
        TMP_Text txt = Instantiate(statusName, pos.position, Quaternion.identity).GetComponent<TMP_Text>();
        txt.transform.parent = damagePlace;
        txt.text = text;

        txt.color = buff;

        txt.transform.DOLocalMoveY(0.4f, 0.8f).SetRelative().SetEase(Ease.InOutQuart);
        txt.DOFade(0, 0.4f).SetDelay(0.4f);
    }
}
