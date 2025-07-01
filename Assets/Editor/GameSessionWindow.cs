#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public class GameSessionWindow : EditorWindow
{
    [MenuItem("Window/Game Session")]
    static void Open() => GetWindow<GameSessionWindow>("Game Session");

    private GameSession _cachedTarget;
    private SerializedObject _cachedTargetSerialized;

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnplayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnplayModeStateChanged;
    }

    private void OnplayModeStateChanged(PlayModeStateChange changeState)
    {
        _cachedTarget = GameManager.gameSession;
        if (_cachedTarget == null)
        {
            Debug.LogWarning("GameManager.gameSession is null during play mode state change.");
            return;
        }
        _cachedTargetSerialized = new SerializedObject(_cachedTarget);
        _cachedTargetSerialized.Update();
        Repaint();
    }

    private void OnGUI()
    {
        //play 모드에서만 감시가능
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Game Session is not playing.", MessageType.Warning);
            return;
        }
        
        if (_cachedTarget == null)
        {
            if (GameManager.gameSession != null)
            {
                _cachedTarget = GameManager.gameSession;
                _cachedTargetSerialized = new SerializedObject(_cachedTarget);
            }
            else
            {
                EditorGUILayout.HelpBox("Game Session is not playing.", MessageType.Warning);
                return;
            }
        }

        _cachedTargetSerialized.Update();
        EditorGUILayout.PropertyField(_cachedTargetSerialized.FindProperty("selectedSongSpec"));
        EditorGUILayout.PropertyField(_cachedTargetSerialized.FindProperty("playSpeed"));
        _cachedTargetSerialized.ApplyModifiedProperties();
    }

}
#endif
