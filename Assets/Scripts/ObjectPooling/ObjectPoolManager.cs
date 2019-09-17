using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    //TODO Objectpool meer performant maken.

    //TODO singleton 'veiliger' maken (voorkomen .instance.gameObject.Destroy())
    #region Singleton
    public static ObjectPoolManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public List<Pool> pools;
    public Dictionary<Pool, Queue<GameObject>> poolDictionary;

    public GameObject objectToSpawn;

    private void Start()
    {
        poolDictionary = new Dictionary<Pool, Queue<GameObject>>();
        pools = new List<Pool>();
    }

    //Add a new pool type to the pool dictionary of the manager.
    public void AddPool(Pool pool)
    {
        if (!pools.Contains(pool))
        {
            pools.Add(pool);


            Queue<GameObject> objectPool = new Queue<GameObject>();
            GameObject parentObject = new GameObject();
            parentObject.name = pool.poolName + " Pool";

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.parent = parentObject.transform;
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool, objectPool);
       }
    }


    //Enable/'spawn' an object from the defined pool, with given position and rotation.
    public GameObject SpawnFromPool(Pool pool, Vector3 position, Quaternion rotation)
    {

        if (!pools.Contains(pool))
        {
            Debug.LogWarning("Pool " + pool.poolName + " does not exist.");
            return null;
        }

        objectToSpawn = poolDictionary[pool].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;

        IPoolObject pooledObj = objectToSpawn.GetComponent<IPoolObject>();

        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[pool].Enqueue(objectToSpawn);


        return objectToSpawn;


    }


}
