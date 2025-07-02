using UnityEngine;

/// <summary>
/// 월드좌표계 point와 특정 평면에 맵핑된 mapped 평면상대좌표계 데이터
/// </summary>
public struct Point2DMapping
{
    public Point2DMapping(Vector3 point, Plane plane)
    {
        this.point = point;
        Vector3 normal = plane.normal;
        float distance = plane.distance;
        
        Vector3 planeOriginal = -distance * normal; // 평면 상대 좌표계의 원점
        Vector3 u = Vector3.Cross(normal, (Mathf.Abs(normal.y) < 0.999) ? Vector3.up : Vector3.right).normalized; // 평면 상대좌표계 u단위
        Vector3 v = Vector3.Cross(normal, u); // 평면 상대좌표계 v단위

        Vector3 rel = point - planeOriginal; // 좌표계 위치변환

        this.mapped = new Vector2(Vector3.Dot(rel, u),
                                  Vector3.Dot(rel, v));
    }

    public readonly Vector3 point;
    public readonly Vector2 mapped;
}
