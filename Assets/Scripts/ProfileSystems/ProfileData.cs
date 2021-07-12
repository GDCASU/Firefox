using FMOD;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class that holds data for individual profiles
/// </summary>
[System.Serializable]
public class ProfileData
{
    /**
     * This is the template format on how profile files are saved. Meant
     * to be used with string.Format() to fill in {0}
     */
    public readonly static string profileFilepathTemplate = "/profile_{0}.data";

    /**
     * index:
     * I gave profiles an index to give them numerical identifiers
     * as well as being able to use them within a sort of list
     * 
     * stageScores:
     * Key = an index representation of a level
     * Value = The score achieved
     * 
     * currentStage:
     * This helps keep tracks of game progess by knowing which stage they last beat
     */
    public int profileID;
    public string name;
    public List<CopilotData> CopilotList
    {
        get
        {
            return _copilotList;
        }
    }
    private List<CopilotData> _copilotList;
    public Dictionary<int, int> stageScores;
    public int currentStage;
    public int newGamePlusCount;

    /// <summary>
    /// Constructor to make a brand new profile
    /// </summary>
    public ProfileData(int id, string name)
    {
        this.profileID = id;
        this.name = name;
        this._copilotList = new List<CopilotData>();
        this.stageScores = new Dictionary<int, int>();
        this.currentStage = 0;
        this.newGamePlusCount = 0;
        this._copilotList.Add(new CopilotData(Copilots.Feebee, 0, 0, true));
        this._copilotList.Add(new CopilotData(Copilots.Frederick, 0, 0, true));
    }

    /// <summary>
    /// Constructor to make a new profile using more
    /// pre-defined variabes
    /// </summary>
    public ProfileData(int id, string name, List<CopilotData> copilotList, Dictionary<int, int> stageScores, int currentStage, int newGamePlusCount)
    {
        this.profileID = id;
        this.name = name;
        this._copilotList = copilotList;
        this.stageScores = stageScores;
        this.currentStage = currentStage;
        this.newGamePlusCount = newGamePlusCount;
    }

    /// <summary>
    /// Method to add a new copilot to the profile. This handles
    /// checking to make sure the pilot has not already been added
    /// </summary>
    /// <param name="newPilot">The pilot being added</param>
    /// <returns>True if added and false otherwise</returns>
    public bool AddCopilot(CopilotData newPilot)
    {
        foreach (CopilotData pilot in _copilotList)
            if (pilot.name.Equals(newPilot.name)) return false;

        _copilotList.Add(newPilot);
        return true;
    }

    /// <summary>
    /// Simple method to get a copilot for this profile 
    /// based on the pilots name
    /// </summary>
    /// <param name="pilotName">Name of the pilot to find</param>
    /// <returns>The found copilot object</returns>
    public CopilotData GetCopilot(string pilotName)
    {
        foreach(CopilotData pilot in _copilotList)
            if (pilot.name.Equals(pilotName)) return pilot;

        return null;
    }
}
