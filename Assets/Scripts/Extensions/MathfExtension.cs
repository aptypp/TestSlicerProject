using UnityEngine;

namespace UnityTemplateProjects.Extensions
{
    public static class MathfExtension
    {
        public static void ComputeBarycentricCoordinates(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector3 point, out float alpha,
            out float beta, out float gamma)
        {
            Vector3 edge1 = vertex2 - vertex1;
            Vector3 edge2 = vertex3 - vertex1;
            Vector3 edge3 = vertex3 - vertex2;

            Vector3 toPoint1 = point - vertex1;
            Vector3 toPoint2 = point - vertex2;

            Vector3 normal = Vector3.Cross(edge1, edge2);
            float inverseArea = 1.0f / normal.magnitude;

            alpha = Vector3.Cross(toPoint2, edge3).magnitude * inverseArea;
            beta = Vector3.Cross(toPoint1, edge2).magnitude * inverseArea;
            gamma = 1.0f - alpha - beta;
        }

        public static float LinePlaneInterception(in Vector3 point0, in Vector3 point1, Plane plane, out Vector3 interceptionPoint)
        {
            Ray rayFromPoint0ToPoint1 = new Ray(point0, point1 - point0);

            plane.Raycast(rayFromPoint0ToPoint1, out float distance);
            interceptionPoint = rayFromPoint0ToPoint1.GetPoint(distance);

            return distance;
        }

        public static Vector2 ComputeUvCoordinates(in Vector3 vert0, in Vector3 vert1, in Vector3 vert2, in Vector3 newVert, in Vector2 uv0,
            in Vector2 uv1, in Vector2 uv2)
        {
            MathfExtension.ComputeBarycentricCoordinates(vert0, vert1, vert2, newVert, out float alpha, out float beta, out float gamma);

            return new Vector2(uv0.x * alpha + uv1.x * beta + uv2.x * gamma, uv0.y * alpha + uv1.y * beta + uv2.y * gamma);
        }
    }
}