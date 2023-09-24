namespace MeshCutting
{
    public static class MeshSideExtension
    {
        public static MeshSide BoolToMeshSide(bool value) => value ? MeshSide.Positive : MeshSide.Negative;
    }
}