using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using System;
using System.Globalization;
using System.Collections;
using System.Text.RegularExpressions;


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

	public int FramesPerSec { get; protected set; }

	private static WatchdogConfiguration config;
	private static GameObject watchdog;
	private static Watchdog instance = null;
	public static Watchdog Instance {
		get {
			if (GameObject.Find("Watchdog") == null) {
				watchdog = new GameObject("Watchdog");
				instance = watchdog.AddComponent<Watchdog>();
				config = watchdog.AddComponent<WatchdogConfiguration>();

			}
			return instance;
		}
	}

	private void Reset() {
		
	} 

	private void OnEnable() {
		host = EditorPrefs.GetString("Watchdog_Host");
		port = EditorPrefs.GetInt("Watchdog_Port");
		freq = EditorPrefs.GetFloat("Watchdog_Freq");
	}

	private void Start() {
		Init();
		StartCoroutine(Frame());
	}

	private IEnumerator Frame() {
		for(;;){

			// Capture frame-per-second
			int lastFrameCount = Time.frameCount;
			float lastTime = Time.realtimeSinceStartup;
			yield return new WaitForSeconds(freq);

			//send OSC packages for display watch data
			for (int i = 0; i < 8; i++) {
				int slot = i+1;
				// sendOSCData(slot,ProfilerProperties.GetPropertyString(WatchFigures[i]));

			}

		}
	}

	public void Init() {
		ProfilerDriver.ClearAllFrames ();
		Profiler.enabled = true;
		OSCHandler.Instance.Init (host, port);
		
	}

	public void sendOSCData(int slot,string attribute) {
		data = new float[buffersize];
		float maxValue = 0;

		int frameIndex = ProfilerDriver.GetPreviousFrameIndex(0);
		int guid = -1;

		try {
			guid = ProfilerDriver.GetStatisticsIdentifier(attribute);
		}
		catch (Exception e) {
			Debug.Log (e.ToString ());
	
		}

		if (guid >= 0) {
			string string_formatted = ProfilerDriver.GetFormattedStatisticsValue (frameIndex, guid);
			ProfilerDriver.GetStatisticsValues(guid, 0, 1, data, out maxValue);

			string extractedKeyFigure = Regex.Match(string_formatted, @"(\d*\.\d*)").Value;
			float keyFigureValue = float.Parse(extractedKeyFigure, CultureInfo.InvariantCulture);

			OSCHandler.Instance.SendMessageToClient ("UnityMonitor", "/UnityWatchdog/DisplaySlot"+slot+"/"+attribute, string_formatted);

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