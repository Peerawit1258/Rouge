using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using DG.Tweening;
using Random = UnityEngine.Random;

public class EncounterManagementSystem : SerializedMonoBehaviour
{
    //[TabGroup("Value"), SerializeField] Dictionary<string, int> stage = new Dictionary<string, int>();
    [TableList(ShowIndexLabels = true)] public List<StageDetail> stageDetail = new List<StageDetail>();
    public StageDetail restArea;
    [ReadOnly, TabGroup("Value")] public string stageName;
    [ReadOnly, TabGroup("Value")] public int stageCount = 0;
    [TabGroup("Value")] public List<Node> nodes;

    [TabGroup("Reward")]public List<SkillAction> skillDrops;
    [TabGroup("Reward")]public List<Relic> relicDrops;
    [TabGroup("Reward")] public int gold;

    [SerializeField] List<DoorNode> doors;
    [SerializeField, ReadOnly] List<string> previousNode;

    TurnManager turnManager;
    // Start is called before the first frame update
    void Start()
    {
        turnManager = GameManager.instance.turnManager;
    }

    #region Drop
    public void AddDrop(SkillAction skill, Relic relic, int value)
    {
        if (skill != null)
        {
            if (!CheckAlreadySkillDrop(skill))
                skillDrops.Add(skill);
        }
        if(relic != null)
        {
            if(!CheckAlreadyRelicDrop(relic))
                relicDrops.Add(relic);
        }

        gold += value;
    }


    public bool CheckAlreadySkillDrop(SkillAction skill)
    {
        foreach (var sk in skillDrops)
        {
            if (sk.id == skill.id)
                return true;
        }

        return false;
    }

    public bool CheckAlreadyRelicDrop(Relic relic)
    {
        foreach (var rl in relicDrops)
        {
            if (rl.id == relic.id)
                return true;
        }

        return false;
    }
    #endregion
    #region Door
    public void CreateNextDoorNode()
    {
        StageDetail current = GetStageDetailwithName();
        
        if(stageCount == current.stageCount - 1)// Boss Stage
        {

            
        }
        else if(stageCount == current.stageCount)// Rest Stage
        {

        }
        else
        {
            int num = Random.Range(2, 4);
            EncounterNode newNode = new EncounterNode();
            List<EncounterNode> lists = new List<EncounterNode>();
            for (int i = 0; i < 3; i++)
            {
                lists = GetStageDetailwithName().GetEncounterList(GetNodeType());
                do
                {
                    newNode = lists[Random.Range(0, lists.Count)];
                } while (CheckAlreadyHaveEncounter(newNode));

                doors[i].DoorSetup(newNode);
                //Debug.Log(newNode.nodeID);
            }
        }
    }

    private Node GetNodeType()
    {
        Node node = Node.Normal;
        do
        {
            node = nodes[Random.Range(0, nodes.Count)];
        } while (CheckDoorNodewithType(node));
        return node;
    }

    private bool CheckDoorNodewithType(Node type)
    {
        if (type == Node.Normal && !GameManager.instance.isTest) return false;
        foreach (var door in doors)
        {
            if (door.GetEncounter() != null)
                if (door.GetEncounter().node == type)
                    return true;
        }
        return false;
    }

    private bool CheckAlreadyHaveEncounter(EncounterNode node)
    {
        foreach (var door in doors)
            if (door.GetEncounter() != null)
                if (door.GetEncounter().nodeID == node.nodeID)
                    return true;

        if (node.node != Node.Treasure && previousNode.Contains(node.nodeID))
            return true;


        return false;
    }

    public void SetActiveDoor(bool active)
    {
        foreach(var door in doors)
            door.gameObject.SetActive(active);
    }

    public void ClearDoorNode()
    {
        foreach (var door in doors)
            if (door.GetEncounter() != null)
                door.ClearDoor();
    }
    #endregion
    #region Stage
    [Button]
    public void StartSetupNextEncounter(EncounterNode node = null)
    {
        stageCount++;
        if (stageCount >= GetCurrentStageCountwithName())
        {
            ChangeStage();
            return;
        }
        else
        {
            GameManager.instance.animationChangeSceneSystem.BlackFade(1, 1, () =>
            {
                turnManager.player.animationAction.WalkInSceneAction();
                if (node.node == Node.Normal)
                {
                    GameManager.instance.battleSetup.SetupEnemyBattle(node.enemyGroup.enemies);
                    DOVirtual.DelayedCall(1, () =>
                    {
                        Debug.Log("Create");
                        GameManager.instance.turnManager.StartTurn();
                    });
                }
                else if(node.node == Node.Event)
                {
                    StartCoroutine(SetupEvent(node));
                }
                else if(node.node == Node.Treasure)
                {
                    //GameManager.instance.battleSetup.SetupSpecialEnemyBattle(node.chest, 1);EnemyController enemy;
                    Instantiate(node.chest.character, GameManager.instance.battleSetup.GetEnemyPos()[1]);
                    
                }
                previousNode.Add(node.nodeID);
                SetActiveDoor(false);
            }); 
        }
    }

    void ChangeStage()
    {
        for (int i = 0; i < stageDetail.Count; i++)
        {
            if (stageDetail[i].stageName == stageName)
            {
                if (i + 1 >= stageDetail.Count) return;
                stageDetail[i].encounterMap.SetActive(false);
                stageDetail[i + 1].encounterMap.SetActive(true);
                stageName = stageDetail[i + 1].stageName;
                stageCount = 1;
            }

        }
    }

    int GetCurrentStageCountwithName()
    {
        for(int i = 0; i < stageDetail.Count; i++)
        {
            if(stageDetail[i].stageName == stageName)
                return stageDetail[i].stageCount;
        }
        return 0;
    }

    StageDetail GetStageDetailwithName()
    {
        for (int i = 0; i < stageDetail.Count; i++)
        {
            if (stageDetail[i].stageName == stageName)
                return stageDetail[i];
        }
        return null;
    }
    #endregion

    IEnumerator SetupEvent(EncounterNode node)
    {
        GameManager.instance.battleSetup.eventSymbol.SetActive(true);
        //turnManager.player.animationAction.WalkInSceneAction();
        yield return new WaitUntil(()=>!turnManager.player.animationAction.isAction);
        yield return new WaitForSeconds(1);
        
        GameManager.instance.eventManager.EventSetup(node.eventInfo);

    }

    public void ResetValue()
    {
        skillDrops.Clear();
        gold = 0;
    }

    [Button]
    public void SetAllIDNode()
    {
        foreach(var stage in stageDetail)
        {
            foreach (var node in stage.normalNode)
                node.SetID();
            foreach (var node in stage.eliteNode)
                node.SetID();
            foreach (var node in stage.eventNode)
                node.SetID();
            foreach (var node in stage.treasureNode)
                node.SetID();
            
            if(stage.bossNode != null) stage.bossNode.SetID();
        }
    }
}

[Serializable]
public class StageDetail
{
    public GameObject encounterMap;
    public string stageName;
    [TabGroup("Detail", "Detail")]public int stageCount;
    [TabGroup("Detail", "Detail")]public List<EncounterNode> normalNode;
    [TabGroup("Detail", "Detail")]public List<EncounterNode> eliteNode;
    [TabGroup("Detail", "Detail")]public List<EncounterNode> eventNode;
    [TabGroup("Detail", "Detail")]public List<EncounterNode> treasureNode;
    [TabGroup("Detail", "Detail")]public EncounterNode bossNode;

    public List<EncounterNode> GetEncounterList(Node type)
    {
        if (type == Node.Treasure) return treasureNode;
        else if (type == Node.Elite) return eliteNode;
        else if (type == Node.Event) return eventNode;
        else return normalNode;
    }

}