using System.Collections.Generic;
using System.Linq;

public static class ConvexHullCalculator
{
    static List<Point2DMapping> MonotoneChain(List<Point2DMapping> points)
    {
        points.Sort((p1, p2) =>
        {
            int compare = p1.mapped.x.CompareTo(p2.mapped.x);

            if (compare == 0)
            {
                compare = p1.mapped.y.CompareTo(p2.mapped.y);
            }

            return compare;
        });

        List<Point2DMapping> lower = new List<Point2DMapping>();

        for (int i = 0; i < points.Count; i++)
        {
            Point2DMapping point = points[i];
            while (lower.Count >= 2 && CCW(lower[^2], lower[^1], point) <= 0)
            {
                lower.RemoveAt(lower.Count - 1);
            }

            lower.Add(point);
        }

        List<Point2DMapping> upper = new List<Point2DMapping>();

        for (int i = points.Count - 1; i >= 0; i--)
        {
            Point2DMapping point = points[i];
            while (upper.Count >= 2 && CCW(upper[^2], upper[^1], point) <= 0)
            {
                upper.RemoveAt(upper.Count - 1);
            }

            upper.Add(point);
        }

        lower.RemoveAt(lower.Count - 1);
        upper.RemoveAt(upper.Count - 1);
        lower.AddRange(upper);
        return lower;
    }


    static int CCW(Point2DMapping p1, Point2DMapping p2, Point2DMapping p3)
    {
        long crossK = (long)(p2.mapped.x - p1.mapped.x) * (long)(p3.mapped.y - p1.mapped.y)
                     - (long)(p2.mapped.y - p1.mapped.y) * (long)(p3.mapped.x - p1.mapped.x);

        if (crossK == 0)
        {
            return 0;
        }

        return crossK > 0 ? 1 : -1;
    }
    
    
}