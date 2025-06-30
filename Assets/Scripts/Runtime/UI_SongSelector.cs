using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_SongSelector : MonoBehaviour
{
    [Serializable]
    class SongInfo
    {
        public SongSpec songSpec;
        public GameObject songCard;
    }

    public int selectedSongIndex
    {
        get => _selectedSongIndex;
        set
        {
            _songInfos[_selectedSongIndex].songCard.SetActive(false); // 이전 카드 비활성화
            _selectedSongIndex = value;
            _songInfos[_selectedSongIndex].songCard.SetActive(true); // 현재 카드 활성화
            _songTitle.text = _songInfos[_selectedSongIndex].songSpec.title; // 노래 제목 바꿔줌
        }
    }

    [SerializeField] private TMP_Text _songTitle;
    [SerializeField] private SongInfo[] _songInfos;
    private int _selectedSongIndex;
    [SerializeField] Button _nextButton;
    [SerializeField] Button _prevButton;
    [SerializeField] Button _playButton;

    private void OnEnable()
    {
        _nextButton.onClick.AddListener(Next);
        _prevButton.onClick.AddListener(Prev);
        _playButton.onClick.AddListener(Play);
    }

    private void OnDisable()
    {
        _nextButton.onClick.RemoveListener(Next);
        _prevButton.onClick.RemoveListener(Prev);
        _playButton.onClick.RemoveListener(Play);
    }

    private void Start()
    {
        for (int i = 0; i < _songInfos.Length; i++)
        {
            _songInfos[i].songCard.SetActive(i == _selectedSongIndex);
        }
        _songTitle.text = _songInfos[_selectedSongIndex].songSpec.title;
    }


    public void Next ()
    {
        selectedSongIndex = (_selectedSongIndex + 1) % _songInfos.Length;
    }

    public void Prev()
    {
        selectedSongIndex = (_selectedSongIndex + _songInfos.Length - 1) % _songInfos.Length;
    }

    public void Play()
    {
        GameManager.gameSession.selectedSongSpec = _songInfos[selectedSongIndex].songSpec;
        SceneManager.LoadScene("InGame");
    }

}