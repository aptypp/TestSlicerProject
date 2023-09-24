using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects.Extensions;

namespace MeshCutting
{
    public class MeshSlicer
    {
        private List<int> _positiveTriangles;
        private List<int> _negativeTriangles;

        private List<Vector2> _positiveUv;
        private List<Vector2> _negativeUv;

        private List<Vector3> _positiveVertices;
        private List<Vector3> _negativeVertices;

        private List<Vector3> _positiveNormals;
        private List<Vector3> _negativeNormals;

        public Mesh[] Slice(Mesh mesh, Quaternion meshRotation, Plane slicePlane)
        {
            _positiveTriangles = new List<int>(mesh.triangles.Length);
            _positiveVertices = new List<Vector3>(mesh.vertices.Length);
            _positiveNormals = new List<Vector3>(mesh.triangles.Length);
            _positiveUv = new List<Vector2>(mesh.triangles.Length);

            _negativeTriangles = new List<int>(mesh.triangles.Length);
            _negativeVertices = new List<Vector3>(mesh.vertices.Length);
            _negativeNormals = new List<Vector3>(mesh.triangles.Length);
            _negativeUv = new List<Vector2>(mesh.triangles.Length);

            for (int verticesIndex = 0; verticesIndex < mesh.triangles.Length; verticesIndex += 3)
            {
                int triangleIndex0 = mesh.triangles[verticesIndex];
                int triangleIndex1 = mesh.triangles[verticesIndex + 1];
                int triangleIndex2 = mesh.triangles[verticesIndex + 2];

                Vector3 vert0 = meshRotation * mesh.vertices[triangleIndex0];
                Vector3 vert1 = meshRotation * mesh.vertices[triangleIndex1];
                Vector3 vert2 = meshRotation * mesh.vertices[triangleIndex2];

                Vector2 uv0 = mesh.uv[triangleIndex0];
                Vector2 uv1 = mesh.uv[triangleIndex1];
                Vector2 uv2 = mesh.uv[triangleIndex2];

                bool point0Side = slicePlane.GetSide(vert0);
                bool point1Side = slicePlane.GetSide(vert1);
                bool point2Side = slicePlane.GetSide(vert2);

                if (point0Side == point1Side && point0Side == point2Side)
                {
                    AddTriangleToMesh(vert0, vert1, vert2, uv0, uv1, uv2, MeshSideExtension.BoolToMeshSide(point0Side));

                    continue;
                }

                if (point0Side != point1Side && point0Side != point2Side)
                {
                    MathfExtension.LinePlaneInterception(vert0, vert1, slicePlane, out Vector3 vert01);
                    MathfExtension.LinePlaneInterception(vert0, vert2, slicePlane, out Vector3 vert02);

                    Vector2 uv01 = MathfExtension.ComputeUvCoordinates(vert0, vert1, vert2, vert01, uv0, uv1, uv2);
                    Vector2 uv02 = MathfExtension.ComputeUvCoordinates(vert0, vert1, vert2, vert02, uv0, uv1, uv2);

                    AddTriangleToMesh(vert01, vert02, vert0, uv01, uv02, uv0, MeshSideExtension.BoolToMeshSide(point0Side));

                    AddTriangleToMesh(vert01, vert1, vert02, uv01, uv1, uv02, MeshSideExtension.BoolToMeshSide(!point0Side));

                    AddTriangleToMesh(vert2, vert02, vert1, uv2, uv02, uv1, MeshSideExtension.BoolToMeshSide(!point0Side));

                    continue;
                }

                if (point2Side != point0Side)
                {
                    MathfExtension.LinePlaneInterception(vert2, vert0, slicePlane, out Vector3 vert20);
                    MathfExtension.LinePlaneInterception(vert2, vert1, slicePlane, out Vector3 vert21);

                    Vector2 uv20 = MathfExtension.ComputeUvCoordinates(vert0, vert1, vert2, vert20, uv0, uv1, uv2);
                    Vector2 uv21 = MathfExtension.ComputeUvCoordinates(vert0, vert1, vert2, vert21, uv0, uv1, uv2);

                    AddTriangleToMesh(vert20, vert21, vert2, uv20, uv21, uv2, MeshSideExtension.BoolToMeshSide(point2Side));

                    AddTriangleToMesh(vert21, vert20, vert1, uv21, uv20, uv1, MeshSideExtension.BoolToMeshSide(!point2Side));

                    AddTriangleToMesh(vert1, vert20, vert0, uv1, uv20, uv0, MeshSideExtension.BoolToMeshSide(!point2Side));

                    continue;
                }

                MathfExtension.LinePlaneInterception(vert1, vert0, slicePlane, out Vector3 vert10);
                MathfExtension.LinePlaneInterception(vert1, vert2, slicePlane, out Vector3 vert12);

                Vector2 uv10 = MathfExtension.ComputeUvCoordinates(vert0, vert1, vert2, vert10, uv0, uv1, uv2);
                Vector2 uv12 = MathfExtension.ComputeUvCoordinates(vert0, vert1, vert2, vert12, uv0, uv1, uv2);

                AddTriangleToMesh(vert1, vert12, vert10, uv1, uv12, uv10, MeshSideExtension.BoolToMeshSide(point1Side));

                AddTriangleToMesh(vert10, vert12, vert2, uv10, uv12, uv2, MeshSideExtension.BoolToMeshSide(!point1Side));

                AddTriangleToMesh(vert10, vert2, vert0, uv10, uv2, uv0, MeshSideExtension.BoolToMeshSide(!point1Side));
            }

            List<Mesh> newMeshes = new List<Mesh>();

            if (_positiveVertices.Count > 0)
            {
                Mesh positiveMesh = new Mesh();
                positiveMesh.vertices = _positiveVertices.ToArray();
                positiveMesh.triangles = _positiveTriangles.ToArray();
                positiveMesh.normals = _positiveNormals.ToArray();
                positiveMesh.uv = _positiveUv.ToArray();

                newMeshes.Add(positiveMesh);
            }

            if (_negativeVertices.Count > 0)
            {
                Mesh negativeMesh = new Mesh();
                negativeMesh.vertices = _negativeVertices.ToArray();
                negativeMesh.triangles = _negativeTriangles.ToArray();
                negativeMesh.normals = _negativeNormals.ToArray();
                negativeMesh.uv = _negativeUv.ToArray();

                newMeshes.Add(negativeMesh);
            }

            _positiveTriangles = null;
            _positiveVertices = null;
            _positiveNormals = null;
            _positiveUv = null;
            _negativeTriangles = null;
            _negativeVertices = null;
            _negativeNormals = null;
            _negativeUv = null;

            return newMeshes.ToArray();
        }

        private void AddTriangleToMesh(in Vector3 vert0, in Vector3 vert1, in Vector3 vert2, in Vector2 uv0, in Vector2 uv1, in Vector2 uv2,
            MeshSide meshSide)
        {
            ICollection<Vector3> vertices = meshSide == MeshSide.Positive ? _positiveVertices : _negativeVertices;
            ICollection<int> triangles = meshSide == MeshSide.Positive ? _positiveTriangles : _negativeTriangles;
            ICollection<Vector3> normals = meshSide == MeshSide.Positive ? _positiveNormals : _negativeNormals;
            ICollection<Vector2> uvs = meshSide == MeshSide.Positive ? _positiveUv : _negativeUv;

            Vector3 normal = Vector3.Cross(vert1 - vert0, vert2 - vert0);

            vertices.Add(vert0);
            vertices.Add(vert1);
            vertices.Add(vert2);

            triangles.Add(vertices.Count - 3);
            triangles.Add(vertices.Count - 2);
            triangles.Add(vertices.Count - 1);

            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);

            uvs.Add(uv0);
            uvs.Add(uv1);
            uvs.Add(uv2);
        }
    }
}