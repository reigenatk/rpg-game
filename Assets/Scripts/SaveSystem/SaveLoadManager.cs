using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : Singleton<SaveLoadManager>
{

    public GameSave gameSave;
    public List<ISaveable> iSaveableObjectList;
    public string path;

    protected override void Awake()
    {
        base.Awake();

        iSaveableObjectList = new List<ISaveable>();
        path =  Application.persistentDataPath + "/avpedee.dat";
    }

    // called from load game button
    public void LoadDataFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter();
        // Debug.Log("Does save exist at " + path + " :" + File.Exists(path));
        if (File.Exists(path))
        {
            Debug.Log("Save exists at " + path);
            gameSave = new GameSave();

            FileStream file = File.Open(path, FileMode.Open);

            // deserialize the binary file back into the GameSave object
            gameSave = (GameSave)bf.Deserialize(file);


            Debug.Log("iSaveableObjectList size is " + iSaveableObjectList.Count);
            // loop through all ISaveable objects and call load on each one.
            for (int i = iSaveableObjectList.Count - 1; i > -1; i--)
            {
                if (gameSave.gameObjectData.ContainsKey(iSaveableObjectList[i].ISaveableUniqueID))
                {
                    iSaveableObjectList[i].ISaveableLoad(gameSave);
                }
                // else if iSaveableObject unique ID is not in the game object data then destroy object
                else
                {
                    Component component = (Component)iSaveableObjectList[i];
                    Destroy(component.gameObject);
                }
            }

            file.Close();
        }
        else
        {
            Debug.Log("Save does not exist at " + path);
        }

        UIManager.Instance.DisablePauseMenu();
    }

    // called from save game button
    public void SaveDataToFile()
    {
        gameSave = new GameSave();

        // loop through all ISaveable objects and generate save data
        // game saves consist of a ton of key value pairs for each object we wanna save
        // the key is generated via generateGUID on awake() for each of the saveable gameobjects, and the value is just a GameObjectSave object
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            Debug.Log("Isaveable object " + iSaveableObject.ToString() + " with GUID " + iSaveableObject.ISaveableUniqueID);
            if (!gameSave.gameObjectData.ContainsKey(iSaveableObject.ISaveableUniqueID))
            {
                Debug.Log("[SaveDataToFile] Inserting data with GUID " + iSaveableObject.ISaveableUniqueID);
                gameSave.gameObjectData.Add(iSaveableObject.ISaveableUniqueID, iSaveableObject.ISaveableSave());
            }
            
        }

        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Open(path, FileMode.Create);

        bf.Serialize(file, gameSave);

        file.Close();

        Debug.Log("Saved game to path " + path);

        UIManager.Instance.DisablePauseMenu();
    }

    public void StoreCurrentSceneData()
    {
        // loop through all ISaveable objects and trigger store scene data for each
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            Debug.Log("[StoreCurrentSceneData] " + iSaveableObject.ToString());
            iSaveableObject.ISaveableStoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void RestoreCurrentSceneData()
    {
        // loop through all ISaveable objects and trigger restore scene data for each
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            Debug.Log("[RestoreCurrentSceneData] " + iSaveableObject.ToString());
            iSaveableObject.ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }
}
