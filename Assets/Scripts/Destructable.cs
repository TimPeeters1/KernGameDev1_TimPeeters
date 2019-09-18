using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Destructable : MonoBehaviour, IDamagable
{

    ObjectPoolManager objectPool;
    [SerializeField] float health;
    float maxHealth;

    [SerializeField] Pool particlePool;
    [SerializeField] Image healthBar;

    void Start()
    {
        objectPool = ObjectPoolManager.Instance;

        if (!objectPool.pools.Contains(particlePool))
        {
            //objectPool.AddPool(particlePool);
        }

        maxHealth = health;

        healthBar.fillAmount = (float)health / (float)maxHealth;
    }
    public void Damage(int damage)
    {
        if (health <= 0)
        {
            Die();
        }
        else
        {
            health -= damage;

            healthBar.fillAmount = (float)health / (float)maxHealth;
        }
    }

    public void Die()
    {
        //Death particle spawn
        objectPool.SpawnFromPool(particlePool, transform.position, particlePool.prefab.transform.rotation);

        Destroy(this.gameObject);
    }
}
