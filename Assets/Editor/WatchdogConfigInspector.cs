using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

[CustomEditor(typeof(WatchdogConfiguration))]
public class WatchdogConfigInspector : Editor {

	// int[] selected = new int[10];
	bool[] foldouts = new bool[10]; 

	string[] thresholdType = new string[]{">", "<"};


	public override void OnInspectorGUI() {
		WatchdogConfiguration config = (WatchdogConfiguration)target;
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Profiler Properties", EditorStyles.boldLabel);
		EditorGUILayout.Separator();

		GUILayoutOption[] options;			
		int labelWidthCol1 = 40; 
		int spaceAfterCol1 = 40;

		int count = 0;
		foreach (PropertyWatcher watcher in config.watchers) {

			// GUILayoutOption[] options = new GUILayoutOption[]{ GUILayout.MinWidth(100), GUILayout.};
			watcher.Index = EditorGUILayout.Popup("Slot "+watcher.Slot, watcher.Index, config.profilerPropertiesFormatted);
			if (watcher.Index > 0) {
				watcher.Property = config.profilerProperties[watcher.Index-1];
			}
			
			options = new GUILayoutOption[]{ GUILayout.ExpandWidth(false)};
			GUILayoutOption fixedWidth = GUILayout.Width(EditorGUIUtility.currentViewWidth);				

			EditorGUI.indentLevel++;
			foldouts[count] = EditorGUILayout.Foldout(foldouts[count], "Alarm");
			if (foldouts[count++]) {

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(20);

				EditorGUILayout.BeginVertical();
					//Alarm Zeile
					EditorGUILayout.BeginHorizontal(GUILayout.Width (120));
						GUILayout.Label("Sound", GUILayout.Width(labelWidthCol1));
						watcher.AlarmActive = EditorGUILayout.Toggle(watcher.AlarmActive);
						GUILayout.Space(spaceAfterCol1);
						GUILayout.Label("Threshold");
						watcher.AlarmThreshType = EditorGUILayout.Popup(watcher.AlarmThreshType, thresholdType, GUILayout.Width(50)); 
						watcher.AlarmThreshold = EditorGUILayout.FloatField(1.8f);
					EditorGUILayout.EndHorizontal();

					//Vibra Zeile
					EditorGUILayout.BeginHorizontal(GUILayout.Width (120));
						GUILayout.Label("Vibra", GUILayout.Width(labelWidthCol1));
						watcher.VibraActive = EditorGUILayout.Toggle(watcher.VibraActive);
						GUILayout.Space(spaceAfterCol1);
						GUILayout.Label("Threshold");
						watcher.VibraThreshType = EditorGUILayout.Popup(watcher.VibraThreshType, thresholdType, GUILayout.Width(50)); 
						watcher.VibraThreshold = EditorGUILayout.FloatField(1.8f);
					EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();


			}
			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();

				// EditorGUILayout.EndHorizontal();				
		}
				
		//Save??
		// EditorGUILayout.BeginHorizontal(); 

		//  if (GUILayout.Button("Save Configuration")) {
		// 	 config.ImportConfig();

		//  }

		//  if (GUILayout.Button("Load Configuration")) {
		// 	 config.ExportConfig();

		//  }

		if (GUI.changed) {
			config.SavePrefs();
			// EditorUtility.SetDirty(config);
		}

	}

	// public void OnInspectorUpdate() {
    //     this.Repaint();
    // }

	
}
