using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NoteManager : MonoBehaviour
{
    private AudioSource _audioSource;
    private List<float> _peaks;
    [SerializeField] private GameObject _notePrefab;
    [SerializeField] private Vector2 _spawnRange = new (1f, 1f);

    private Dictionary<Transform, float> _noteTable;
    [SerializeField] private float _spawnDelay = 3f;

    [SerializeField] private float _deadLineZ = -1f;
    private List<Transform> _cachedDeadNotes;
    
    [SerializeField] private MapNoteDissolveAnimation _dissolveAnimation;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _noteTable = new Dictionary<Transform, float>();
        _cachedDeadNotes = new List<Transform>();
    }

    private void Start()
    {
        _peaks = GameManager.gameSession.selectedSongSpec.peaks;
        Play();
    }

    public void Play()
    {
        StartCoroutine(C_PlayAudio());
        StartCoroutine(C_SpawnNotes());
        StartCoroutine(C_MoveNotes());
    }

    IEnumerator C_PlayAudio()
    {
        yield return new WaitForSeconds(_spawnDelay);
        _audioSource.Play();
    }

    IEnumerator C_SpawnNotes()
    {
        int index = 0;
        float playTime = _audioSource.clip.length;
        float playSpeed = GameManager.gameSession.playSpeed;

        while (index < _peaks.Count)
        {
            while (index < _peaks.Count)
            {
                if (_audioSource.time >= _peaks[index])
                {
                    GameObject note = Instantiate(_notePrefab);
                    int x = Random.Range(-1, 2);
                    int y = Random.Range(1, 3);

                    note.transform.position = new Vector3(x * _spawnRange.x / 2,
                                                            y * _spawnRange.y / 2,
                                                            _peaks[index] * playSpeed + _spawnDelay * playSpeed);
                    _noteTable.Add(note.transform, _peaks[index]);
                    index++;
                }
                else
                {
                    break;
                }
            }
            yield return null;            
        }
    }

    IEnumerator C_MoveNotes()
    {
        float playSpeed = GameManager.gameSession.playSpeed;

        while (true)
        {
            _cachedDeadNotes.Clear();
            
            foreach (KeyValuePair<Transform, float> notePair in _noteTable)
            {
                Vector3 position = notePair.Key.position;
                position.z = (notePair.Value - _audioSource.time + _spawnDelay) * playSpeed;
                notePair.Key.position = position;

                if (position.z < _deadLineZ)
                {
                    Renderer renderer = notePair.Key.GetComponent<Renderer>();
                    _dissolveAnimation.PlayC_Dissolve(renderer,
                        () => {
                            Destroy(renderer.gameObject);
                            SoundManager.instance.PlaySFX(SFXType.DestroySound);
                        });
                    _cachedDeadNotes.Add(notePair.Key);
                }
            }

            for (int i = 0; i < _cachedDeadNotes.Count; i++)
            {
                _noteTable.Remove(_cachedDeadNotes[i]);
            }

            yield return null;
        }
    }
}
