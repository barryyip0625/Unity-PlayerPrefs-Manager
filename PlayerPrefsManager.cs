using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace BYUtils.EditorTools
{
    public class PlayerPrefsManager : EditorWindow
    {
        // Static regex pattern to match Unity's hash suffix
        private static readonly Regex HashRegex = new Regex("_h[a-zA-Z0-9]+$");
        
        [MenuItem("Tools/BY Utils/PlayerPrefsManager")]
        public static void ShowWindow()
        {
            GetWindow<PlayerPrefsManager>("PlayerPrefsManager");
        }
        
        private Vector2 scrollPosition;
        private string newKey = "";
        private string newValue = "";
        private int selectedType = 0;
        private string[] typeOptions = { "String", "Int", "Float" };
        private string searchQuery = "";
        private List<PlayerPrefData> playerPrefs = new List<PlayerPrefData>();
        private bool showRawKeys = false;
        private string editingKey = null;
        private string editingValue = "";
        private int editingTypeIndex = 0;
        
        // Action queue for deferred operations
        private List<Action> pendingActions = new List<Action>();
        
        private void OnEnable()
        {
            RefreshPlayerPrefs();
        }
        
        private void OnGUI()
        {
            // Search field
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(50));
            string newSearchQuery = GUILayout.TextField(searchQuery);
            if (newSearchQuery != searchQuery)
            {
                searchQuery = newSearchQuery;
                RefreshPlayerPrefs();
            }
            
            if (GUILayout.Button("Refresh", GUILayout.Width(100)))
            {
                RefreshPlayerPrefs();
            }
            
            if (GUILayout.Button("Delete All", GUILayout.Width(100)))
            {
                if (EditorUtility.DisplayDialog("Delete All PlayerPrefs", 
                    "Are you sure you want to delete all PlayerPrefs?", "Yes", "No"))
                {
                    pendingActions.Add(() => {
                        PlayerPrefs.DeleteAll();
                        PlayerPrefs.Save();
                        RefreshPlayerPrefs();
                    });
                }
            }
            GUILayout.EndHorizontal();
            
            // Display Options
            bool newShowRawKeys = EditorGUILayout.ToggleLeft("Show Raw Keys (with hash)", showRawKeys);
            if (newShowRawKeys != showRawKeys)
            {
                showRawKeys = newShowRawKeys;
                RefreshPlayerPrefs();
            }
            
            // Add new PlayerPref
            GUILayout.Space(10);
            GUILayout.Label("Add New PlayerPref", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Type:", GUILayout.Width(50));
            selectedType = EditorGUILayout.Popup(selectedType, typeOptions);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Key:", GUILayout.Width(50));
            newKey = GUILayout.TextField(newKey);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Value:", GUILayout.Width(50));
            newValue = GUILayout.TextField(newValue);
            GUILayout.EndHorizontal();
            
            if (GUILayout.Button("Add"))
            {
                if (!string.IsNullOrEmpty(newKey))
                {
                    string keyToAdd = newKey;
                    string valueToAdd = newValue;
                    int typeToAdd = selectedType;
                    
                    pendingActions.Add(() => {
                        switch (typeToAdd)
                        {
                            case 0: // String
                                PlayerPrefs.SetString(keyToAdd, valueToAdd);
                                break;
                            case 1: // Int
                                int intValue;
                                if (int.TryParse(valueToAdd, out intValue))
                                    PlayerPrefs.SetInt(keyToAdd, intValue);
                                break;
                            case 2: // Float
                                float floatValue;
                                if (float.TryParse(valueToAdd, out floatValue))
                                    PlayerPrefs.SetFloat(keyToAdd, floatValue);
                                break;
                        }
                        PlayerPrefs.Save();
                        RefreshPlayerPrefs();
                    });
                    
                    newKey = "";
                    newValue = "";
                }
            }
            
            // Display existing PlayerPrefs
            GUILayout.Space(10);
            GUILayout.Label("Existing PlayerPrefs", EditorStyles.boldLabel);
            
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            
            EditorGUILayout.BeginVertical();
            
            if (playerPrefs.Count == 0)
            {
                GUILayout.Label("No PlayerPrefs found");
            }
            else
            {
                // Header
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Key", EditorStyles.boldLabel, GUILayout.Width(Mathf.Max(180, Screen.width * 0.3f)));
                GUILayout.Label("Type", EditorStyles.boldLabel, GUILayout.Width(Mathf.Max(80, Screen.width * 0.15f)));
                GUILayout.Label("Value", EditorStyles.boldLabel, GUILayout.Width(Mathf.Max(180, Screen.width * 0.3f)));
                GUILayout.Label("Actions", EditorStyles.boldLabel, GUILayout.Width(Mathf.Max(140, Screen.width * 0.2f)));
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                
                // Create a copy of the list to prevent modification during iteration
                List<PlayerPrefData> prefsToDisplay = new List<PlayerPrefData>(playerPrefs);
                
                // PlayerPrefs items
                foreach (var pref in prefsToDisplay)
                {
                    EditorGUILayout.BeginHorizontal();
                    
                    // If this key is being edited
                    if (editingKey == pref.RawKey)
                    {
                        // Don't allow editing the key, only the value and type
                        GUILayout.Label(pref.CleanKey, GUILayout.Width(Mathf.Max(180, Screen.width * 0.3f)));
                        
                        // Type selection
                        editingTypeIndex = EditorGUILayout.Popup(editingTypeIndex, typeOptions, GUILayout.Width(Mathf.Max(80, Screen.width * 0.15f)));
                        
                        // Value field
                        editingValue = GUILayout.TextField(editingValue, GUILayout.Width(Mathf.Max(180, Screen.width * 0.3f)));
                        
                        // Save and Cancel buttons
                        GUILayout.BeginHorizontal(GUILayout.Width(Mathf.Max(140, Screen.width * 0.2f)));
                        if (GUILayout.Button("Save", GUILayout.Width(Mathf.Max(65, Screen.width * 0.075f))))
                        {
                            string keyToEdit = pref.RawKey;
                            string cleanKeyToEdit = pref.CleanKey;
                            string valueToEdit = editingValue;
                            int typeToEdit = editingTypeIndex;
                            
                            pendingActions.Add(() => {
                                SaveEditedValue(keyToEdit, cleanKeyToEdit, valueToEdit, typeToEdit);
                            });
                            
                            editingKey = null;
                        }
                        
                        if (GUILayout.Button("Cancel", GUILayout.Width(Mathf.Max(65, Screen.width * 0.075f))))
                        {
                            editingKey = null;
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        // Regular display
                        GUILayout.Label(pref.Key, GUILayout.Width(Mathf.Max(180, Screen.width * 0.3f)));
                        GUILayout.Label(pref.Type, GUILayout.Width(Mathf.Max(80, Screen.width * 0.15f)));
                        GUILayout.Label(pref.Value, GUILayout.Width(Mathf.Max(180, Screen.width * 0.3f)));
                        
                        GUILayout.BeginHorizontal(GUILayout.Width(Mathf.Max(140, Screen.width * 0.2f)));
                        
                        // Edit button with a blue tint
                        GUI.backgroundColor = new Color(0.3f, 0.6f, 1.0f);
                        if (GUILayout.Button("Edit", GUILayout.Width(Mathf.Max(65, Screen.width * 0.075f))))
                        {
                            editingKey = pref.RawKey;
                            editingValue = pref.Value;
                            
                            // Set the type index
                            switch (pref.Type)
                            {
                                case "String":
                                    editingTypeIndex = 0;
                                    break;
                                case "Int":
                                    editingTypeIndex = 1;
                                    break;
                                case "Float":
                                    editingTypeIndex = 2;
                                    break;
                            }
                        }
                        
                        // Delete button with a red tint
                        GUI.backgroundColor = new Color(1.0f, 0.3f, 0.3f);
                        if (GUILayout.Button("Delete", GUILayout.Width(Mathf.Max(65, Screen.width * 0.075f))))
                        {
                            string keyToDelete = pref.RawKey;
                            string displayKey = pref.CleanKey;
                            
                            if (EditorUtility.DisplayDialog("Delete PlayerPref", 
                                $"Are you sure you want to delete '{displayKey}'?", "Yes", "No"))
                            {
                                pendingActions.Add(() => {
                                    PlayerPrefs.DeleteKey(keyToDelete);
                                    PlayerPrefs.Save();
                                    RefreshPlayerPrefs();
                                });
                            }
                        }
                        GUI.backgroundColor = Color.white; // Reset color
                        
                        GUILayout.EndHorizontal();
                    }
                    
                    EditorGUILayout.EndHorizontal();
                    
                    // Add a bit of space between entries
                    GUILayout.Space(2);
                }
            }
            
            EditorGUILayout.EndVertical();
            
            GUILayout.EndScrollView();
            
            // Process pending actions at the end of OnGUI
            ProcessPendingActions();
        }
        
        private void ProcessPendingActions()
        {
            if (pendingActions.Count > 0)
            {
                // Execute the first action and remove it from the queue
                pendingActions[0].Invoke();
                pendingActions.RemoveAt(0);
                
                // Force a repaint to process remaining actions in the next frame
                Repaint();
            }
        }
        
        private void SaveEditedValue(string rawKey, string cleanKey, string newValue, int typeIndex)
        {
            try
            {
                // First, delete the old key to avoid duplicates
                PlayerPrefs.DeleteKey(rawKey);
                
                // Always use the clean key (without hash) for new entries
                string keyToUse = cleanKey;
                
                // Then set the new value
                bool setSuccessful = false;
                
                switch (typeIndex)
                {
                    case 0: // String
                        PlayerPrefs.SetString(keyToUse, newValue);
                        setSuccessful = true;
                        break;
                    case 1: // Int
                        int intValue;
                        if (int.TryParse(newValue, out intValue))
                        {
                            PlayerPrefs.SetInt(keyToUse, intValue);
                            setSuccessful = true;
                        }
                        else
                            EditorUtility.DisplayDialog("Invalid Value", "The value could not be converted to an integer.", "OK");
                        break;
                    case 2: // Float
                        float floatValue;
                        if (float.TryParse(newValue, out floatValue))
                        {
                            PlayerPrefs.SetFloat(keyToUse, floatValue);
                            setSuccessful = true;
                        }
                        else
                            EditorUtility.DisplayDialog("Invalid Value", "The value could not be converted to a float.", "OK");
                        break;
                }
                
                // Only save if the operation was successful
                if (setSuccessful)
                {
                    PlayerPrefs.Save();
                    RefreshPlayerPrefs();
                }
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to save value: {ex.Message}", "OK");
            }
        }
        
        private void RefreshPlayerPrefs()
        {
            playerPrefs.Clear();
            
            Dictionary<string, List<string>> keyGroups = new Dictionary<string, List<string>>();
            
            // This requires reflection to access PlayerPrefs registry keys
            // NOTE: This approach is platform-specific (Windows)
            #if UNITY_EDITOR_WIN
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Unity\\UnityEditor\\" + PlayerSettings.companyName + "\\" + PlayerSettings.productName);
            
            if (key != null)
            {
                string[] valueNames = key.GetValueNames();
                
                // First pass: group keys by their base name (without hash)
                foreach (string valueName in valueNames)
                {
                    if (string.IsNullOrEmpty(searchQuery) || valueName.ToLower().Contains(searchQuery.ToLower()))
                    {
                        string displayName = valueName;
                        
                        if (HashRegex.IsMatch(valueName))
                        {
                            displayName = HashRegex.Replace(valueName, "");
                        }
                        
                        if (!keyGroups.ContainsKey(displayName))
                        {
                            keyGroups[displayName] = new List<string>();
                        }
                        
                        keyGroups[displayName].Add(valueName);
                    }
                }
                
                // Second pass: process each group
                foreach (var group in keyGroups)
                {
                    string displayName = group.Key;
                    
                    // Use the first raw key in the group
                    string rawName = group.Value[0];
                    
                    try
                    {
                        object registryValue = key.GetValue(rawName);
                        string valueType = "Unknown";
                        string valueString = "";
                        
                        // Try to determine type and value
                        try
                        {
                            // Check for string first
                            string stringValue = PlayerPrefs.GetString(rawName, "__PlayerPrefsViewer_NotFound__");
                            if (stringValue != "__PlayerPrefsViewer_NotFound__")
                            {
                                valueType = "String";
                                valueString = stringValue;
                            }
                            else
                            {
                                // Try int
                                int defaultInt = int.MinValue;
                                int intValue = PlayerPrefs.GetInt(rawName, defaultInt);
                                if (intValue != defaultInt)
                                {
                                    valueType = "Int";
                                    valueString = intValue.ToString();
                                }
                                else
                                {
                                    // Must be float
                                    float floatValue = PlayerPrefs.GetFloat(rawName);
                                    valueType = "Float";
                                    valueString = floatValue.ToString();
                                }
                            }
                        }
                        catch
                        {
                            // Fallback to registry type detection
                            if (registryValue is int)
                            {
                                valueType = "Int";
                                valueString = PlayerPrefs.GetInt(rawName).ToString();
                            }
                            else
                            {
                                // Try string
                                try
                                {
                                    valueType = "String";
                                    valueString = PlayerPrefs.GetString(rawName);
                                }
                                catch
                                {
                                    // Must be float
                                    valueType = "Float";
                                    valueString = PlayerPrefs.GetFloat(rawName).ToString();
                                }
                            }
                        }
                        
                        // If showing raw keys, add each variant, otherwise just add the display name once
                        if (showRawKeys)
                        {
                            foreach (string variantName in group.Value)
                            {
                                playerPrefs.Add(new PlayerPrefData(variantName, valueType, valueString, variantName, displayName));
                            }
                        }
                        else
                        {
                            playerPrefs.Add(new PlayerPrefData(displayName, valueType, valueString, rawName, displayName));
                        }
                    }
                    catch (Exception)
                    {
                        // Skip this key if there's an error
                    }
                }
            }
            #else
            // Add cross-platform support for other platforms here
            Debug.LogWarning("PlayerPrefsManager currently only supports Windows. For other platforms, manual implementation is needed.");
            #endif
        }
    }
    
    public class PlayerPrefData
    {
        public string Key { get; private set; }
        public string Type { get; private set; }
        public string Value { get; private set; }
        public string RawKey { get; private set; } // Store the original key with hash
        public string CleanKey { get; private set; } // Always store the clean key without hash
        
        public PlayerPrefData(string key, string type, string value, string rawKey, string cleanKey)
        {
            Key = key;
            Type = type;
            Value = value;
            RawKey = rawKey;
            CleanKey = cleanKey;
        }
    }
}