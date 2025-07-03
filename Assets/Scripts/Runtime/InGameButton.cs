using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class InGameButton : MonoBehaviour
{
    [SerializeField] private Button _homeButton;

    private void OnEnable()
    {
        _homeButton.onClick.AddListener(Return);
    }

    private void OnDisable()
    {
        _homeButton.onClick.RemoveListener(Return);
    }

    private void Return()
    {
        SceneManager.LoadScene(0);
    }
}
