using UnityEngine;
using UnityEditor;
// using System.Collections;
// using System.Linq;

[CustomEditor(typeof(WatchdogConfiguration))]
public class WatchdogConfigInspector : Editor {

	// int[] selected = new int[10];
	bool[] foldouts = new bool[10]; 

	string[] thresholdType = new string[]{">", "<"};
	string[] notes = new string[]{"A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#"};
	float[] notesFreq = new float[]{440, 466.2f, 493.9f, 523.3f, 554.4f, 587.3f, 622.3f, 659.3f, 698.5f, 740, 784, 830.6f};


	public override void OnInspectorGUI() {
		WatchdogConfiguration config = (WatchdogConfiguration)target;
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Profiler Properties", EditorStyles.boldLabel);
		EditorGUILayout.Separator();

		GUILayoutOption[] options;			
		int labelWidthCol1 = 40; 
		int spaceAfterCol1 = 0;
		int toggleSpace = 25; 


		int count = 0;
		foreach (PropertyWatcher watcher in config.Watchers) {

			// GUILayoutOption[] options = new GUILayoutOption[]{ GUILayout.MinWidth(100), GUILayout.};
			watcher.Index = EditorGUILayout.Popup("Slot "+watcher.Slot, watcher.Index, config.ProfilerPropertiesFormatted);
			if (watcher.Index > 0) {
				watcher.Property = config.ProfilerProperties[watcher.Index-1];
			}
			
			options = new GUILayoutOption[]{ GUILayout.ExpandWidth(false)};
			GUILayoutOption fixedWidth = GUILayout.Width(EditorGUIUtility.currentViewWidth);	

			// EditorGUILayout.BeginHorizontal();
			// GUILayout.Label();
			EditorGUILayout.LabelField("Current Value:", "65 ms");	
			// EditorGUILayout.EndHorizontal();			
		
			// EditorGUILayout.BeginHorizontal();		
			// GUILayout.Label();
			EditorGUILayout.LabelField("Max Value", "65 ms");
			// EditorGUILayout.EndHorizontal();			

			EditorGUI.indentLevel++;
			foldouts[count] = EditorGUILayout.Foldout(foldouts[count], "Alarm");
			if (foldouts[count++]) {

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(20);

				EditorGUILayout.BeginVertical();
					//Alarm Zeile
					EditorGUILayout.BeginHorizontal(GUILayout.Width (120));
						GUILayout.Label("Sound", GUILayout.Width(labelWidthCol1));
						watcher.SoundActive = EditorGUILayout.Toggle(watcher.SoundActive, GUILayout.Width(toggleSpace));
						GUILayout.Space(spaceAfterCol1);
						GUILayout.Label("Threshold");
						watcher.SoundThreshType = EditorGUILayout.Popup(watcher.SoundThreshType, thresholdType, GUILayout.Width(50)); 
						watcher.SoundThreshold = EditorGUILayout.FloatField(watcher.SoundThreshold);
						GUILayout.Label("Note");
						watcher.SoundNote = EditorGUILayout.Popup(watcher.SoundNote, notes, GUILayout.Width(50)); 
					EditorGUILayout.EndHorizontal();

					//Vibra Zeile
					EditorGUILayout.BeginHorizontal(GUILayout.Width (120));
						GUILayout.Label("Vibra", GUILayout.Width(labelWidthCol1));
						watcher.VibraActive = EditorGUILayout.Toggle(watcher.VibraActive, GUILayout.Width(toggleSpace));
						GUILayout.Space(spaceAfterCol1);
						GUILayout.Label("Threshold");
						watcher.VibraThreshType = EditorGUILayout.Popup(watcher.VibraThreshType, thresholdType, GUILayout.Width(50)); 
						watcher.VibraThreshold = EditorGUILayout.FloatField(watcher.VibraThreshold);
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
