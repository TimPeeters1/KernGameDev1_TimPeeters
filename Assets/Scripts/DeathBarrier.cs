using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBarrier : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.minDistance = transform.position.z;
    }

}
