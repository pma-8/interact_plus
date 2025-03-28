using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the data collection for the csv file.
/// </summary>
public class CsvWriter : MonoBehaviour
{

    //Spawn time of each target
    public static List<float> spawnTargetTimes = new List<float>();

    //Hit time of each target
    public static List<float> hitTargetTimes = new List<float>();

    //Distances between each target
    public static List<float> targetDistances = new List<float>();

    //Time of round start
    public static float startRoundTime;

    //Time of round finish
    public static float endRoundTime;

    //Current/Used approach
    public static float gestureMode;

    //Current round
    public static float round;

    //Current target round
    public static float scale;

    //Time of game start
    public static float startGameTime;

    //Time of game finish
    public static float endGameTime;

    //Amount of target misses
    public static float missCount;

    //Amount of target hits
    public static float hitCount;

    //Accuracy per round
    public static float accuracy;

    //Complete csv data entry
    public static string csvEntry;

    /// <summary>
    /// Write a data entry to the csv log file.
    /// </summary>
    /// <param name="pFilename">Name of the csv file</param>
    public static void WriteGameInfoToCsv(string pFilename)
    {
        if (csvEntry.Length < 0)
        {
            return;
        }

        try
        {
            //True - Add stuff at the end of file
            //False - Overwrite whole file
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(getPath() + pFilename, true))
            {
                file.WriteLine(csvEntry);
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Unsuccesful writing to csv file : ", ex);
        }
    }

    /// <summary>
    /// Retrives the relative path as device platform.
    /// </summary>
    /// <returns></returns>
    public static string getPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/";
#elif UNITY_ANDROID
        return Application.persistentDataPath + "/";
#else
        return Application.dataPath +"/";
#endif
    }
}
