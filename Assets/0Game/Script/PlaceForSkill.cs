using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlaceForSkill : MonoBehaviour
{

    [SerializeField] SkillShow skill;
    public RectTransform pos;

    public void SetSkill(SkillShow show)
    {
        skill = show;
        show.GetObjPos().parent = gameObject.transform;
        show.GetObjPos().DOAnchorPos(new Vector2(50, 50), 0.1f).OnComplete(()=>show.ResetCurrentPos());
        show.inventory = true;
        GameManager.instance.playerData.AddNewSkill(show.GetSkillAction());
    }

    public void ClearValue() => skill = null;

    public SkillShow GetSkill() => skill;
}
