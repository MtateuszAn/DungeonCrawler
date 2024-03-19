using System.Collections.Generic;
using System.Drawing;
using System;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;

public class Edge
{
    public GridElement Start { get; }
    public GridElement End { get; }

    public float weight;

    public Edge(GridElement start, GridElement end)
    {
        Start = start;
        End = end;
        weight = (float)Math.Sqrt(((start.x - end.x)* (start.x - end.x)) +((start.z - end.z)* (start.z - end.z)));
    }

    // Implementacja Equals oraz GetHashCode dla porównywania krawêdzi
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Edge other = (Edge)obj;
        return (Start.Equals(other.Start) && End.Equals(other.End)) ||
               (Start.Equals(other.End) && End.Equals(other.Start));
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Start.GetHashCode();
            hash = hash * 23 + End.GetHashCode();
            return hash;
        }
    }
    public bool HasVertex(GridElement point)
    {
        return Start.Equals(point) || End.Equals(point);
    }
}
public class Triangle
{
    public GridElement A { get; }
    public GridElement B { get; }
    public GridElement C { get; }

    public Edge AB { get; }
    public Edge BC { get; }
    public Edge CA { get; }

    public List<Edge> Edges { get; }

    public Triangle(GridElement a, GridElement b, GridElement c)
    {
        A = a;
        B = b;
        C = c;

        AB = new Edge(a, b);
        BC = new Edge(b, c);
        CA = new Edge(c, a);

        Edges = new List<Edge> { AB, BC, CA };
    }

    public bool HasEdge(Edge edge)
    {
        return Edges.Contains(edge);
    }

    public bool HasVertex(GridElement point)
    {
        return A.Equals(point) || B.Equals(point) || C.Equals(point);
    }

    public bool ContainsPoint(GridElement p)
    {
        double denominator = ((B.z - C.z) * (A.x - C.x) + (C.x - B.x ) * (A.z - C.z));
        double alpha = ((B.z - C.z) * (p.x - C.x) + (C.x - B.x) * (p.z - C.z)) / denominator;
        double beta = ((C.z - A.z) * (p.x - C.x) + (A.x - C.x) * (p.z - C.z)) / denominator;
        double gamma = 1.0 - alpha - beta;

        return alpha > 0 && beta > 0 && gamma > 0;
    }
}
public static class FindPath
{
    
    public static List<Edge> BowyerWatsonDelaunayTriangulation()
    {
        List<GridElement> points = MapGrid.instance.dorrs;

        // Tworzymy pocz¹tkowy trójk¹t, zawieraj¹cy wszystkie punkty
        double maxX = double.MinValue, maxY = double.MinValue, minX = double.MaxValue, minY = double.MaxValue;
        foreach (var point in points)
        {
            if (point.x > maxX)
                maxX = point.x;
            if (point.z > maxY)
                maxY = point.z;
            if (point.x < minX)
                minX = point.x;
            if (point.z < minY)
                minY = point.z;
        }

        double dx = maxX - minX;
        double dy = maxY - minY;
        double deltaMax = Math.Max(dx, dy);
        double midx = (minX + maxX) / 2.0;
        double midy = (minY + maxY) / 2.0;

        GridElement p1 = new GridElement(midx - 2 * deltaMax, midy - deltaMax);
        GridElement p2 = new GridElement(midx, midy + 2 * deltaMax);
        GridElement p3 = new GridElement(midx + 2 * deltaMax, midy - deltaMax);

        List<Triangle> triangles = new List<Triangle>();
        triangles.Add(new Triangle(p1, p2, p3));
        
        // Dodajemy punkty i aktualizujemy triangulacjê
        foreach (var point in points)
        {
            List<Triangle> badTriangles = new List<Triangle>();

            foreach (var triangle in triangles)
            {
                if (triangle.ContainsPoint(point))
                    badTriangles.Add(triangle);
            }

            List<Edge> polygon = new List<Edge>();

            foreach (var triangle in badTriangles)
            {
                foreach (var edge in triangle.Edges)
                {
                    bool shared = false;
                    foreach (var otherTriangle in badTriangles)
                    {
                        if (otherTriangle != triangle && otherTriangle.HasEdge(edge))
                        {
                            shared = true;
                            break;
                        }
                    }
                    if (!shared)
                    {
                        polygon.Add(edge);
                    }
                }
            }

            foreach (var triangle in badTriangles)
            {
                triangles.Remove(triangle);
            }

            foreach (var edge in polygon)
            {
                triangles.Add(new Triangle(edge.Start, edge.End, point));
            }
        }


        // Usuwamy trójk¹ty
        List<Edge> edges = new List<Edge>();
        foreach (var triangle in triangles)
        {
            foreach (var edge in triangle.Edges)
                edges.Add(edge);
        }
        List<Edge> edgesWithoutSuper = new List<Edge>();
        foreach (var edge in edges)
        {
            if (!edge.HasVertex(p1) && !edge.HasVertex(p2) && !edge.HasVertex(p3))
            {
                edgesWithoutSuper.Add(edge);
            }
        }
        
        return edgesWithoutSuper;
    }

    public static List<Edge> MSTreePrim(List<Edge> edges)
    {
        List<Edge> bestEdges = new List<Edge>();
     
        List<GridElement> visited = new List<GridElement>();

        visited.Add(edges[0].Start);
        bool empty = false;
        var i = 0;
        do
        {
            empty = false;
            /*foreach (var edge in edges)
            {
                if(visited.Contains(edge.Start) && !visited.Contains(edge.End))
                {
                    visited.Add(edge.End);
                    bestEdges.Add(edge);
                    empty=true;
                }
            }*/
            GridElement visitedEnd = null;
            Edge best = null;
            foreach( Edge edge in edges.Where( n=> visited.Contains(n.Start) && !visited.Contains(n.End) ))
            {
                if (best == null || best.weight > edge.weight)
                {
                    best = edge;
                    empty= true;
                    visitedEnd = edge.End;
                }  
            }
            if(best!=null && visitedEnd != null)
            {
                bestEdges.Add(best);
                visited.Add(visitedEnd);
            }
            i++;
        }while (empty && i<1000);

        return bestEdges;
    }
}
