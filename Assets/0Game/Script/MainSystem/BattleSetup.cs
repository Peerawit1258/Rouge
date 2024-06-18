using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class BattleSetup : SerializedMonoBehaviour
{
    [SerializeField] Transform playerPos;
    [SerializeField] List<Transform> enemyPos;
    public GameObject eventSymbol;
    public Transform arrow;
    public Transform backStage;

    //public Dictionary<string, GameObject> battleScene;

    // Start is called before the first frame update
    void Start()
    {
        eventSymbol.SetActive(false);
    }

    public void SetupEnemyBattle(List<CharacterDetail> enemies)
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            if(enemies[i].e_type == EnemyType.Boss)
            {
                if (enemies[i].characterName == "The Golem") SetupSpecialEnemyBattle(enemies[i], 1);
                continue;
            }
            EnemyController enemy;
            if (enemyPos[i].childCount == 0)
            {
                 enemy = Instantiate(enemies[i].character, enemyPos[i]).GetComponent<EnemyController>();
            }
            else
            {
                if (i + 1 >= enemyPos.Count) return;
                enemy = Instantiate(enemies[i].character, enemyPos[i + 1]).GetComponent<EnemyController>();
            }
            enemy.transform.localPosition = Vector3.zero;
            enemy.SetInfoEnemy(enemies[i]);
            //enemy.SetDrop(enemies[i].skillDrop, enemies[i].relicDrop, enemies[i].goldDrop);
            enemy.name = enemies[i].characterName + "_" + i;
            GameManager.instance.gaugeHpEnemy.SetPositionGauge(enemy);
            GameManager.instance.turnManager.enemies.Add(enemy);
        }
    }

    public void SetupSpecialEnemyBattle(CharacterDetail special, int index = 1)// boss/special
    {
        EnemyController enemy;
        enemy = Instantiate(special.character, enemyPos[index]).GetComponent<EnemyController>();
        enemy.transform.localPosition = Vector3.zero;
        enemy.SetInfoEnemy(special);
        //enemy.SetDrop(special.skillDrop, special.relicDrop, special.goldDrop);
        enemy.name = special.characterName;
        GameManager.instance.gaugeHpEnemy.SetPositionGauge(enemy);
        GameManager.instance.turnManager.enemies.Add(enemy);
        //enemy.enabled = false;
    }

    public void SetupPlayerBattle(CharacterDetail player)
    {
        PlayerController character = Instantiate(player.character, playerPos).GetComponent<PlayerController>();
        GameManager.instance.detailPanel.SetStartDetail(player.atk, player.def, GameManager.instance.playerData.GetCurrentGold());
        character.SetStatValue(player.maxHP, player.atk, player.def, player.damageReduce);
        character.name = player.characterName;

        GameManager.instance.gaugeHpEnemy.SetPlayerHpWidget(character);
        GameManager.instance.turnManager.player = character;
    }

    public void SetPositionArrow(Transform pos)
    {
        if (GameManager.instance.turnManager.CheckEnemyTaunt()) return;
        arrow.position = pos.position + new Vector3(0, 0.2f, 0);
    }

    public List<Transform> GetEnemyPos() => enemyPos;

}
