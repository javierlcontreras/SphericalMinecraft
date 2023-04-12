using System.IO;
using System;
using UnityEngine;

public class FileManager {
    string folderPath;
    public FileManager(string worldName, string systemName) {
        string worldPath = Path.Combine(SaveSystemManager.DATA_FOLDER_PATH, worldName);
        folderPath = Path.Combine(worldPath, systemName);
        if (!Directory.Exists(worldPath)) {
            Directory.CreateDirectory(worldPath);
        }
        if (!Directory.Exists(folderPath)) {
            Directory.CreateDirectory(folderPath);
        }
    }

    public void WriteFile(string data, string fileName) {
        string filePath = Path.Combine(folderPath, fileName);
        File.WriteAllText(filePath, data);
    }
    public string ReadFile(string fileName) {
        string filePath = Path.Combine(folderPath, fileName);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Trying to read " + filePath + " doesn't exist.");
            return null;
        }
        return File.ReadAllText(filePath);
    }
}