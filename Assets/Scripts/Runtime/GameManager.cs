using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager
{
    public static GameSession gameSession { get; private set; }

    [RuntimeInitializeOnLoadMethod]
    public static void Bootstrap()
    {
        SceneManager.LoadScene("MetaGameScene");
        gameSession = ScriptableObject.CreateInstance<GameSession>();
    }
}
