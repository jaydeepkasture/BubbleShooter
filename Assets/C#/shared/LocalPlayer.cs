using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer : MonoBehaviour
{
    private static int _currentLevel, _clearedLevel,_maxLevel;
    private static bool _isSoundEnabled;

    public static void LoadGame()
    {
        _currentLevel = PlayerPrefs.GetInt("_currentLevel", 1);
        _isSoundEnabled = bool.Parse(PlayerPrefs.GetString("_isSoundEnabled", "true"));
        _maxLevel = PlayerPrefs.GetInt("_maxLevel", 1);
    }
    public static void SaveGame()
    {

        PlayerPrefs.SetInt("_currentLevel", _currentLevel);
        PlayerPrefs.SetInt("_maxLevel", _maxLevel);
        PlayerPrefs.SetString("_isSoundEnabled", _isSoundEnabled.ToString());
        PlayerPrefs.Save();
    }

    public static int CurrentLevel
    {
        get { return _currentLevel; }
        set {  _currentLevel = value; }
    }
    public static int MaxLevel
    {
        get { return _maxLevel; }
        set
        {
            _maxLevel = value;
        }
    }

 
    public static bool isSoundEnabled
    {
        get
        {
            return _isSoundEnabled;
        }
        set
        {
            _isSoundEnabled = value;
        }


    }
}
