using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    [SerializeField] float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(time > 0)
            Destroy(gameObject, time);
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }

}
