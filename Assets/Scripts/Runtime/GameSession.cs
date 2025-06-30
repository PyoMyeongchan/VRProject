using UnityEngine;

[CreateAssetMenu(fileName = "GameSession", menuName = "Scriptable Objects/GameSession")]
public class GameSession : ScriptableObject
{
    public SongSpec selectedSongSpec;
}
