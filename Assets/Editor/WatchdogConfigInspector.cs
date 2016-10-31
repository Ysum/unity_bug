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
		// config.InitializeConfiguration();
		
		// EditorGUILayout.Space();
		// EditorGUILayout.LabelField("Profiler Properties", EditorStyles.boldLabel);
		// EditorGUILayout.Separator();

		if (config.HasEntries()) {

			// EditorGUILayout.BeginHorizontal(); 

			int count = 0;
			foreach (PropertyWatcher watcher in config.watchers) {
				GUILayoutOption[] options = new GUILayoutOption[]{ GUILayout.MinWidth(100)};
				watcher.Index = EditorGUILayout.Popup("Slot "+watcher.Slot, watcher.Index, config.profilerPropertiesFormatted, options);
				if (watcher.Index > 0) {
					watcher.Property = config.profilerProperties[watcher.Index-1];
				}
				
				EditorGUI.indentLevel++;
				foldouts[count] = EditorGUILayout.Foldout(foldouts[count], "Alarm");
				if (foldouts[count++]) {
					// EditorGUI.indentLevel++;

					//Alarm Zeile
					EditorGUILayout.BeginHorizontal(); 
						EditorGUILayout.BeginHorizontal(options);
						EditorGUILayout.PrefixLabel("Sound");
						watcher.AlarmActive = EditorGUILayout.Toggle(watcher.AlarmActive);
						EditorGUILayout.PrefixLabel("Threshold");
						watcher.AlarmThreshType = EditorGUILayout.Popup(watcher.AlarmThreshType, thresholdType); 
						watcher.AlarmThreshold = EditorGUILayout.FloatField(1.8f);
						EditorGUILayout.EndHorizontal();

					EditorGUILayout.EndHorizontal();

					//Vibra Zeile
					EditorGUILayout.BeginHorizontal(); 
					
						EditorGUILayout.BeginHorizontal(options);
						EditorGUILayout.PrefixLabel("Vibra");
						watcher.VibraActive = EditorGUILayout.Toggle(watcher.VibraActive);
						EditorGUILayout.PrefixLabel("Threshold");
						watcher.VibraThreshType = EditorGUILayout.Popup(watcher.VibraThreshType, thresholdType); 
						watcher.VibraThreshold = EditorGUILayout.FloatField(1.8f);
						EditorGUILayout.EndHorizontal();

					EditorGUILayout.EndHorizontal();

					EditorGUI.indentLevel--;

				}
				EditorGUI.indentLevel--;

				EditorGUILayout.Separator();
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

			// EditorGUILayout.EndHorizontal();
		}

		if (GUI.changed) {
			EditorUtility.SetDirty(config);
		}

	}

	// public void OnInspectorUpdate() {
    //     this.Repaint();
    // }

	
}
