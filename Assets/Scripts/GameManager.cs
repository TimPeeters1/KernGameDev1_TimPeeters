﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ObjectPoolManager))]
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

    public GameObject DeathScreen;

    public List<Enemy> activeEnemies;
    public float enemySpeed;

    private void Start()
    {
        playerObject = Player.Instance;

        DeathScreen.SetActive(false);
    }

    private void Update()
    {
        if (activeEnemies.Count <= 0)
        {
            Spawner.Instance.Spawn();
        }
    }

    public void IncreaseSpeed()
    {
        //TODO Enemy increase smoothen.

        enemySpeed = activeEnemies[0].moveSpeed;
        enemySpeed += 0.2f;

        StartCoroutine(DoIncrease(1f, activeEnemies[0].moveSpeed, enemySpeed)); 
    }

    IEnumerator DoIncrease(float _overTime, float _currentValue, float _newValue)
    {
        float startTime = Time.time;

        while(Time.time < (startTime + _overTime))
        {
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                activeEnemies[i].moveSpeed = Mathf.Lerp(_currentValue, _newValue, (Time.time - startTime) / _overTime);
                activeEnemies[i].sideSpeed = Mathf.Lerp(_currentValue, _newValue, (Time.time - startTime) / _overTime);
            }
            yield return null;
        }

        yield return null;
    }

    public void GameOver()
    {
        Player.Instance.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        DeathScreen.SetActive(true);
    }

}
