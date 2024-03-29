﻿using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    private float _speedMultiplier = 2;
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _canFire = -1f;

    [SerializeField]
    private int _lives = 3;
    private int _score = 0;

    private SpawnManager _spawnManager;

    private Vector3 _offset = new Vector3(0, 1.05f, 0);

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleLaserPrefab;
    [SerializeField]
    private GameObject _playerShield;
    [SerializeField]
    private GameObject _rightEngine;
    [SerializeField]
    private GameObject _leftEngine;

    [SerializeField]
    private AudioClip _laserSound;
    private AudioSource _audioSource;

    private bool _isTripleShotActive = false;
    //private bool _isSpeedBoostActive = false;
    private bool _isShieldBoostActive = false;

    private UIManager _uiManager;

    void Start()
    {
        _rightEngine.SetActive(false);
        _leftEngine.SetActive(false);

        _playerShield.SetActive(false);
        transform.position = Vector3.zero;

        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();


        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is null.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is null.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("The Audio Souce is null.");
        }
        else
        {
            _audioSource.clip = _laserSound;
        }
    }

    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        #region User Input

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInout = Input.GetAxis("Vertical");

        //transform.Translate(Vector3.right * horizontalInput *  _speed * Time.deltaTime);
        //transform.Translate(Vector3.up * verticalInout *  _speed * Time.deltaTime);

        //More optimal -> less "new" keyword usage
        Vector3 direction = new Vector3(horizontalInput, verticalInout, 0);

        transform.Translate(direction * _speed * Time.deltaTime);

        #endregion

        #region Player Bounds

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11)
        {
            transform.position = new Vector3(-11, transform.position.y, 0);
        }
        else if (transform.position.x < -11)
        {
            transform.position = new Vector3(11, transform.position.y, 0);
        }

        #endregion
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive)
        {
            Instantiate(_tripleLaserPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + _offset, Quaternion.identity);
        }

        _audioSource.Play();
    }

    public void Damage()
    {
        if (_isShieldBoostActive)
        {
            _isShieldBoostActive = false;
            _playerShield.SetActive(false);
            return;
        }

        _lives--;

        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;

        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);

        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        //_isSpeedBoostActive = true;

        _speed *= _speedMultiplier;

        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);

        //_isSpeedBoostActive = false;

        _speed /= _speedMultiplier;
    }

    public void ShieldActive()
    {
        _isShieldBoostActive = true;
        _playerShield.SetActive(true);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
