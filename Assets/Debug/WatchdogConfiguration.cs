using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEditorInternal;
// using System.Runtime.Serialization.Formatters.Binary;
// using System.IO;


[Serializable]
public class WatchdogConfiguration : MonoBehaviour 
{
	[SerializeField]
	private string[] profilerProperties;
	public string[] ProfilerProperties 
	{
		get 
		{
			return profilerProperties;
		}
	}
	
	[SerializeField]
	private string[] profilerPropertiesFormatted;
	public string[] ProfilerPropertiesFormatted 
	{
		get 
		{
			return profilerPropertiesFormatted;
		}
	}

	// private PropertyWatcherList config;
	// public PropertyWatcherList Config
	// {
	// 	get 
	// 	{
	// 		LoadPrefs();
	// 		return config;
	// 	}
	// }
	
	[SerializeField]
	private PropertyWatcher[] watchers;	
	public PropertyWatcher[] Watchers 
	{
		get 
		{
			return watchers;
		}
	}


	[SerializeField]
	private static WatchdogConfiguration instance = null;
	public static WatchdogConfiguration Instance 
	{
		get 
		{	
			GameObject watchdog = GameObject.Find("Watchdog");
			instance = (WatchdogConfiguration) watchdog.GetComponent(typeof(WatchdogConfiguration));
			// instance = (WatchdogConfiguration)FindObjectOfType(typeof(WatchdogConfiguration));


			//create if doesn't exist
			if (instance == null) {
				instance = watchdog.AddComponent<WatchdogConfiguration>();
				// instance = ScriptableObject.CreateInstance<WatchdogConfiguration>();
				instance.FetchProfilerProperties();
				instance.InitializeConfiguration();
			}

			instance.LoadPrefs();
			
			return instance;
		}
	}

	// void Reset() {
	// 	if (config == null) {
	// 		config = ScriptableObject.CreateInstance<PropertyWatcherList>();
	// 	}
	// }

	public void clearPrefs() 
	{
		instance.watchers = null;
		instance.InitializeConfiguration();
	}

	public void SavePrefs() 
	{
		foreach (PropertyWatcher w in watchers) {
			// if (w.Index > 0) {
				EditorPrefs.SetString("Watchdog_Slot"+w.Slot+"_Property", w.Property);
				EditorPrefs.SetInt("Watchdog_Slot"+w.Slot+"_Index", w.Index);
				EditorPrefs.SetBool("Watchdog_Slot"+w.Slot+"_SoundActive", w.SoundActive);		
				// EditorPrefs.SetFloat("Watchdog_Slot"+w.Slot+"_AlarmFreq", w.SoundFreq);		
				EditorPrefs.SetFloat("Watchdog_Slot"+w.Slot+"_SoundThreshold", w.SoundThreshold);		
				EditorPrefs.SetInt("Watchdog_Slot"+w.Slot+"_SoundThreshType", w.SoundThreshType);		
				EditorPrefs.SetBool("Watchdog_Slot"+w.Slot+"_VibraActive", w.VibraActive);
				EditorPrefs.SetFloat("Watchdog_Slot"+w.Slot+"_VibraThreshold", w.VibraThreshold);		
				EditorPrefs.SetInt("Watchdog_Slot"+w.Slot+"_VibraThreshType", w.VibraThreshType);	
			// }	
		} 
	}
	
	[SerializeField]
	public void LoadPrefs() {
		foreach (PropertyWatcher w in watchers) {
			//  if (w.Index > 0) {
				w.Property = EditorPrefs.GetString("Watchdog_Slot"+w.Slot+"_Property");
				w.Index = EditorPrefs.GetInt("Watchdog_Slot"+w.Slot+"_Index");
				w.SoundActive = EditorPrefs.GetBool("Watchdog_Slot"+w.Slot+"_SoundActive");
				// w.AlarmFreq = EditorPrefs.GetFloat("Watchdog_Slot"+w.Slot+"_SoundFreq");
				w.SoundThreshold = EditorPrefs.GetFloat("Watchdog_Slot"+w.Slot+"_SoundThreshold");
				w.SoundThreshType = EditorPrefs.GetInt("Watchdog_Slot"+w.Slot+"_SoundThreshType");
				w.VibraActive = EditorPrefs.GetBool("Watchdog_Slot"+w.Slot+"_VibraActive");         
				w.VibraThreshold = EditorPrefs.GetFloat("Watchdog_Slot"+w.Slot+"_VibraThreshold");
				w.VibraThreshType = EditorPrefs.GetInt("Watchdog_Slot"+w.Slot+"_VibraThreshType");  		
			//  }
			
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

		if (instance.watchers == null) {
			instance.watchers = new PropertyWatcher[10];

			for (int i = 1; i <= 10; i++) {
				PropertyWatcher pw = new PropertyWatcher(i);
				instance.watchers[i-1] = pw;
			}

		}
	}

	// private bool AddPropertyWatcher(string property) 
	// {
	// 	if (instance.watchers != null) {
	// 		bool ok = IsValidProperty(property); 
	// 		bool cont = !ConfigContainsWatcher(property);
	// 		if (ok && cont) {
	// 			int newSlot = instance.watchers.Count+1;
	// 			PropertyWatcher pw = new PropertyWatcher(newSlot, property);
	// 			pw.Index = ProfilerPropertiesGetIndexOf(property)+1;
	// 			instance.watchers.Add(pw);
	// 		} else {
	// 			throw new System.ArgumentException("invalid Property added to configuration.");
	// 		}
	// 		return ok;
	// 	} else {
	// 		throw new System.InvalidOperationException("no config found.");
	// 	}
	// }

	// public void ExportConfig() 
	// {
	// 	BinaryFormatter formatter = new BinaryFormatter();
	// 	FileStream file = File.Create(Application.persistentDataPath + "/config.dat");

	// 	formatter.Serialize(file, watchers);
	// 	file.Close();
	// }

	// public void ImportConfig() 
	// {
	// 	if (File.Exists(Application.persistentDataPath + "/config.dat")) {
	// 		BinaryFormatter formatter = new BinaryFormatter();
	// 		FileStream file = File.Open(Application.persistentDataPath + "config.dat", FileMode.Open);
			
	// 		List<PropertyWatcher> loadedWatchers =  (List<PropertyWatcher>)formatter.Deserialize(file);
	// 		file.Close();

	// 		watchers = loadedWatchers;
	// 	}	
	// }


	// private int ProfilerPropertiesGetIndexOf(string property) 
	// {
	// 	for (int i = 0; i < profilerProperties.Length; i++) {
	// 		if (property.Equals(profilerProperties[i])){
	// 			return i;
	// 		}
	// 	}
	// 	throw new ArgumentException("Invalid Property");
	// }

	// public bool HasEntries() 
	// {
	// 	return instance.watchers != null && watchers.Length > 0;

	// }

	// bool IsValidProperty(string s) 
	// {
	// 	bool result = false;
	// 	foreach (string property in profilerProperties) {
	// 		if(s.Equals(property)) {
	// 			result = true;
	// 		}
	// 	}
	// 	return result;
	// }

	// bool ConfigContainsWatcher(string s) 
	// {
	// 	foreach (PropertyWatcher watcher in instance.watchers) {
	// 		if (s.Equals(watcher.Property)) {
	// 			return true;
	// 		}
	// 	}
	// 	return false;
	// }
	
}

// [Serializable]
// public class PropertyWatcherList : ScriptableObject {

// 	[SerializeField]
// 	private List<PropertyWatcher> watchers;	
// 	public List<PropertyWatcher> Watchers	

// 	{
// 		get 
// 		{ 
// 			return watchers;
// 		} 

// 		set 
// 		{ 
// 			watchers = value;
// 		} 
// 	}

// }

[Serializable]
public class PropertyWatcher {

	[SerializeField]
	private string property; 
	public string Property 
	{
		get 
		{ 
			return property;
		}
		set
		{
			property = value;
		} 
	}

	[SerializeField]
	private int index; 
	public int Index 
	{ 
		get 
		{
			return index;
		} 
		
		set 
		{
			index = value;

		}
	}

	[SerializeField]
	private int slot; 
	public int Slot 
	{ 
		get 
		{
			return slot;
		} 
		
		set 
		{
			slot = value;

		}
	}

	[SerializeField]	
	private bool soundActive;
	public bool SoundActive 
	{ 
		get 
		{
			return soundActive;
		} 
		set
		{
			soundActive = value;
			} 
		}

	[SerializeField]
	private int soundNote;
	public int SoundNote 
	{ 
		get 
		{
			return soundNote;
		} 
		set
		{
			soundNote = value;
		} 
	}

	[SerializeField]
	private float soundThreshold;
	public float SoundThreshold 
	{ 
		get 
		{
			return soundThreshold;
		} 
		set
		{
			soundThreshold = value;
		} 
	}
	
	[SerializeField]
	private int soundThreshType;
	public int SoundThreshType 
	{ 
		get 
		{
			return soundThreshType;
		} 
		set
		{
			soundThreshType = value;
		} 
	}
    
	[SerializeField]
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
		} 
	}

	[SerializeField]
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
		} 
	}

	[SerializeField]
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
		} 
	}

	// default constructor
	public PropertyWatcher(int slot) {
		Slot = slot;
		SoundActive = false;
		// SoundFreq = 0;
		SoundThreshold = 0;
		SoundThreshType = 0; // 0: alarm above or 1: below threshold
		
		VibraActive = false;
		VibraThreshold = 0;
		VibraThreshType = 0; // 0: alarm above or 1: below threshold
	}
	
	// string property constructor
	public PropertyWatcher(int slot, string property) : this(slot) {
		Property = Property;

	}

}
