
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MeshSlicer
{
    const float EPS = 0.0001f;
    
    public static void Slice(Mesh mesh, Plane plane, out Mesh lower, out Mesh upper)
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        
        List<Vector3> lowerVertices = new List<Vector3>();
        List<Vector3> upperVertices = new List<Vector3>();
        List<int> lowerTriangles = new List<int>();
        List<int> upperTriangles = new List<int>();
        
        List<Vector3> sectionPoints = new List<Vector3>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i1 = triangles[i];
            int i2 = triangles[i + 1];
            int i3 = triangles[i + 2];

            Vector3 p1 = vertices[i1];
            Vector3 p2 = vertices[i2];
            Vector3 p3 = vertices[i3];
            
            float d1 = SignedDistance(p1, plane);
            float d2 = SignedDistance(p2, plane);
            float d3 = SignedDistance(p3, plane);

            bool up1 = d1 >= EPS;
            bool up2 = d2 >= EPS;
            bool up3 = d3 >= EPS;
            bool down1 = d1 <= -EPS;
            bool down2 = d2 <= -EPS;
            bool down3 = d3 <= -EPS;

            if (up1 && up2 && up3)
            {
                upperTriangles.Add(i1);
                upperTriangles.Add(i2);
                upperTriangles.Add(i3);
                continue;
            }
            
            if (down1 && down2 && down3)
            {
                lowerTriangles.Add(i1);
                lowerTriangles.Add(i2);
                lowerTriangles.Add(i3);
                continue;
            }
            
            Clip(p1, p2, p3, true, d1, d2, d3, plane, upperVertices, upperTriangles, sectionPoints);
            Clip(p1, p2, p3, false, d1, d2, d3, plane, lowerVertices, lowerTriangles, sectionPoints);
        }
        // 단면이 존재할 경우
        if (sectionPoints.Count > 3)
        {
            List<Point2DMapping> point2DMappings = sectionPoints.Select(point => new Point2DMapping(point, plane)).ToList();
            List<Point2DMapping> capHull = ConvexHullCalculator.MonotoneChain(point2DMappings);
            TriangulateCap(capHull, true, upperVertices, upperTriangles);
            TriangulateCap(capHull, false, lowerVertices, lowerTriangles);
        }

        upper = new Mesh();
        upper.SetVertices(upperVertices);
        upper.SetTriangles(upperTriangles, 0);
        upper.RecalculateNormals();
            
        lower = new Mesh();
        lower.SetVertices(lowerVertices);
        lower.SetTriangles(lowerTriangles, 0);
        lower.RecalculateNormals();
    }

    static void TriangulateCap(List<Point2DMapping> hull, bool isUpper, List<Vector3> targetVertices, List<int> targetTriangles)
    {
        if (hull.Count < 3)
        {
            return;
        }
        
        int i0 = 0;

        for (int i = 1; i < hull.Count; i++)
        {
            if (isUpper)
            {
                targetVertices.Add(hull[i0].point);
                targetTriangles.Add(targetVertices.Count - 1);
                targetVertices.Add(hull[i].point);
                targetTriangles.Add(targetVertices.Count - 1);
                targetVertices.Add(hull[i + 1].point);
                targetTriangles.Add(targetVertices.Count - 1);
            }
            else
            {
                targetVertices.Add(hull[i0].point);
                targetTriangles.Add(targetVertices.Count - 1);
                targetVertices.Add(hull[i + 1].point);
                targetTriangles.Add(targetVertices.Count - 1);
                targetVertices.Add(hull[i].point);
                targetTriangles.Add(targetVertices.Count - 1);
                
            }
        }
    }

    static float SignedDistance(Vector3 point, Plane plane)
    {
        return Vector3.Dot(plane.normal, point) + plane.distance;
    }

    /// <summary>
    /// 평면에 교차하는 삼각형을 자르는 함수
    /// TODO : Vertices 메모리 최적화 - 이미 사용하던 Vertex를 다른 삼각형에서 쓸때, Vertices 배열 늘리지말고 원래 쓰던 위치를 가리키게 해야함
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="isUpper"></param>
    /// <param name="d1"></param>
    /// <param name="d2"></param>
    /// <param name="d3"></param>
    /// <param name="plane"></param>
    /// <param name="outTriangles"></param>
    static void Clip(Vector3 p1, Vector3 p2, Vector3 p3,
                     bool isUpper,
                     float d1, float d2, float d3,
                     Plane plane,
                     List<Vector3> outTrianglesVertices,
                     List<int> outTriangles,
                     List<Vector3> outSectionVertices)
    {
        List<Vector3> polygon = new List<Vector3>();
        List<Vector3> sectionVertices = new List<Vector3>();
        
        bool isUpOrDown = isUpper ? d1 >= EPS : d1 <= -EPS;
        bool isUpOrDown2 = isUpper ? d2 >= EPS : d2 <= -EPS;
        bool isUpOrDown3 = isUpper ? d3 >= EPS : d3 <= -EPS;

        if (isUpOrDown)
        {
            polygon.Add(p1);
        }

        if (isUpOrDown ^ isUpOrDown2)
        {
            polygon.Add(Intersect(p1, p2, d1, d2, plane, sectionVertices));
        }
        
        if (isUpOrDown2)
        {
            polygon.Add(p2);
        }

        if (isUpOrDown2 ^ isUpOrDown3)
        {
            polygon.Add(Intersect(p2, p3, d2, d3, plane, sectionVertices));
        }
        
        if (isUpOrDown3)
        {
            polygon.Add(p3);
        }

        if (isUpOrDown3 ^ isUpOrDown)
        {
            polygon.Add(Intersect(p3, p1, d3, d1, plane, sectionVertices));
        }

        if (polygon.Count < 3)
        {  
            return;
        }
        if (isUpper)
        {
            outTrianglesVertices.Add(polygon[0]);
            outTriangles.Add(outTrianglesVertices.Count - 1);
            outTrianglesVertices.Add(polygon[1]);
            outTriangles.Add(outTrianglesVertices.Count - 1);
            outTrianglesVertices.Add(polygon[2]);
            outTriangles.Add(outTrianglesVertices.Count - 1);

            if (polygon.Count == 4)
            {
                outTrianglesVertices.Add(polygon[0]);
                outTriangles.Add(outTrianglesVertices.Count - 1);
                outTrianglesVertices.Add(polygon[2]);
                outTriangles.Add(outTrianglesVertices.Count - 1);
                outTrianglesVertices.Add(polygon[3]);
                outTriangles.Add(outTrianglesVertices.Count - 1);
            }
        }
        else
        {
            outTrianglesVertices.Add(polygon[2]);
            outTriangles.Add(outTrianglesVertices.Count - 1);
            outTrianglesVertices.Add(polygon[1]);
            outTriangles.Add(outTrianglesVertices.Count - 1);
            outTrianglesVertices.Add(polygon[0]);
            outTriangles.Add(outTrianglesVertices.Count - 1);

            if (polygon.Count == 4)
            {
                outTrianglesVertices.Add(polygon[2]);
                outTriangles.Add(outTrianglesVertices.Count - 1);
                outTrianglesVertices.Add(polygon[3]);
                outTriangles.Add(outTrianglesVertices.Count - 1);
                outTrianglesVertices.Add(polygon[0]);
                outTriangles.Add(outTrianglesVertices.Count - 1);
            }
        }
        
        outSectionVertices.AddRange(sectionVertices);
    }

    static Vector3 Intersect(Vector3 p1, Vector3 p2, float d1, float d2, Plane plane, List<Vector3> sectionVertices)
    {
        float t = d1 / (d1 - d2);
        Vector3 result =  p1 + t * (p2 - p1);
        sectionVertices.Add(result);
        return result;
    }
}