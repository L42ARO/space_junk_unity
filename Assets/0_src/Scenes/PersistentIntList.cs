using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class PersistentIntList : MonoBehaviour
{
    private string filePath;
    private List<int> intList = new List<int>();

    private void Start()
    {
        filePath = Application.persistentDataPath + "/intList.json";
        LoadIntList();
    }

    private void Update()
    {
        // Example: Append a new integer to the list every second
        if (Time.frameCount % 60 == 0)
        {
            int newInt = intList.Count + 1; // Example of adding incremental values
            intList.Add(newInt);
            Debug.Log("Added: " + newInt);
            SaveIntList();
        }
    }

    private void LoadIntList()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            intList = JsonUtility.FromJson<IntListWrapper>(json).intList;
            Debug.Log("Loaded list from file.");
        }
        else
        {
            Debug.Log("File not found, creating new list.");
            SaveIntList(); // Save the initial empty list
        }
    }

    private void SaveIntList()
    {
        IntListWrapper wrapper = new IntListWrapper { intList = intList };
        string json = JsonUtility.ToJson(wrapper);
        File.WriteAllText(filePath, json);
        Debug.Log("List saved to file.");
    }

    [System.Serializable]
    private class IntListWrapper
    {
        public List<int> intList;
    }
}
