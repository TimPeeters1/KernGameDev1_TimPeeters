using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour, IDamagable
{
    //TODO UI en Events toevoegen.
    #region Singleton
    public static Player Instance;

    private void Awake()
    {
        Instance = this;

    }
    #endregion

    [HideInInspector] public GameObject cameraObject;

    [Header("Mouse Settings")]
    [SerializeField] float xSensitivity = 0.5f;
    [SerializeField] float ySensitivity = 0.5f;

    [Header("Rotation Settings")]
    [SerializeField] Vector2 maxHorizontalRotation = new Vector2(-120, 120);
    [SerializeField] Vector2 maxVerticalRotation = new Vector2(-80, 45);

    [Header("Move Settings")]
    [SerializeField] Vector2 movementRestrictions = new Vector2(-120, 120);
    [SerializeField] float moveSpeed;

    private float yaw = 0f;
    private float pitch = 0f;

    [Space]
    [Header("Gun Settings")]
    [SerializeField] Pool bulletPool;
    [SerializeField] float bulletForce;
    [SerializeField] int bulletDamage;
    [SerializeField] float shotTimer;

    [Space]
    [Tooltip("Spawn positions of bullets (e.g. the position where the bullets come out of the rifle.")]
    [SerializeField] GameObject[] spawnPositions;
    [SerializeField] GameObject crosshairPosition;

    [Space]
    [Header("Health Settings")]
    [SerializeField] int maxHealth;
    int health;
    [SerializeField] Image healthBar;
    [SerializeField] int maxLives;
    int currentLives;

    //ObjectPool
    private ObjectPoolManager objectPool;
    [SerializeField] Pool particlePool;

    private float timer;

    private float Timer()
    {
        if (timer > 0)
        {
            return timer -= Time.deltaTime;
        }
        else
        {
            return timer = 0;
        }
    }

    private void Start()
    {
     
        objectPool = ObjectPoolManager.Instance;

        objectPool.AddPool(bulletPool);
       // objectPool.AddPool(particlePool);

        cameraObject = GetComponentInChildren<Camera>().gameObject;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        health = maxHealth;

        healthBar.fillAmount = (float)health / (float)maxHealth;

        currentLives = maxLives;
    }

    private void Update()
    {
        DoLookAround();
        DoMovement();

        if (Input.GetButtonDown("Fire1") && timer <= 0)
        {
            DoShot();   
            timer = shotTimer;
        }

        Timer();

        //Ray ray = 


        Debug.DrawRay(spawnPositions[0].transform.position, spawnPositions[0].transform.forward * 100, Color.green);
        Debug.DrawRay(spawnPositions[1].transform.position, spawnPositions[1].transform.forward * 100, Color.green);
    }

    private void DoMovement()
    {
        float _horizontalAxis = Input.GetAxis("Horizontal");

        Vector3 _newPlayerPostion = transform.position;
        _newPlayerPostion = new Vector3(transform.position.x + _horizontalAxis * moveSpeed, transform.position.y, transform.position.z);
        _newPlayerPostion.x = Mathf.Clamp(_newPlayerPostion.x, movementRestrictions.x, movementRestrictions.y);

        transform.position = _newPlayerPostion;

        //transform.position = Vector3.Lerp(transform.position, newPlayerPostion, Time.deltaTime * 0.5f);
    }

    private void DoLookAround()
    {
        float _mouseX = Input.GetAxis("Mouse X");
        float _mouseY = Input.GetAxis("Mouse Y");

        yaw += _mouseX * xSensitivity;
        pitch -= _mouseY * ySensitivity;

        yaw = Mathf.Clamp(yaw, maxHorizontalRotation.x, maxHorizontalRotation.y);
        pitch = Mathf.Clamp(pitch, maxVerticalRotation.x, maxVerticalRotation.y);

        cameraObject.transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }

    private void DoShot()
    {
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            Debug.DrawRay(spawnPositions[i].transform.position, spawnPositions[i].transform.forward, Color.red);

            GameObject _bulletClone = objectPool.SpawnFromPool(bulletPool, spawnPositions[i].transform.position, spawnPositions[i].transform.rotation);
            _bulletClone.GetComponent<Bullet>().bulletDamage = bulletDamage;

            //TODO Bullet accurate maken
            _bulletClone.GetComponent<Rigidbody>().AddForce(spawnPositions[i].transform.forward * bulletForce);

        }
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
        if(currentLives > 1)
        {
            currentLives--;
            health = maxHealth;

            objectPool.SpawnFromPool(particlePool, transform.position, particlePool.prefab.transform.rotation);
        }
        else
        {
            GameManager.Instance.GameOver();
        }
    }
}
