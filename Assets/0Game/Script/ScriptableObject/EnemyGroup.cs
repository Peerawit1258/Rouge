using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewGroup",menuName = "Group/Enemy")]
public class EnemyGroup : ScriptableObject
{
    public List<CharacterDetail> enemies;
}
