///
/// Created by Alex Fischer | July 2024
/// 

using System.Collections;
using System.Collections.Generic;
using Udar.SceneManager;
using UnityEngine;

[CreateAssetMenu(fileName = "MapInformation", menuName = "Map Information", order = 2)]
public class MapInformationSO : ScriptableObject
{
    [Header("Map Information")]
    [SerializeField] string mapName;
    [SerializeField] Sprite mapIcon;
    [SerializeField] SceneField sceneFile;
    [SerializeField] MapType type;
    [SerializeField] string audioKey;

    /// <summary>
    /// Returns the map name from the information
    /// </summary>
    /// <returns>The map's name as a string</returns>
    public string GetMapName()
    {
        return mapName;
    }

    /// <summary>
    /// Returns the map icon from the information
    /// </summary>
    /// <returns>The map's name as a string</returns>
    public Sprite GetMapIcon()
    {
        return mapIcon;
    }

    /// <summary>
    /// Returns the map scene file
    /// </summary>
    /// <returns>The map's name as a string</returns>
    public SceneField GetSceneFile()
    {
        return sceneFile;
    }

    /// <summary>
    /// Returns the map type.
    /// </summary>
    /// <returns></returns>
    public MapType GetMapType()
    {
        return type;
    }

    /// <summary>
    /// Returns the audio key.
    /// </summary>
    /// <returns></returns>
    public string GetAudioKey()
    {
        return audioKey;
    }
}
