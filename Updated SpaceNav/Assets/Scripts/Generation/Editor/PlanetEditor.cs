using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Creates a new editor tab for easily creating planets directly through the Unity Editor.
[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    // Reference to the planet.
    private Planet planet;

    // Define editors.
    private Editor shapeEditor;
    private Editor colorEditor;

    // Implement OnInspectorGUI() to create a custom inspector.
    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            // Derive from OnInspectorGUI()
            base.OnInspectorGUI();
            if (check.changed)
            {
                planet.GeneratePlanet(); // Update the planet if changes are made.
            }
        }

        // Generate a planet whent this button is pressed.
        if (GUILayout.Button("Generate Planet"))
        {
            planet.GeneratePlanet();
        }

        // Create two seperate edetior tabs for both shape settings and color settings.
        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(planet.colorSettings, planet.OnColorSettingsUpdated, ref planet.colorSettingsFoldout, ref colorEditor);
    }

    // Create the settings editor for the shape and color settings, then add it to the Unity Engine.
    // Pass in last two by reference so that the foldout value in the planet class can actually be changed.
    void DrawSettingsEditor(UnityEngine.Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            // Check if the foldout arrow is clicked.
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings); // Create a title bar for each editor.

            // Check if any of the planet's attributes have changed.
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if(foldout) // If the arrow is clicked, draw the editor.
                {
                    CreateCachedEditor(settings, null, ref editor); // Save the editor that is passed in by reference so we do not have to create a new one each time it is opened.
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {
                        if (onSettingsUpdated != null)
                        {
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
    }

    // Initialize the planet.
    private void OnEnable()
    {
        planet = (Planet)target;
    }
}
