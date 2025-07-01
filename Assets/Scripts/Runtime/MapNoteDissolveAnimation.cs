using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNoteDissolveAnimation : MonoBehaviour
{
    private readonly int DISSOLVE_ID = Shader.PropertyToID("_Dissolve");
    [SerializeField] private float _duration = 2.0f;
    
    private MaterialPropertyBlock _block;

    private void Awake()
    {
        _block = new MaterialPropertyBlock();
    }

    public void PlayC_Dissolve(Renderer renderer, Action onDestroyComplete = null)
    {
        StartCoroutine(C_Dissolve(renderer, onDestroyComplete));
    }

    IEnumerator C_Dissolve(Renderer renderer, Action onDestroyComplete)
    {
        float animationSpeed = 1f / _duration;
        float dissolve = 0f;
        float elapsedTime = 0f;

        while (dissolve < 1f)
        {
            dissolve = elapsedTime * animationSpeed;
            
            elapsedTime += Time.deltaTime;
            
            renderer.SetPropertyBlock(_block);
            _block.SetFloat(DISSOLVE_ID, dissolve);
            renderer.SetPropertyBlock(_block);
            yield return null;
        }

        onDestroyComplete?.Invoke();
    }
}
