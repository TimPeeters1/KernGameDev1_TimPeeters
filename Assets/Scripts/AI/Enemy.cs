using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable, IPoolObject
{

    [Header("Move Settings")]
    [Tooltip("The distance from the player for which it stops.")]
    public float stopDistance = 200f;
    public float moveSpeed = 10f;
    public float sideSpeed = 10f;

    [Header("Health Settings")]
    [SerializeField] int health = 40;
    [SerializeField] int score;

    [Header("Gun Settings")]
    [SerializeField] Vector2 waitTimeForShot;

    [SerializeField] Pool bulletPool;
    [SerializeField] float bulletForce;
    [SerializeField] int bulletDamage;

    [Space]
    [Tooltip("Spawn positions of bullets (e.g. the position where the bullets come out of the rifle.")]
    [SerializeField] private GameObject[] spawnPositions;

    private GameObject target;
    private ObjectPoolManager objectPool;

    [Space]
    [Header("Death Settings")]
    [SerializeField] Pool particlePool;

    [SerializeField]
    private float attackTimer;

    public float prefferedHeight;

    float sideStartPosition;

    private void Start()
    {   
        objectPool = ObjectPoolManager.Instance;

        if (!objectPool.pools.Contains(bulletPool) && !objectPool.pools.Contains(particlePool))
        {
            objectPool.AddPool(bulletPool);
            objectPool.AddPool(particlePool);
        }
        
        target = Player.Instance.gameObject;

        attackTimer = Random.Range(waitTimeForShot.x, waitTimeForShot.y);

        sideStartPosition = transform.position.x;
    }

    public void OnObjectSpawn()
    {
        attackTimer = Random.Range(waitTimeForShot.x, waitTimeForShot.y);

        if (!GameManager.Instance.activeEnemies.Contains(this))
        {
            GameManager.Instance.activeEnemies.Add(this);
        }
    }

    public void OnObjectDespawn()
    {
        if (GameManager.Instance.activeEnemies.Contains(this))
        {
            GameManager.Instance.IncreaseSpeed();
            GameManager.Instance.activeEnemies.Remove(this);
        }

        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        MoveToTarget();

        if (Timer() < 0)
        {
            Attack();
        }
    }
        
    private void MoveToTarget()
    {
        float _targetDistance = Vector3.Distance(transform.position, Vector3.zero);
        float _height = transform.position.y;

        Vector3 _forwardPostion;

        if (_targetDistance > stopDistance){
            // Debug.DrawRay(transform.position, -transform.forward * 10, Color.red);

            if(_height > prefferedHeight)
            {
                _forwardPostion = transform.position + new Vector3(0, -3, -1);
                transform.position = Vector3.Lerp(transform.position, _forwardPostion, Time.deltaTime * moveSpeed * 5f);
            }
            else
            {
                MoveSideways();
                _forwardPostion = transform.position + -transform.forward;
                transform.position = Vector3.Lerp(transform.position, _forwardPostion, Time.deltaTime * moveSpeed);
            }
        }
    }

    private void MoveSideways()
    {
        float _sidewaysPostion = Mathf.Lerp(sideStartPosition, sideStartPosition + 300f, Mathf.PingPong(Time.time * sideSpeed * 0.01f, 1));

        transform.position = new Vector3(_sidewaysPostion, transform.position.y, transform.position.z);
    }   

    public void Damage(int damage)
    {
        if (health <= 0) {
            Die();
        }
        else{
            health -= damage;
        }
    }

    public void Die()
    {
        //Highscore naar manager
        GameManager.Instance.score += score;

        //Death particle spawn
        objectPool.SpawnFromPool(particlePool, transform.position, particlePool.prefab.transform.rotation);

        OnObjectDespawn();
    }

    private void Attack()
    {
        //yield return new WaitForSeconds(Random.Range(maxWaitTimeForShot/2, maxWaitTimeForShot));

        for (int i = 0; i < spawnPositions.Length; i++)
        {
            Vector3 _targetDirection = target.transform.position - spawnPositions[i].transform.position;

            GameObject _bulletClone = objectPool.SpawnFromPool(bulletPool, spawnPositions[i].transform.position, spawnPositions[i].transform.rotation);
            _bulletClone.GetComponent<Bullet>().bulletDamage = bulletDamage;

            _bulletClone.GetComponent<Rigidbody>().AddForce(_targetDirection * bulletForce);

        }

       attackTimer = Random.Range(waitTimeForShot.x, waitTimeForShot.y);

    }

    private float Timer()
    {
        return attackTimer -= Time.deltaTime;
    }


}
