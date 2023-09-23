using UnityEngine;

namespace MeshCutting
{
    [RequireComponent(typeof(MeshFilter))]
    public class TestMeshCutter : MonoBehaviour
    {
        [SerializeField]
        private Transform _slicePlane;

        [SerializeField]
        private CustomMeshRenderer _customMeshRendererPrefab;

        private MeshSlicer _meshSlicer;
        private MeshFilter _meshFilter;

        private void Awake()
        {
            _meshSlicer = new MeshSlicer();
            _meshFilter = GetComponent<MeshFilter>();
        }

        private void Start()
        {
            Mesh[] meshes = _meshSlicer.Slice(_meshFilter.mesh, new Plane(_slicePlane.forward, _slicePlane.position - transform.position));

            for (int meshIndex = 0; meshIndex < meshes.Length; meshIndex++)
            {
                CustomMeshRenderer newMeshRenderer = Instantiate(_customMeshRendererPrefab);

                newMeshRenderer.SetMesh(meshes[meshIndex]);
            }
        }
    }
}