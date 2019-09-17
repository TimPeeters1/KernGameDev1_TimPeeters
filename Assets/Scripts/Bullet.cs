using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolObject
{
    [SerializeField] private float destroyTime;

    [HideInInspector] public int bulletDamage; //Will be assigned by gun firing the bullet.

    //public float bulletForce;
    //public Vector3 bulletDirection;
    private ObjectPoolManager poolManager;
    private Rigidbody rb;

    private void Start()
    {
        poolManager = ObjectPoolManager.Instance;
        rb = GetComponent<Rigidbody>();
    }

    public void OnObjectSpawn()
    {
        StartCoroutine(DisableAfterTime());
    }

    public void OnObjectDespawn()
    {
        this.gameObject.SetActive(false);

        rb.velocity = Vector3.zero;
        GetComponent<TrailRenderer>().Clear();
    }

    /*
    public void AddForce(Vector3 direction)
    {
        GetComponent<Rigidbody>().AddForce(direction * bulletForce);
    }
    */

    private void OnCollisionEnter(Collision collision)
    {
        try
        {
            if (collision.gameObject.GetComponent<IDamagable>() != null)
            {
                collision.gameObject.GetComponentInChildren<IDamagable>().Damage(bulletDamage);
                OnObjectDespawn();
            }
            else
            {   
                collision.gameObject.transform.root.GetComponent<IDamagable>().Damage(bulletDamage);
                OnObjectDespawn();
            }
        }
        catch { }
    }

    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(destroyTime);

        OnObjectDespawn();
    }

    
}
