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

    float timer;
    Camera mainCam;
    RaycastHit _hit;

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

        mainCam = GetComponentInChildren<Camera>();
        cameraObject = mainCam.gameObject;

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
        //Debug.DrawRay(_camRay.origin, _camRay.direction * 10000, Color.cyan);
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

        RaycastHit _hit;
        Ray _camRay = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Physics.Raycast(_camRay, out _hit);
      
        Vector3 _dir;

        for (int i = 0; i < spawnPositions.Length; i++)
        {
            //Debug.DrawRay(spawnPositions[i].transform.position, spawnPositions[i].transform.forward, Color.red);

            GameObject _bulletClone = objectPool.SpawnFromPool(bulletPool, spawnPositions[i].transform.position, spawnPositions[i].transform.rotation);
            _bulletClone.GetComponent<Bullet>().bulletDamage = bulletDamage;

            if (Physics.Raycast(_camRay, out _hit, 10000f))
            {
                _dir = _hit.point - spawnPositions[i].transform.position;
                //_dir = _dir.normalized;

                _bulletClone.GetComponent<Rigidbody>().AddForce(_dir * bulletForce);
            }
            else
            {
                _dir = _camRay.GetPoint(2000f) - spawnPositions[i].transform.position;
                _bulletClone.GetComponent<Rigidbody>().AddForce(_dir * bulletForce);
            }
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

            StartCoroutine(CameraShake(.2f, .4f));
        }
    }

    public void Die()
    {
        if (currentLives > 1)
        {
            currentLives--;
            health = maxHealth;
            healthBar.fillAmount = (float)health / (float)maxHealth;

            objectPool.SpawnFromPool(particlePool, transform.position, particlePool.prefab.transform.rotation);
        }
        else
        {
            GameManager.Instance.GameOver();
            StartCoroutine(CameraShake(.5f, 1.2f));
        }
    }

    public IEnumerator CameraShake(float _duration, float _magnitude)
    {
        Vector3 _originalPos = cameraObject.transform.localPosition;

        float _elapsed = 0.0f;

        while(_elapsed < _duration)
        {
            float x = Random.Range(-1f, 1f) * _magnitude;
            float y = Random.Range(-1f, 1f) * _magnitude;

            cameraObject.transform.localPosition = new Vector3(_originalPos.x + x, _originalPos.y + y, _originalPos.z);

            _elapsed += Time.deltaTime;

            yield return null;
        }

        cameraObject.transform.localPosition = _originalPos;
    }
}
