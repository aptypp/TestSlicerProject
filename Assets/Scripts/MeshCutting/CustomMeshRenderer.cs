using UnityEngine;

namespace MeshCutting
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class CustomMeshRenderer : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer _meshRenderer;

        [SerializeField]
        private MeshFilter _meshFilter;

        public void SetMesh(Mesh mesh)
        {
            _meshFilter.mesh = mesh;
        }
    }
}