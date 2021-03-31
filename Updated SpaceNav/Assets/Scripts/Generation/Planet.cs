using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Class responsible for creating six terrain faces and determining which direction they are attributed to.
 * 3/6/21
 */
 // All scripts in this folder were coded with the following tutorial series by Sebastian Lague: https://www.youtube.com/watch?v=QN39W020LqU&list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8
public class Planet : MonoBehaviour
{
    // Limit the range of the resolution (256 since 256^2 is the maximum amount of vertices a mesh can have).
    [Range(2, 256)]
    public int resolution = 10;  // Number of vertices per line.
    public bool autoUpdate = true; // Automatically update the mesh when changes are made.
    public enum FaceRenderMask {All, Top, Bottom, Left, Right, Front, Back}
    public FaceRenderMask faceRenderMask;

    // Arrays that contain the data for the six faces.
    // Save in Unity editor and hide in the inspector so it is not visible.
    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;

    // Planet appearance generators.
    ShapeGenerator shapeGenerator = new ShapeGenerator();
    ColorGenerator colorGenerator = new ColorGenerator();
    private void Awake()
    {
        GeneratePlanet(); // Generate the planet on game start.
    }
    
    // Create the arrays and mesh data.
    private void Initialize() 
    {
        // Assign the current appearance settings.
        shapeGenerator.UpdateSettings(shapeSettings);
        colorGenerator.UpdateSettings(colorSettings);

        // Only reinitialize mesh filters if the array is currently null or has no elements since we don't want to create six new mesh filters each time this is initialized.
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6]; // Creates 6 mesh filters to display the terrain face.
        }
        terrainFaces = new TerrainFace[6]; // An array of the six terrain faces.

        Vector3[] directions = {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back}; // Array of different directions the current terrain can face.
        for(int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh"); // Create a new mesh.
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>(); // Creates a new mesh renderer.
                meshFilters[i] = meshObj.AddComponent<MeshFilter>(); // Assign a mesh filter to the current index.
                meshFilters[i].sharedMesh = new Mesh(); // Assign a new mesh to the current index.
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial; // Assign the texture of the planet.

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]); // Create a new terrain face and assign it to the current index.

            // Calculate the current selected face.
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace); // Only render the active face.
        }
    }

    // Create the planet.
    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    // Update the planet's appearance if the shape is manipulated and autoUpdate is true.
    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    // Update the planet's appearance if the color is manipulated and autoUpdate is true.
    public void OnColorSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColors();
        }
    }

    // Create the mesh.
    private void GenerateMesh()
    {
        // Loop through all mesh filters and construct a new mesh.
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf) // Only generate a mesh if the current face is active.
            {
                terrainFaces[i].ConstructMesh();
            }
        }

        colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    // Loop through the meshes and apply the colors based on what the color settings are.
    private void GenerateColors()
    {
        colorGenerator.UpdateColors();
        // Loop through all terrain faces and update the uv data.
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf) // Only generate a mesh if the current face is active.
            {
                terrainFaces[i].UpdateUVs(colorGenerator);
            }
        }
    }
}
