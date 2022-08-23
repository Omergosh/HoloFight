using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterGameDataSet", menuName = "ScriptableObjects/CharacterGameDataSetScriptableObject", order = 1)]
public class CharacterGameDataSetScriptableObject : ScriptableObject
{
    // Filename for individual JSON files containing frame data for every animation
    // (may require separate ScriptableObjects to be made for individual characters, due to character-specific behavior/animations)
    public string idleFrameDataFilename;
    public string runFrameDataFilename;
    public string jumpFrameDataFilename;
    public string fallFrameDataFilename;
    public string attackA1FrameDataFilename;
    public string attackB1FrameDataFilename;
    public string airAttackA1FrameDataFilename;
    public string hitstunFrameDataFilename;

    public TextAsset idleFrameDataJSON;
    public TextAsset runFrameDataJSOn;
    public TextAsset jumpFrameDataJSON;
    public TextAsset fallFrameDataJSON;
    public TextAsset attackA1FrameDataJSON;
    public TextAsset attackB1FrameDataJSON;
    public TextAsset airAttackA1FrameDataJSON;
    public TextAsset hitstunFrameDataJSON;
}
