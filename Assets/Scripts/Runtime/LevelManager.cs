using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private GameObject _mapUnitPrefab;
    [SerializeField] private int _initSpawnCount = 20;
    [SerializeField] private float _mapUnitLengthz = 2f;
    [SerializeField] private float _baseZOffset = -10f;
    [SerializeField] private float _speed = 3f;

    private Transform[] _mapUnits;
    private int[] _unitIndices;
    private float _currentScrollZ;

    private void Awake()
    {
        if (GameManager.gameSession != null)
        {
            GameManager.gameSession.playSpeed = _speed;
        }
        else
        {
            Debug.LogWarning("GameSession is null. Make sure GameManager is initialized first.");
        }
    }

    private void Start()
    {
        _mapUnits = new Transform[_initSpawnCount];
        _unitIndices = new int[_initSpawnCount];

        for (int i = 0; i < _initSpawnCount; i++)
        {
            GameObject mapUnit = Instantiate(_mapUnitPrefab, transform);
            _mapUnits[i] = mapUnit.transform;
            _unitIndices[i] = i;
            
            float z = _baseZOffset + _unitIndices[i] * _mapUnitLengthz;
            _mapUnits[i].localPosition = new Vector3(0, 0, z);
        }
        
    }

    private void Update()
    {
        _currentScrollZ += Time.deltaTime * _speed;
        float recycleThreshold = _baseZOffset - _mapUnitLengthz;

        for (int i = 0; i < _mapUnits.Length; i++)
        {
            float z = _baseZOffset + _unitIndices[i] * _mapUnitLengthz - _currentScrollZ;

            if (z < recycleThreshold)
            {
                _unitIndices[i] += _initSpawnCount;
                z = _baseZOffset + _unitIndices[i] * _mapUnitLengthz - _currentScrollZ;
            }

            _mapUnits[i].localPosition = new Vector3(0, 0, z);
        }
    }
}
