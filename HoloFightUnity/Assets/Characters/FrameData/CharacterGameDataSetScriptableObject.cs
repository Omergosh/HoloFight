using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterGameDataSet", menuName = "ScriptableObjects/CharacterGameDataSetScriptableObject", order = 1)]
public class CharacterGameDataSetScriptableObject : ScriptableObject
{
    // Filename for individual JSON files containing frame data for every animation
    // (may require separate ScriptableObjects to be made for individual characters, due to character-specific behavior/animations)
    public string idleFrameData;
    public string runFrameData;
    public string jumpFrameData;
    public string fallFrameData;
    public string attackB1FrameData;
}
