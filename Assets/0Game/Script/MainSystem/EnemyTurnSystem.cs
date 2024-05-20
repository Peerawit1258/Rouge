using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnSystem : MonoBehaviour
{

    TurnManager turnManager;
    // Start is called before the first frame update
    void Start()
    {
        turnManager = GameManager.instance.turnManager;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int currentEnemy = 0;

    public void StartOrderEnemyAttack()
    {
        if (turnManager.enemies.Count <= 0)
        {
            GameManager.instance.resultBattle.StartWinResult();
            return;
        }
        currentEnemy = 0;
        StartCoroutine(OrderEnemyAttack());
    }
    IEnumerator OrderEnemyAttack()
    {
        if (turnManager.player.hpValue <= 0) yield break;
        turnManager.enemies[currentEnemy].StartEnemyAttack();

        
        if (currentEnemy < turnManager.enemies.Count - 1)
        {
            yield return new WaitUntil(() => !turnManager.enemies[currentEnemy].animationAction.isAction);
            yield return new WaitForSeconds(1);
            currentEnemy++;
            StartCoroutine(OrderEnemyAttack());
        }
        else
        {
            yield return new WaitForSeconds(1);
            turnManager.StartTurn();
        }
            
    }

    public void DecreaseCurrentEnemy()=> currentEnemy--;
}
