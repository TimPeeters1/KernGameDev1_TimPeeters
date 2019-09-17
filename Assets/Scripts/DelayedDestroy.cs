using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDestroy : MonoBehaviour
{
    [SerializeField] float secondsBeforeDestroy;

    ObjectPoolManager poolManager;

    void Start()
    {
        Destroy(this.gameObject, secondsBeforeDestroy);
    }


}
