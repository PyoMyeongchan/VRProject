using UnityEngine;

public struct Plane
{
    /// <summary>
    /// 법선벡터와 이 평면이 법선벡터로부터 얼마만큼 떨어져있나
    /// </summary>
    /// <param name="normal">법선 벡터</param>
    /// <param name="point">평면 위의 임의의 점</param>
    public Plane(Vector3 normal, Vector3 point)
    {
        this.normal = normal;
        distance = -Vector3.Dot(normal, point);
    }

    public Plane(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        normal =Vector3.Cross(p2 - p1, p3 - p1).normalized;
        distance = -Vector3.Dot(normal, p1);
    }

    public Vector3 normal;
    public float distance;
}