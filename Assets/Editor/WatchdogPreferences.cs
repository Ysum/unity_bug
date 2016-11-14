using UnityEngine;
using UnityEditor;


public class WatchdogPreferences : MonoBehaviour {
    private static bool prefsLoaded = false;

	public static bool enable = true;
	public static string host = "127.0.0.1";
	public static int port = 3333;
	public static float freq = 0.5f;

    [PreferenceItem("Watchdog")]
    public static void PreferencesGUI()
    {
		
        if (!prefsLoaded)
        {
			enable = EditorPrefs.GetBool("Watchdog_Enable");
			host = EditorPrefs.GetString("Watchdog_Host");
			port = EditorPrefs.GetInt("Watchdog_Port");
			freq = EditorPrefs.GetFloat("Watchdog_Freq");
            prefsLoaded = true;
        }

		enable = EditorGUILayout.Toggle("Enable", enable);
		host = EditorGUILayout.TextField("Host",host);
		port = EditorGUILayout.IntField("Port",port);		
		freq = EditorGUILayout.FloatField("Update Frequency",freq);

        // Save the preferences
        if (GUI.changed) {
			EditorPrefs.SetBool("Watchdog_Enable", enable);
			EditorPrefs.SetString("Watchdog_Host", host);
			EditorPrefs.SetInt("Watchdog_Port", port);
			EditorPrefs.SetFloat("Watchdog_Freq", freq);

			// enable/disable Watchdog
			if (!enable) {
				DestroyImmediate(GameObject.Find("Watchdog"));
			} else {
				if(!GameObject.Find("Watchdog")) {
					GameObject watchdog = new GameObject("Watchdog");
					watchdog.AddComponent<Watchdog>().Init();
				}
				
			}
		}
		
	}


}
