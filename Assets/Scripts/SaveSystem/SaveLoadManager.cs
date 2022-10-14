using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : Singleton<SaveLoadManager>
{

    public GameSave gameSave;
    public List<ISaveable> iSaveableObjectList;

    protected override void Awake()
    {
        base.Awake();

        iSaveableObjectList = new List<ISaveable>();
    }

    // called from load game button
    public void LoadDataFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/15Cigs.dat"))
        {
            gameSave = new GameSave();

            FileStream file = File.Open(Application.persistentDataPath + "/15Cigs.dat", FileMode.Open);

            // deserialize the binary file back into the GameSave object
            gameSave = (GameSave)bf.Deserialize(file);

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
            gameSave.gameObjectData.Add(iSaveableObject.ISaveableUniqueID, iSaveableObject.ISaveableSave());
        }

        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Open(Application.persistentDataPath + "/15Cigs.dat", FileMode.Create);

        bf.Serialize(file, gameSave);

        file.Close();

        UIManager.Instance.DisablePauseMenu();
    }

    public void StoreCurrentSceneData()
    {
        // loop through all ISaveable objects and trigger store scene data for each
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            iSaveableObject.ISaveableStoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void RestoreCurrentSceneData()
    {
        // loop through all ISaveable objects and trigger restore scene data for each
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            iSaveableObject.ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }
}
