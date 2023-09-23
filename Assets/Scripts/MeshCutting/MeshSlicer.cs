using System.Collections.Generic;
using UnityEngine;

namespace MeshCutting
{
    public class MeshSlicer
    {
        public Mesh[] Slice(Mesh mesh, Plane plane)
        {
            List<Vector3> positiveVertices = new List<Vector3>(mesh.vertices.Length);
            List<Vector3> positiveNormals = new List<Vector3>(mesh.triangles.Length);
            List<Vector2> positiveUv = new List<Vector2>(mesh.triangles.Length);
            List<int> positiveTriangles = new List<int>(mesh.triangles.Length);

            List<Vector3> negativeVertices = new List<Vector3>(mesh.vertices.Length);
            List<Vector3> negativeNormals = new List<Vector3>(mesh.triangles.Length);
            List<Vector2> negativeUv = new List<Vector2>(mesh.triangles.Length);
            List<int> negativeTriangles = new List<int>(mesh.triangles.Length);

            for (int verticesIndex = 0; verticesIndex < mesh.triangles.Length; verticesIndex += 3)
            {
                Vector3 pointI0 = mesh.vertices[mesh.triangles[verticesIndex]];
                Vector3 pointI1 = mesh.vertices[mesh.triangles[verticesIndex + 1]];
                Vector3 pointI2 = mesh.vertices[mesh.triangles[verticesIndex + 2]];

                bool point0Side = plane.GetSide(pointI0);
                bool point1Side = plane.GetSide(pointI1);
                bool point2Side = plane.GetSide(pointI2);

                if (point0Side == point1Side && point0Side == point2Side)
                {
                    if (point0Side)
                    {
                        AddTriangleToMesh(pointI0, pointI1, pointI2, positiveVertices, positiveTriangles);
                    }
                    else
                    {
                        AddTriangleToMesh(pointI0, pointI1, pointI2, negativeVertices, negativeTriangles);
                    }

                    continue;
                }

                if (point0Side != point1Side && point0Side != point2Side)
                {
                    LinePlaneInterception(pointI0, pointI1, plane, out Vector3 interceptionPoint01);
                    LinePlaneInterception(pointI0, pointI2, plane, out Vector3 interceptionPoint02);

                    if (point0Side)
                    {
                        AddTriangleToMesh(interceptionPoint01, interceptionPoint02, pointI0, positiveVertices, positiveTriangles);

                        AddTriangleToMesh(interceptionPoint01, pointI1, interceptionPoint02, negativeVertices, negativeTriangles);

                        AddTriangleToMesh(pointI2, interceptionPoint02, pointI1, negativeVertices, negativeTriangles);
                    }
                    else
                    {
                        AddTriangleToMesh(interceptionPoint01, interceptionPoint02, pointI0, negativeVertices, negativeTriangles);

                        AddTriangleToMesh(interceptionPoint01, pointI1, interceptionPoint02, positiveVertices, positiveTriangles);

                        AddTriangleToMesh(pointI2, interceptionPoint02, pointI1, positiveVertices, positiveTriangles);
                    }

                    continue;
                }

                if (point2Side != point1Side && point2Side != point0Side)
                {
                    LinePlaneInterception(pointI2, pointI0, plane, out Vector3 interceptionPoint20);
                    LinePlaneInterception(pointI2, pointI1, plane, out Vector3 interceptionPoint21);

                    if (point2Side)
                    {
                        AddTriangleToMesh(interceptionPoint20, interceptionPoint21, pointI2, positiveVertices, positiveTriangles);

                        AddTriangleToMesh(interceptionPoint21, interceptionPoint20, pointI1, negativeVertices, negativeTriangles);

                        AddTriangleToMesh(pointI1, interceptionPoint20, pointI0, negativeVertices, negativeTriangles);
                    }
                    else
                    {
                        AddTriangleToMesh(interceptionPoint20, interceptionPoint21, pointI2, negativeVertices, negativeTriangles);

                        AddTriangleToMesh(interceptionPoint21, interceptionPoint20, pointI1, positiveVertices, positiveTriangles);

                        AddTriangleToMesh(pointI1, interceptionPoint20, pointI0, positiveVertices, positiveTriangles);
                    }

                    continue;
                }

                if (point1Side != point0Side && point1Side != point2Side)
                {
                    LinePlaneInterception(pointI1, pointI0, plane, out Vector3 interceptionPoint10);
                    LinePlaneInterception(pointI1, pointI2, plane, out Vector3 interceptionPoint12);

                    if (point1Side)
                    {
                        AddTriangleToMesh(pointI1, interceptionPoint12, interceptionPoint10, positiveVertices, positiveTriangles);

                        AddTriangleToMesh(interceptionPoint10, interceptionPoint12, pointI2, negativeVertices, negativeTriangles);

                        AddTriangleToMesh(interceptionPoint10, pointI2, pointI0, negativeVertices, negativeTriangles);
                    }
                    else
                    {
                        AddTriangleToMesh(pointI1, interceptionPoint12, interceptionPoint10, negativeVertices, negativeTriangles);

                        AddTriangleToMesh(interceptionPoint10, interceptionPoint12, pointI2, positiveVertices, positiveTriangles);

                        AddTriangleToMesh(interceptionPoint10, pointI2, pointI0, positiveVertices, positiveTriangles);
                    }

                    continue;
                }
            }

            List<Mesh> newMeshes = new List<Mesh>();

            if (positiveVertices.Count > 0)
            {
                Mesh positiveMesh = new Mesh();
                positiveMesh.vertices = positiveVertices.ToArray();
                positiveMesh.triangles = positiveTriangles.ToArray();

                newMeshes.Add(positiveMesh);
            }

            if (negativeVertices.Count > 0)
            {
                Mesh negativeMesh = new Mesh();
                negativeMesh.vertices = negativeVertices.ToArray();
                negativeMesh.triangles = negativeTriangles.ToArray();

                newMeshes.Add(negativeMesh);
            }

            return newMeshes.ToArray();
        }

        private void AddTriangleToMesh(in Vector3 point0, in Vector3 point1, in Vector3 point2, ICollection<Vector3> vertices,
            ICollection<int> triangles)
        {
            vertices.Add(point0);
            vertices.Add(point1);
            vertices.Add(point2);

            triangles.Add(vertices.Count - 3);
            triangles.Add(vertices.Count - 2);
            triangles.Add(vertices.Count - 1);
        }

        private float LinePlaneInterception(in Vector3 point0, in Vector3 point1, Plane plane, out Vector3 interceptionPoint)
        {
            Ray rayFromPoint0ToPoint1 = new Ray(point0, point1 - point0);

            plane.Raycast(rayFromPoint0ToPoint1, out float distance);
            interceptionPoint = rayFromPoint0ToPoint1.GetPoint(distance);

            return distance;
        }
    }
}