using System;
using UnityEngine;

public class Test : MonoBehaviour
{
    public MeshFilter targetfilter;
    public Vector3 planeNormal = Vector3.one;
    public Vector3 planeOffset = new Vector3(0.1f, 0.2f, -0.1f);

    private void OnGUI()
    {
        if (GUI.Button(new Rect(20, 40, 80, 20), "Slice!"))
        {
            Plane plane = new Plane(planeNormal, planeOffset);
            MeshSlicer.Slice(targetfilter.mesh, plane, out Mesh lower, out Mesh upper);
            
            GameObject lowerGo = new GameObject("Lower");
            lowerGo.AddComponent<MeshFilter>().mesh = lower;
            lowerGo.AddComponent<MeshRenderer>().material = targetfilter.GetComponent<MeshRenderer>().sharedMaterial;
            
            GameObject upperGo = new GameObject("upper");
            upperGo.AddComponent<MeshFilter>().mesh = upper;
            upperGo.AddComponent<MeshRenderer>().material = targetfilter.GetComponent<MeshRenderer>().sharedMaterial;
        }
    }
}