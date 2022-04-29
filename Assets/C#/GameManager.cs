using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {


    public static bool isComingBackFromGameScene;

    private void Awake()
    {
        LocalPlayer.LoadGame();
    }
    private void Start()
    {
        isComingBackFromGameScene = false;

        DontDestroyOnLoad(this);
        Debug.Log("max level " + LocalPlayer.MaxLevel);
        Debug.Log("Current level " + LocalPlayer.CurrentLevel);

        Scene scene = SceneManager.GetActiveScene();


    }

    private void OnApplicationQuit()
    {
        LocalPlayer.SaveGame();
    }

}
