using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPoolDisable : MonoBehaviour, IPoolObject
{
    [SerializeField] float disableTime;

    public void OnObjectSpawn()
    {
        StartCoroutine(DisableAfterTime());
    }

    public void OnObjectDespawn()
    {
        StopCoroutine(DisableAfterTime());

        this.gameObject.SetActive(false);
    }

    IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(disableTime);

        OnObjectDespawn();
    }

}
