using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditorInternal;



[Serializable]
public class WatchdogConfiguration : MonoBehaviour 
{
	[SerializeField]
	public string[] profilerProperties;
	[SerializeField]
	public string[] profilerPropertiesFormatted;

	[SerializeField]
	public List<PropertyWatcher> watchers;
	
	// singleton 
 	[SerializeField]
	public static WatchdogConfiguration instance = null;
	public static WatchdogConfiguration Instance 
	{
		get 
		{
			if (instance == null) {
				instance = GameObject.Find("Watchdog").AddComponent<WatchdogConfiguration>();
				instance.FetchProfilerProperties();
				instance.InitializeConfiguration();
			}
			return instance;
		}
	}


	public void FetchProfilerProperties() 
	{
		// get all profiler properties
		string[] allProfilerProperties = ProfilerDriver.GetAllStatisticsProperties();
		//get profiler areas
		ProfilerArea[] areas = (ProfilerArea[])Enum.GetValues(typeof(ProfilerArea));

		profilerProperties = new string[allProfilerProperties.Length];  
		profilerPropertiesFormatted = new string[allProfilerProperties.Length+1];  
		
		int index = 0;
		foreach(ProfilerArea a in areas) {
			if (a != ProfilerArea.AreaCount) {
				string[] areaProperties = ProfilerDriver.GetGraphStatisticsPropertiesForArea(a);
				profilerPropertiesFormatted[0] = " - ";
				for (int i = 0; i < areaProperties.Length; i++) {
					profilerProperties[index++] = areaProperties[i];
					profilerPropertiesFormatted[index] = a.ToString()+"/"+areaProperties[i].Replace('/', '\\');
				}
			}	
		}
	}

	public void InitializeConfiguration() 
	{

		if (watchers == null) {
			watchers = new List<PropertyWatcher>();

			for (int i = 1; i <= 10; i++) {
				PropertyWatcher pw = new PropertyWatcher();
				pw.Slot = i;
				watchers.Add(pw);
			}

			// this.AddPropertyWatcher("VSync");
			// this.AddPropertyWatcher("Triangles");
			// this.AddPropertyWatcher("Mesh Memory");
			// this.AddPropertyWatcher("GC Allocated");
			// this.AddPropertyWatcher("Contacts");
		}



		
	}

	public bool AddPropertyWatcher(string property) 
	{
		if (watchers != null) {
			bool ok = IsValidProperty(property); 
			bool cont = !ConfigContainsWatcher(property);
			if (ok && cont) {
				PropertyWatcher pw = new PropertyWatcher(property);
				pw.Slot = watchers.Count+1;
				pw.Index = ProfilerPropertiesGetIndexOf(property)+1;
				this.watchers.Add(pw);
			} else {
				throw new System.ArgumentException("invalid Property added to configuration.");
			}
			return ok;
		} else {
			throw new System.InvalidOperationException("no config found.");
		}
	}

	public void ExportConfig() {
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/config.dat");

		formatter.Serialize(file, watchers);
		file.Close();
	}

	public void ImportConfig() {
		if (File.Exists(Application.persistentDataPath + "/config.dat")) {
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "config.dat", FileMode.Open);
			
			List<PropertyWatcher> loadedWatchers =  (List<PropertyWatcher>)formatter.Deserialize(file);
			file.Close();

			watchers = loadedWatchers;
		}	
	}


	public int ProfilerPropertiesGetIndexOf(string s) {
		for (int i = 0; i < profilerProperties.Length; i++) {
			if (s.Equals(profilerProperties[i])){
				return i;
			}
		}
		return 666;
	}

	public bool HasEntries() {
		return watchers != null && watchers.Count > 0;

	}

	bool IsValidProperty(string s) {
		bool result = false;
		foreach (string property in profilerProperties) {
			if(s.Equals(property)) {
				result = true;
			}
		}
		return result;
	}

	bool ConfigContainsWatcher(string s) {
		foreach (PropertyWatcher watcher in watchers) {
			if (s.Equals(watcher.Property)) {
				return true;
			}
		}
		return false;
	}
	
}

[Serializable]
public class PropertyWatcher {

	[SerializeField]
	private string property; 
	public string Property 
	{
		get 
		{ 
			property = EditorPrefs.GetString("Watchdog_Slot"+Slot+"_Property");
			return property;
		}
		set
		{
			property = value;
			EditorPrefs.SetString("Watchdog_Slot"+Slot+"_Property", property);
		} 
	}

	[SerializeField]
	private int index; 
	public int Index 
	{ 
		get 
		{
			index = EditorPrefs.GetInt("Watchdog_Slot"+Slot+"_Index");
			return index;
		} 
		
		set 
		{
			index = value;
			EditorPrefs.SetInt("Watchdog_Slot"+Slot+"_Index", index);

		}
	}

	[SerializeField]
	public int Slot { get; set; }
	
	private bool alarmActive;
	public bool AlarmActive 
	{ 
		get 
		{
			return alarmActive;
		} 
		set
		{
			alarmActive = value;
			EditorPrefs.SetBool("Watchdog_Slot"+Slot+"_AlarmActive", alarmActive);		
			} 
		}

	private float alarmFreq;
	public float AlarmFreq 
	{ 
		get 
		{
			return alarmFreq;
		} 
		set
		{
			alarmFreq = value;
			EditorPrefs.SetFloat("Watchdog_Slot"+Slot+"_AlarmFreq", alarmFreq);		
		} 
	}

	private float alarmThreshold;
	public float AlarmThreshold 
	{ 
		get 
		{
			return alarmThreshold;
		} 
		set
		{
			alarmThreshold = value;
			EditorPrefs.SetFloat("Watchdog_Slot"+Slot+"_AlarmThreshold", alarmThreshold);		
		} 
	}
	
	private int alarmThreshType;
	public int AlarmThreshType 
	{ 
		get 
		{
			return alarmThreshType;
		} 
		set
		{
			alarmThreshType = value;
			EditorPrefs.SetInt("Watchdog_Slot"+Slot+"_AlarmThreshType", alarmThreshType);		
		} 
	}
    
	private bool vibraActive;
	public bool VibraActive 
	{ 
		get 
		{
			return vibraActive;
		} 
		set
		{
			vibraActive = value;
			EditorPrefs.SetBool("Watchdog_Slot"+Slot+"_VibraActive", vibraActive);
		} 
	}

	private float vibraThreshold;
	public float VibraThreshold 
	{ 
		get 
		{
			return vibraThreshold;
		} 
		set
		{
			vibraThreshold = value;
			EditorPrefs.SetFloat("Watchdog_Slot"+Slot+"_VibraThreshold", vibraThreshold);		
		} 
	}

	private int vibraThreshType;
	public int VibraThreshType 
	{ 
		get 
		{
			return vibraThreshType;
		} 
		set
		{
			vibraThreshType = value;
			EditorPrefs.SetInt("Watchdog_Slot"+Slot+"_VibraThreshType", vibraThreshType);		
		} 
	}


	// public PropertyWatcher(string s) {
	// 	Property = s;
		
	// 	AlarmActive = false;
	// 	AlarmFreq = 0;
	// 	AlarmThreshold = 0;
	// 	AlarmThreshType = 0; // 0: alarm above or 1: below threshold
		
	// 	VibraActive = false;
	// 	VibraThreshold = 0;
	// 	VibraThreshType = 0; // 0: alarm above or 1: below threshold

	// }

		//constructor

		public PropertyWatcher() {
			AlarmActive = false;
			AlarmFreq = 0;
			AlarmThreshold = 0;
			AlarmThreshType = 0; // 0: alarm above or 1: below threshold
			
			VibraActive = false;
			VibraThreshold = 0;
			VibraThreshType = 0; // 0: alarm above or 1: below threshold
		}

		public PropertyWatcher(string s) : this() {
			Property = s;

		}

}
