using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Class responsible for generating a single face of terrain for the planet.
 * 3/6/21
 */
public class TerrainFace
{
    // Fields.
    private ShapeGenerator shapeGenerator;

    // Mesh for the shape and model.
    private Mesh mesh;

    // Level of detail, or number of vertices along the edge of a face.
    private int resolution;

    // Direction of the face.
    private Vector3 localUp;

    // Other axes of the face apart from the direction it is pointing towards.
    private Vector3 axisA;
    private Vector3 axisB;

    // Constructor for initializing fields and creating a new face.
    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        // Calculate axisA.
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        // Calculate axisB (perpendicular to axisA).
        axisB = Vector3.Cross(localUp, axisA);
    }

    // Generate the mesh of the face.
    public void ConstructMesh()
    {
        // Array of vertices (points on the plane)
        Vector3[] vertices = new Vector3[resolution * resolution];
        // Calculate the amount of triangles within the plane given the amount of vertices.
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0; // The current index (point) we are drawing the triangle from.
        Vector2[] uv = (mesh.uv.Length == vertices.Length) ? mesh.uv : new Vector2[vertices.Length]; // Ensure UVs are the correct size so it is consistent with planet resolution.

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                // Calculates the current index (which triangle we are creating).
                int i = x + y * resolution;

                // Keeps track of how complete the mesh is, used to define where the vertex should be on the face.
                Vector2 percent = new Vector2(x, y) / (resolution - 1); 
                
                // Places a vertex on the cube using the percent value above; the axes, which tells us how far along each axis we are; and the current shape settings.
                Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized; // Create a sphere by making all vertices the same distance away from the center.
                float unscaledElevation= shapeGenerator.CalculateUnscaledGeneration(pointOnUnitSphere);
                vertices[i] = pointOnUnitSphere * shapeGenerator.GetScaledElevation(unscaledElevation);
                // Get UV data.
                uv[i].y = unscaledElevation;
                // We divide the squares in the mesh into two triangles using the following formulas:
                // i, i + r (resolution, or number of vertices per line) + 1, i + r to determine the points of the first triangle.
                // i, i + 1, i + r + 1 for the second triangle.
                if (x != resolution - 1 && y != resolution - 1)
                {
                    // first triangle
                    triangles[triIndex] = i; // first vertex of first triangle
                    triangles[triIndex + 1] = i + resolution + 1; // second vertex of first triangle
                    triangles[triIndex + 2] = i + resolution; // third vertex of first triangle

                    // second triangle
                    triangles[triIndex + 3] = i; // first vertex of second triangle
                    triangles[triIndex + 4] = i + 1; // second vertex of second triangle
                    triangles[triIndex + 5] = i + resolution + 1; // third vertex of second triangle
                    triIndex += 6; // added six vertices, so we increment the current index by six.
                }
            }
        }
        // Clears current mesh data just in case we are viewing the face at a lower resolution, so nonexistent indeces are no longer referenced.
        mesh.Clear();
        
        // Assign data to the mesh. Ensures that data is not lost upon regeneration.
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uv;
    }

    // Store the biome information in the UV channel (2D coordinates for mapping 3D textures)
    public void UpdateUVs(ColorGenerator colorGenerator)
    {
        Vector2[] uv = mesh.uv;
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                // Calculates the current index (which triangle we are creating).
                int i = x + y * resolution;

                // Keeps track of how complete the mesh is, used to define where the vertex should be on the face.
                Vector2 percent = new Vector2(x, y) / (resolution - 1); 
                
                // Places a vertex on the cube using the percent value above; the axes, which tells us how far along each axis we are; and the current shape settings.
                Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized; // Create a sphere by making all vertices the same distance away from the center.

                uv[i].x = colorGenerator.BiomePercentFromPoint(pointOnUnitSphere);
            }
        }
        mesh.uv = uv;
    }
}
