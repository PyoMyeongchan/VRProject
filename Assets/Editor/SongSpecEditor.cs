#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SongSpec))]
public class SongSpecEditor : Editor
{
    const float THRESHOLD_GAIN = 1.2f;
    AudioClip _cachedAudioClip;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SongSpec songSpec = (SongSpec)target;

        using (new EditorGUILayout.VerticalScope())
        {
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Audio Spectrum Sampling", EditorStyles.boldLabel);
            }

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {

                _cachedAudioClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip",
                    _cachedAudioClip,
                    typeof(AudioClip),
                    false);
            }

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (_cachedAudioClip == null)
                {
                    EditorGUI.BeginDisabledGroup(true);
                }

                if (GUILayout.Button("Bake peaks"))
                {
                    List<float> peaks = ExtractPeaks(_cachedAudioClip, songSpec.bpm);
                    songSpec.BakePeaks(_cachedAudioClip, peaks);
                    
                    EditorUtility.SetDirty(songSpec);
                    AssetDatabase.SaveAssets();
                }
                
                if (_cachedAudioClip == null)
                {
                    EditorGUI.EndDisabledGroup();
                }
            }
        }
    }

    List<float> ExtractPeaks(AudioClip audioClip, float bpm)
    {
        //채널 평균
        int sampleCount = audioClip.samples;
        int channelCount = audioClip.channels;
        
        float[] raw = new float[sampleCount * channelCount];
        audioClip.GetData(raw, 0);
        
        //채널들 평균(모노 채널로 간단 변환)
        float[] mono = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            float sum = 0f;
            for (int j = 0; j < channelCount; j++)
            {
                sum += raw[i * channelCount + j];
            }
            
            mono[i] = sum / channelCount;
        }
        
        //볼륨 값 확인용 RMS (Root Mean Square)
        float windowSize = 25 * audioClip.frequency / bpm;
        int windowCount = sampleCount / Mathf.FloorToInt(windowSize);
        float[] rmsArr = new float[windowCount];
        float meanRms = 0f;

        for (int i = 0; i < windowCount; i++)
        {
            double acc = 0;
            int start = i * Mathf.FloorToInt(windowSize);

            for (int j = 0; j < windowSize; j++)
            {
                float s = mono[start + j];
                acc += s * s;
            }
            float rms = Mathf.Sqrt((float)acc / windowSize);
            rmsArr[i] = rms;
            meanRms += rms;
        }

        meanRms /= windowCount;

        double stdSum = 0;
        
        //표준편차
        for (int i = 0; i < windowCount; i++)
        {
            float d = rmsArr[i] - meanRms;
            stdSum = d * d;
        }

        stdSum /= (double)windowCount;
        float stdRms = Mathf.Sqrt((float)stdSum);
        
        // 임계값 설정(평균값 + 임계계수 + 표준편차)
        float threshold = meanRms + THRESHOLD_GAIN * stdRms;
        
        //임계값 이상만 추출
        List<float> peaks = new List<float>();
        float windowSec = windowSize / (float)audioClip.frequency;

        for (int i = 0; i < windowCount; i++)
        {
            if (rmsArr[i] >= threshold)
            {
                float time = (i + 0.5f) * windowSec;
                peaks.Add(time);
            }
        }

        return peaks;

    }
        
    
}
#endif
