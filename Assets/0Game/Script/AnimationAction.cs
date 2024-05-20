using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class AnimationAction : MonoBehaviour
{
    [SerializeField] Animator animator;
    [TabGroup("Attack"), SerializeField] float attackPos;
    [TabGroup("Attack")] public float attackTime;
    [TabGroup("Attack"), SerializeField] Ease attackEase;

    [TabGroup("Buff"), SerializeField] Ease buffEase;
    [TabGroup("Buff"), SerializeField] Vector3 buffPos;
    [TabGroup("Buff"), SerializeField] float power;
    [TabGroup("Buff"), SerializeField] int numJump;
    [TabGroup("Buff")] public float buffTime;

    public bool isAction;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    [ButtonGroup, GUIColor("red")]
    public void AttackAction()
    {
        isAction = true;
        gameObject.transform.DOLocalMoveX(attackPos, attackTime).SetEase(attackEase).OnComplete(() =>
        {
            gameObject.transform.DOLocalMoveX(0, attackTime).SetEase(attackEase);
            isAction = false;
        });
    }
    [ButtonGroup, GUIColor("green")]
    public void BuffAction()
    {
        isAction = true;
        gameObject.transform.DOLocalJump(buffPos, power, numJump, buffTime).SetEase(buffEase).OnComplete(() =>
        {
            isAction = false;
        });
    }

    [ButtonGroup]
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
