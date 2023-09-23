using UnityEngine;

namespace MeshCutting
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MeshCreator : MonoBehaviour
    {
        [SerializeField]
        private Texture _texture;
        [SerializeField]
        private Texture _normalTexture;

        [SerializeField]
        private Material _material;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            CreateMesh();
        }

        private void CreateMesh()
        {
            Mesh mesh = new();

            mesh.name = "Procedural mesh";
            mesh.vertices = new Vector3[] { Vector3.zero, Vector3.right, Vector3.up, new Vector3(1f, 1f) };
            mesh.triangles = new int[] { 0, 2, 1, 1, 2, 3 };
            mesh.normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
            mesh.uv = new Vector2[] { Vector2.zero, Vector2.right, Vector2.up, Vector2.one };
            mesh.tangents = new Vector4[] { new Vector4(1, 0, 0, -1), new Vector4(1, 0, 0, -1), new Vector4(1, 0, 0, -1), new Vector4(1, 0, 0, -1) };

            _meshFilter.mesh = mesh;

            _meshRenderer.material = new Material(_material) { mainTexture = _texture };
        }
    }
}