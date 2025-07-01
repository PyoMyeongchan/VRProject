using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MapAudioEqualizerEffector : MonoBehaviour
{
    [SerializeField] private GameObject _unitPrefab;
    [SerializeField] private float _unitLengthZ = 1f;
    [SerializeField] private Vector3 _spawningOffset;
    [SerializeField, Range(64, 512)] private int _spawnCount = 128;
    [SerializeField] private float _scaleYMax = 4f;
    [SerializeField] private float _gamma = 2f;
    [SerializeField] private bool _doShuffle = true;
    
    private AudioSource _audioSource;
    private Transform[] _spawnedUnits;
    private float[] _spectrumData;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _spawnedUnits = new Transform[_spawnCount];
        _spectrumData = new float[_spawnCount];
        for (int i = 0; i < _spawnCount; i++)
        {
            Transform spawned = Instantiate(_unitPrefab).transform;
            spawned.position = _spawningOffset + new Vector3(0, 0, i * _unitLengthZ);
            _spawnedUnits[i] = spawned;
        }
    }

    private void Update()
    {
        _audioSource.GetSpectrumData(_spectrumData, 0, FFTWindow.Hamming);

        if (_doShuffle)
        {
            _spectrumData.Shuffle();
        }

        _spectrumData[0] = _spectrumData[_spawnCount - 1] = 0f;
        _spawnedUnits[0].localScale = _spawnedUnits[_spawnCount - 1].localScale = new Vector3(1f, 0.1f, 1f);
        for (int i = 1; i < _spawnCount - 1; i++)
        {
            _spectrumData[i] = Mathf.Log(1f + _spectrumData[i], 2) * _scaleYMax;
            _spectrumData[i] = Mathf.Pow(_spectrumData[i], _gamma);
            _spectrumData[i] = (_spectrumData[i -1] + _spectrumData[i] + _spectrumData[i + 1]) / 3f;
            float prevScaleY = _spawnedUnits[i].localScale.y;
            _spawnedUnits[i].localScale = new Vector3(1, Mathf.Lerp(prevScaleY, _spectrumData[i], Time.deltaTime * 30f), 1);
        }
    }
}
