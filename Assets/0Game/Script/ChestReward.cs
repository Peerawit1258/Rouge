using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class ChestReward : MonoBehaviour
{
    [SerializeField] int spawnRate;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer sprite;
    [TabGroup("Detail"), SerializeField] CharacterDetail mimicDetail;
    [TabGroup("Detail"), SerializeField] Transform gaugePos;
    [TabGroup("Drop"), SerializeField] List<SkillAction> skills;
    [TabGroup("Drop"), SerializeField] List<Relic> relics;
    [TabGroup("Setup"), SerializeField] float duration;
    [TabGroup("Setup"), SerializeField] float strength;
    [TabGroup("Setup"), SerializeField] int vibrato;
    [TabGroup("Setup"), SerializeField] float random;

    bool isMimic;
    bool interact;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnMouseEnter()
    {
        if (interact) return;
        GameManager.instance.battleSetup.arrow.gameObject.SetActive(true);
        GameManager.instance.battleSetup.arrow.position = gameObject.transform.position + new Vector3(0, 0.45f, 0);
    }

    public void OnMouseExit()
    {
        if (interact) return;
        GameManager.instance.battleSetup.arrow.gameObject.SetActive(false);
    }

    public void OnMouseUp()
    {
        if (isMimic || interact) return;
        interact = true;
        GameManager.instance.battleSetup.arrow.gameObject.SetActive(false);
        if (SpawnMimic())
        {
            isMimic = true;
            StartCoroutine(MimicSetup());
        }
        else
        {
            StartCoroutine(RewardChest());
        }
    }

    private bool SpawnMimic()
    {
        if (GameManager.instance.encounterManagementSystem.stageName == "The Cave" || spawnRate == 0) return false;
        int num = Random.Range(0, 100);
        Debug.Log("Random : " + num);
        
        if(num < spawnRate)
            return true;

        return false;
    }

    IEnumerator RewardChest()
    {
        animator.SetTrigger("Open");
        
        yield return new WaitForSeconds(0.25f);
        sprite.DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.25f);
        GameManager.instance.resultBattle.StartRewardPanel();
        Destroy(gameObject);
    }

    IEnumerator MimicSetup()
    {
        transform.DOShakePosition(duration, strength, vibrato, random).OnComplete(() => {
            transform.DOShakePosition(duration, strength, vibrato, random).OnComplete(() => transform.DOLocalMove(new Vector2(0, -0.136f), 0.1f) );
            });
        yield return new WaitForSeconds(1.5f);

        GetComponent<BoxCollider2D>().enabled = false;
        animator.SetTrigger("Mimic");
        transform.DOLocalJump(new Vector2(0, -0.136f), 0.2f, 1, 0.67f).SetEase(Ease.InOutQuart);

        yield return new WaitForSeconds(0.7f);
        EnemyController mimic = gameObject.AddComponent<EnemyController>(); ;
        //mimic.transform.localPosition = Vector3.zero;
        mimic.SetInfoEnemy(mimicDetail);
        mimic.SetDrop(mimicDetail.skillDrop, mimicDetail.relicDrop, mimicDetail.goldDrop);
        mimic.name = mimicDetail.characterName;
        mimic.animationAction = GetComponent<AnimationAction>();
        mimic.SetGaugePos(gaugePos);

        //yield return new WaitUntil(() => mimic != null);
        GameManager.instance.gaugeHpEnemy.SetPositionGauge(mimic);
        GameManager.instance.turnManager.enemies.Add(mimic);

        yield return new WaitForSeconds(0.7f);
        GetComponent<BoxCollider2D>().enabled = true;
        GameManager.instance.turnManager.StartTurn();
        this.enabled = false;

    }

    [Button]
    public void Test()=> transform.DOShakePosition(duration, strength, vibrato, random).OnComplete(() => transform.position = new Vector2(0, -0.136f));
}
