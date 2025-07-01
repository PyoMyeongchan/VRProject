using UnityEngine;

public struct Line
{
    public Line( Vector3 a, Vector3 b)
    {
        A = a;
        B = b;
    }

    public readonly Vector3 A;
    public readonly Vector3 B;
    
    public float Length()
    {
        return Vector3.Distance(A, B);
    }
}