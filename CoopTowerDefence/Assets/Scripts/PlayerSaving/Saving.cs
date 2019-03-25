using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.UI;

public class Saving : MonoBehaviour
{
    public SaveData data;
    public string fileName;
    public GameObject namePanel;
    public InputField nameInput;
    public int currentlyPlayingWith;
    public ClassScriptableObjectBaseStats[] classes;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (data.playerName == "")
            namePanel.SetActive(true);
        else
            PhotonNetwork.playerName = data.playerName;
    }

    public void SetName()
    {
        if (nameInput.text != "")
        {
            namePanel.SetActive(false);
            data.playerName = nameInput.text;
            PhotonNetwork.playerName = nameInput.text;
        }
    }

    public void OnApplicationQuit()
    {
        SaveData();
    }

    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/" + fileName + ".xml"))
        {
            Debug.Log(Application.persistentDataPath + "/" + fileName + ".xml");
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            FileStream stream = new FileStream(Application.persistentDataPath + "/" + fileName + ".xml", FileMode.Open);
            data = (SaveData)serializer.Deserialize(stream);
            stream.Close();
        }
    }

    public void SaveData()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
        FileStream stream = new FileStream(Application.persistentDataPath + "/" + fileName + ".xml", FileMode.Create);
        serializer.Serialize(stream, data);
        stream.Close();
    }
}

[System.Serializable]
public class SaveData
{
    public string playerName;
    public List<ClassSaveData> classSaves;
}

[System.Serializable]
public class ClassSaveData
{
    public int classIndex;
    public int classLevel;
}