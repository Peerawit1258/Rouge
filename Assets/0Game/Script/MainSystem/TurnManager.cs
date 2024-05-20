using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TurnManager : MonoBehaviour
{
    public int turnCount;
    public ActionTurn actionTurn = ActionTurn.player;
    public PlayerController player;
    [ReadOnly] public EnemyController targetEnemy;
    public List<EnemyController> enemies;

    SkillOrderSystem skillOrderSystem;
    // Start is called before the first frame update
    void Start()
    {
        skillOrderSystem = GameManager.instance.skillOrderSystem;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartEnemyTurn()
    {
        if (enemies.Count == 0)
        {
            GameManager.instance.resultBattle.StartWinResult();
            return;
        }

        actionTurn = ActionTurn.enemies;
        GameManager.instance.battleSetup.arrow.gameObject.SetActive(false);
        GameManager.instance.detailPanel.ShowActionTurn(actionTurn, () =>
        {
            skillOrderSystem.slotPlace.gameObject.SetActive(false);
            GameManager.instance.enemyTurnSystem.StartOrderEnemyAttack();
        });
        
    }

    public void StartPlayerTurn()
    {
        actionTurn = ActionTurn.player;
        GameManager.instance.battleSetup.arrow.gameObject.SetActive(true);
        GameManager.instance.detailPanel.ShowActionTurn(actionTurn, () =>
        {
            GameManager.instance.statusEffectSystem.TriggerStatusCount(TriggerStatus.Start);
            GameManager.instance.relicManagerSystem.TriggerRelicEffect(TriggerStatus.Start);
            skillOrderSystem.ClearSlotSkill();
            skillOrderSystem.isSkillActive = false;
            skillOrderSystem.slotPlace.gameObject.SetActive(true);
            skillOrderSystem.CreateSkillSlot();
        });
    }

    public void StartTurn()
    {
        if (enemies.Count == 0)
        {
            GameManager.instance.resultBattle.StartWinResult();
            return;
        }
        if (player.hpValue <= 0) return;

        if (targetEnemy == null) targetEnemy = enemies[0];

        targetEnemy.OnMouseUp();
        turnCount++;

        StartCoroutine(StartDelayPlayerTurn());
    }

    IEnumerator StartDelayPlayerTurn()
    {
        yield return new WaitUntil(()=>GameManager.instance.playerData.currentSkills.Count > 0);
        StartPlayerTurn();
    }

    public void ResetValue()
    {
        turnCount = 0;
    }
    public void RemoveEnemy(EnemyController enemy)
    {
        enemies.Remove(enemy);
        if(enemies.Count > 0)
            targetEnemy = enemies[0];
        targetEnemy.OnMouseUp();
    }
}

public enum ActionTurn
{
    player,
    enemies
}
