using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {
        FindObjectOfType<LevelLoader>().FadeAndLoadScene(SceneName.ActualDarkScene, LevelLoader.Instance.defaultSceneLocation, 2.0f);
    }

    public void LoadGame()
    {
        FindObjectOfType<SaveLoadManager>().LoadDataFromFile();
    }
}
