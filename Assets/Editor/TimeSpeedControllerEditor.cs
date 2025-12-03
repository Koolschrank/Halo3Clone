using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TimeSpeedController))]
public class TimeSpeedControllerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		// Get reference to target script
		TimeSpeedController controller = (TimeSpeedController)target;

		// Draw slider
		controller.timeSpeed = EditorGUILayout.Slider("Time Speed", controller.timeSpeed, 0f, 1f);

		// Add spacing
		EditorGUILayout.Space();

		// Draw button
		if (GUILayout.Button("Apply Time Speed"))
		{
			controller.ApplyTimeSpeed();
		}

		// Save changes to scene
		if (GUI.changed)
		{
			EditorUtility.SetDirty(controller);
		}
	}
}
