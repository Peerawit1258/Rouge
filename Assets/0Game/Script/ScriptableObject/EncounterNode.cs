using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Node", fileName ="NewNode")]
public class EncounterNode : ScriptableObject
{
    [ReadOnly]public string nodeID;
    [PreviewField] public Sprite icon;
    public Node node;

    [ShowIf("@node == Node.Normal || node == Node.Elite || node == Node.Boss ")] public EnemyGroup enemyGroup;
    [ShowIf("@node == Node.Event ")] public EventInfo eventInfo;
    [ShowIf("@node == Node.Treasure ")] public CharacterDetail chest;

    [Button]
    public void SetID()
    {
        nodeID = name;
        //nodeID = name.Split('_')[1];
    }
}

public enum Node
{
    Normal,
    Elite,
    Boss,
    Rest,
    Event,
    Treasure
}
