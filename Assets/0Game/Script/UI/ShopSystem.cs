using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ShopSystem : MonoBehaviour
{
    [TabGroup("Price"), ShowInInspector] public List<int> skillPrice = new List<int>();
    [TabGroup("Price"), ShowInInspector] public List<int> relicPrice = new List<int>();

    [SerializeField] GameObject skillPrefab;
    [SerializeField] GameObject relicPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
