using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [TabGroup("System")] public TurnManager turnManager;
    [TabGroup("System")] public DamageCalculator damageCalculator;
    [TabGroup("System")] public SkillOrderSystem skillOrderSystem;
    [TabGroup("System")] public EnemyTurnSystem enemyTurnSystem;
    [TabGroup("System")] public NumberDamageSystem numberDamageSystem;
    [TabGroup("System")] public StatusEffectSystem statusEffectSystem;
    [TabGroup("System")] public EncounterManagementSystem encounterManagementSystem;
    [TabGroup("System")] public AnimationChangeSceneSystem animationChangeSceneSystem;
    [TabGroup("System")] public RelicManagerSystem relicManagerSystem;
    [TabGroup("Battle")] public BattleSetup battleSetup;
    [TabGroup("Battle")] public GaugeHpEnemy gaugeHpEnemy;
    [TabGroup("Battle")] public ResultBattle resultBattle;
    [TabGroup("Battle")] public EventManager eventManager;
    [TabGroup("Battle")] public DetailPanel detailPanel;
    [TabGroup("Battle")] public InventoryManager inventoryManager;
    [TabGroup("Manager")] public AllData allData;
    [TabGroup("Manager")] public PlayerData playerData;

    public CameraControl mainCamera;
    [SerializeField] CharacterDetail playerDetail;
    [SerializeField] public EnemyGroup group ;
    [SerializeField] public bool isTest;

    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetupBattleStage();
        
        //Debug.Log(t);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupBattleStage()
    {
        if (isTest)
        {
            encounterManagementSystem.stageName = encounterManagementSystem.stageDetail[0].stageName;
            encounterManagementSystem.stageCount++;
            encounterManagementSystem.stageDetail[0].encounterMap.SetActive(true);
            if (turnManager.player == null) battleSetup.SetupPlayerBattle(playerDetail);
            battleSetup.SetupEnemyBattle(group.enemies);

            DOVirtual.DelayedCall(1,() =>
            {
                Debug.Log("Create");
                turnManager.StartTurn();
            });
            return;
        }
    }

}
