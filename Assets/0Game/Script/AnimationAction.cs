using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Sirenix.OdinInspector;

public class AnimationAction : MonoBehaviour
{
    public Animator animator;
    [SerializeField] SpriteRenderer character;
    [TabGroup("Attack"), SerializeField] float attackPos;
    [TabGroup("Attack")] public float attackTime;
    [TabGroup("Attack"), SerializeField] Ease attackEase;

    [TabGroup("Buff"), SerializeField] Ease buffEase;
    [TabGroup("Buff"), SerializeField] Vector3 buffPos;
    [TabGroup("Buff"), SerializeField] float power;
    [TabGroup("Buff"), SerializeField] int numJump;
    [TabGroup("Buff")] public float buffTime;

    [TabGroup("Die")] public float dieTime;

    [TabGroup("TakeDamage")] public float takeTime;
    [TabGroup("TakeDamage")] public float takePos;
    [TabGroup("TakeDamage")] public Ease takeEase;

    public bool isAction;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentsInChildren<Animator>()[0];
        character = GetComponentsInChildren<SpriteRenderer>()[0];
    }
    [ButtonGroup("1"), GUIColor("red")]
    public void AttackAction()
    {
        isAction = true;
        gameObject.transform.DOLocalMoveX(attackPos, attackTime).SetEase(attackEase).OnComplete(() =>
        {
            gameObject.transform.DOLocalMoveX(0, attackTime).SetEase(attackEase);
            isAction = false;
        });
    }
    [ButtonGroup("1"), GUIColor("green")]
    public void BuffAction()
    {
        isAction = true;
        gameObject.transform.DOLocalJump(buffPos, power, numJump, buffTime).SetEase(buffEase).OnComplete(() =>
        {
            isAction = false;
        });
    }
    [ButtonGroup("2"), GUIColor("grey")]
    public void DieAction(UnityAction action = null)
    {
        character.DOColor(Color.red, dieTime);
        character.DOFade(0, dieTime/2).SetDelay(dieTime).OnComplete(() =>
        {
            if(action != null)
                action.Invoke();
        });
    }
    Tween tween;
    [ButtonGroup("2"), GUIColor("yellow")]
    public void TakeDamageAction()
    {
        if(tween != null) tween.Kill();
        isAction = true;
        tween = gameObject.transform.DOLocalMoveX(takePos, takeTime).SetEase(takeEase).OnComplete(() =>
        {
            gameObject.transform.DOLocalMoveX(0, takeTime).SetEase(takeEase);
            isAction = false;
        });
    }

    [ButtonGroup("3")]
    public void WalkInSceneAction()
    {
        isAction = true;
        animator.SetTrigger("Run");
        gameObject.transform.DOLocalMoveX(-1, 1.2f).From().SetEase(Ease.Linear).OnComplete(() =>
        {
            animator.SetTrigger("Idle");
            isAction = false;
        });
    }
}
