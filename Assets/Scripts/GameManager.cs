using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(ObjectPoolManager))]
public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion


    public int highScore;
    public int score;

    private Player playerObject;

    public List<Enemy> activeEnemies;
    public float enemySpeed;
    public float enemySideSpeed;

    private void Start()
    {
        playerObject = Player.Instance;      
    }

    public void IncreaseSpeed()
    {
        enemySpeed = activeEnemies[0].moveSpeed;
        enemySideSpeed = activeEnemies[0].sideSpeed;

        enemySpeed++;
        enemySideSpeed++;

        for (int i = 0; i < activeEnemies.Count; i++)
        {
            activeEnemies[i].moveSpeed = enemySpeed;
            activeEnemies[i].sideSpeed = enemySideSpeed;
        }
    }

    public void GameOver()
    {
        Debug.Log("Game over.");

        //TODO game over uitwerken.
    }

}
