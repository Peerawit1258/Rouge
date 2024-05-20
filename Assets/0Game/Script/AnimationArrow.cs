using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimationArrow : MonoBehaviour
{
    [SerializeField] private Transform arrow;
    // Start is called before the first frame update
   void Start()
    {

        //yield return new WaitForSeconds(1);
        Sequence s = DOTween.Sequence();
        s.Append(arrow.DOLocalMoveY(0.2f, 1));
        //s.Insert(0,arrow.DOLocalMoveY(-0.1f, 2));
        //s.Insert(2,arrow.DOLocalMoveY(0, 1));
        s.SetLoops(-1, LoopType.Yoyo);
    }

}
