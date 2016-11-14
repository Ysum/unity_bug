using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using System;
using System.Globalization;
using System.Collections;
using System.Text.RegularExpressions;


[InitializeOnLoad]
public class Startup {
	static Startup() {
		if (EditorPrefs.GetBool("Watchdog_Enable")) {
			if (GameObject.Find("Watchdog") == null) {
				GameObject watchdog = new GameObject("Watchdog");
				watchdog.AddComponent<Watchdog>().Init();
			}		
		}
	}
}

[Serializable]
public class Watchdog: MonoBehaviour {

	/* Settings */
	string host;
	int port;
	float freq;

	// Key buffer
	int buffersize = 100;
	float[] data;
	float maxValue;
	int counter;

	string[] statisticProperties;
	string[] propertiesToTransmit;

	[SerializeField]
	private WatchdogConfiguration config;
	// private static PropertyWatcherList config;

	
	[SerializeField]
	public void Init() {
		config = WatchdogConfiguration.Instance;
		Debug.Log(config.Watchers);
		config.LoadPrefs();

	}

	private void Reset() {
		// config.LoadPrefs();
		// config = WatchdogConfiguration.Instance;
		// Debug.Log(config);

	}

	private void OnEnable() {

		host = EditorPrefs.GetString("Watchdog_Host");
		port = EditorPrefs.GetInt("Watchdog_Port");
		freq = EditorPrefs.GetFloat("Watchdog_Freq");
	}

	private void Start() {
		PrepareTransmission();
		StartCoroutine(Frame());

	}



	private IEnumerator Frame() {
		for(;;){
			// Capture frame-per-second
			int lastFrameCount = Time.frameCount;
			float lastTime = Time.realtimeSinceStartup;
			yield return new WaitForSeconds(freq);

			//send OSC packages for display watch data
			foreach (PropertyWatcher watcher in config.Watchers) {
				if (watcher.Index > 0) {
					SendOSCData(watcher);	
				}
			}
		}
	}

	private void PrepareTransmission() {
		ProfilerDriver.ClearAllFrames ();
		Profiler.enabled = true;
		OSCHandler.Instance.Init (host, port);
		
	}

	private void SendOSCData(PropertyWatcher watcher) {
		int i = ProfilerDriver.GetPreviousFrameIndex(0);
		string s = ProfilerDriver.GetOverviewText(ProfilerArea.CPU, i);

		data = new float[buffersize];
		float maxValue = 0;

		int frameIndex = ProfilerDriver.GetPreviousFrameIndex(0);
		int guid = -1;

		//get property identifier
		try {
			guid = ProfilerDriver.GetStatisticsIdentifier(watcher.Property);
		}
		catch (Exception e) {
			Debug.Log (e.ToString ());
	
		}

		if (guid >= 0) {
			string string_formatted = ProfilerDriver.GetFormattedStatisticsValue (frameIndex, guid);
			ProfilerDriver.GetStatisticsValues(guid, 0, 1, data, out maxValue);

			string extractedKeyFigure = Regex.Match(string_formatted, @"(\d+\.\d*|\d+)").Value;
			float keyFigureValue = float.Parse(extractedKeyFigure, CultureInfo.InvariantCulture);

			//send display message
			OSCHandler.Instance.SendMessageToClient ("Watchdog", "/UnityWatchdog/Slot"+watcher.Index+"/Display/"+watcher.Property, string_formatted);

			//send sound frequency if set
			if (watcher.SoundNote > 0) {
				OSCHandler.Instance.SendMessageToClient ("Watchdog", "/UnityWatchdog/Slot"+watcher.Index+"/Sound/Freq", watcher.SoundNote);
			}

			//sound alarm active
			if (watcher.SoundActive) {
				if (watcher.SoundThreshType == 0) {

				} else if (watcher.SoundThreshType == 1) {

				}

			}

			//vibra alarm active
			if (watcher.VibraActive) {
				if (watcher.VibraThreshType == 0 && keyFigureValue > watcher.VibraThreshold || watcher.VibraThreshType == 1 && keyFigureValue < watcher.VibraThreshold) {
					Debug.Log("OSC message sent: vibra");
					OSCHandler.Instance.SendMessageToClient ("Watchdog", "/UnityWatchdog/Slot"+watcher.Index+"/Vibra", "");
				}
			}

			// //send alarm message if above threshold (if set)
			// if (SoundWarningThresholds[slot-1] > 0 && keyFigureValue >= SoundWarningThresholds[slot-1]) {
			// 	float delta = keyFigureValue - SoundWarningThresholds[slot-1];
			// 	OSCHandler.Instance.SendMessageToClient ("UnityMonitor", "/UnityWatchdog/Alarm"+slot+"/Delta/", delta);
			// }

			// //send vibra message if above threshold (if set)
			// if (VibraWarningThresholds[slot-1] > 0 && keyFigureValue >= VibraWarningThresholds[slot-1]) {
			// 	float delta = keyFigureValue - VibraWarningThresholds[slot-1];
			// 	OSCHandler.Instance.SendMessageToClient ("UnityMonitor", "/UnityWatchdog/Vibra"+slot, true);
			// }

			
		}
	}
}