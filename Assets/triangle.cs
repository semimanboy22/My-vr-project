using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TriangleMesh : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = new Mesh();

        // Triangle vertices
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0)
        };

        // Vertex order for the triangle
        int[] triangles = new int[]
        {
            0, 1, 2
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}