using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FrameDataEditorScript : MonoBehaviour
{
    // Editor information (e.g. current animation)
    public float pixelsInUnityGridUnit;

    bool isPlaying = false;
    float isPlayingAnimationTimer = 0f;
    float isPlayingAnimationInterval = 1/60f;

    // Events / UI stuff
    public FrameEditorUIScript frameEditorUIValues;
    //public UnityEvent OnFrameAdvanceByOne;

    // Information about the current animation/state being displayed
    [SerializeField]
    public AnimationStateData animationFrameData = new AnimationStateData();
    //public int activeFrame = 0;

    //public Transform hitboxRepresentation1;
    public Transform[] hitboxRepresentations = new Transform[10];

    public VisualRepPlayer visualRep;

    // Start is called before the first frame update
    void Start()
    {
        pixelsInUnityGridUnit = BattleManagerScript.pixelsInWorldUnit;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            isPlayingAnimationTimer += Time.deltaTime;

            if(isPlayingAnimationTimer >= isPlayingAnimationInterval)
            {
                AdvanceFrame();
                isPlayingAnimationTimer = 0f;
            }

            //Debug.Log(isPlayingAnimationTimer);
            //Debug.Log(isPlayingAnimationInterval);
        }

        UpdateVisuals();
    }

    // Saving and loading frame data
    void SaveCurrentActiveAnimationData(string characterName = "INA", string animationName = "testFrameData")
    {
        animationFrameData.animationName = animationName;

        // Save current values (for animation currently being viewed) as a json file
        // in the frame data / game data directory
        // (Assets -> Characters -> FrameData -> INA)
        var serializerSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        string str = JsonConvert.SerializeObject(animationFrameData, serializerSettings);
        Debug.Log(str);

        //using (FileStream fs = new FileStream("Assets/Characters/FrameData/FBK/testFrameData.json", FileMode.OpenOrCreate))

        using (FileStream fs = new FileStream($"Assets/Characters/FrameData/{characterName}/{animationName}.json", FileMode.OpenOrCreate))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(str);
                //writer.Close();
                writer.Dispose();
            }
            //fs.Close();
            fs.Dispose();
        }

        Debug.Log($"Saved current animation's frame data! Character: {characterName} Animation: {animationName}");
    }

    // Load Animation Frame Data From File
    void LoadCurrentActiveAnimationData(string characterName, string animationName)
    {
        // Load current values (for animation currently being viewed) from a json file
        // in the frame data / game data directory
        // (Assets -> Characters -> FrameData -> INA)
        string str;

        //using (FileStream fs = new FileStream("Assets/Characters/FrameData/FBK/testFrameData.json", FileMode.Open))

        using (FileStream fs = new FileStream($"Assets/Characters/FrameData/{characterName}/{animationName}.json", FileMode.Open))
        {
            using (StreamReader reader = new StreamReader(fs))
            {
                str = reader.ReadLine();
                //reader.Close();
                reader.Dispose();
            }
            //fs.Close();
            fs.Dispose();
        }

        var serializerSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        var loadedFrameData = JsonConvert.DeserializeObject<AnimationStateData>(str, serializerSettings);
        Debug.Log(loadedFrameData);
        animationFrameData = loadedFrameData;

        Debug.Log($"Loaded current animation's frame data! Character: {characterName} Animation: {animationName}");
    }

    // Visuals / UI Updates
    void UpdateVisuals()
    {
        FrameData currentFrame = animationFrameData.CurrentFrame;

        for (int i = 0; i < hitboxRepresentations.Length; i++)
        {
            //hitboxRepresentations[i].gameObject.SetActive(currentFrame.active);
            if (i < currentFrame.hitboxes.Count)
            {
                hitboxRepresentations[i].gameObject.SetActive(true);

                hitboxRepresentations[i].localScale = new Vector3(
                    ConvertFrameDataMeasureToUnityUnits(currentFrame.hitboxes[i].hitboxRect.width),
                    ConvertFrameDataMeasureToUnityUnits(currentFrame.hitboxes[i].hitboxRect.height),
                    hitboxRepresentations[i].localScale.z
                );
                hitboxRepresentations[i].localPosition = new Vector3(
                    ConvertFrameDataMeasureToUnityUnits(currentFrame.hitboxes[i].hitboxRect.x + currentFrame.hitboxes[i].hitboxRect.width / 2),
                    ConvertFrameDataMeasureToUnityUnits(currentFrame.hitboxes[i].hitboxRect.y + currentFrame.hitboxes[i].hitboxRect.height / 2),
                    hitboxRepresentations[i].position.z
                );
            }
            else
            {
                hitboxRepresentations[i].gameObject.SetActive(false);
            }
        }
    }

    float ConvertFrameDataMeasureToUnityUnits(float frameDataSizeValue)
    {
        return frameDataSizeValue / pixelsInUnityGridUnit;
    }

    // Input Events

    public void OnAdvanceFrame()
    {
        AdvanceFrame();
        frameEditorUIValues.UpdateUIForFrame();
    }

    public void OnRewindFrame()
    {
        RewindFrame();
        frameEditorUIValues.UpdateUIForFrame();
    }

    public void OnResetToFrameZero()
    {
        GoToFrame(0);
        frameEditorUIValues.UpdateUIForFrame();
    }

    public void OnSaveAnimationData()
    {
        // Retrieve input values from UI text fields
        string charName = frameEditorUIValues.currentCharacterInputField.text;
        string animName = frameEditorUIValues.currentAnimationInputField.text;

        // Save using input values provided
        SaveCurrentActiveAnimationData(charName, animName);
    }
    public void OnLoadAnimationData()
    {
        // Retrieve input values from UI text fields
        string charName = frameEditorUIValues.currentCharacterInputField.text;
        string animName = frameEditorUIValues.currentAnimationInputField.text;

        // Load using input values provided
        LoadCurrentActiveAnimationData(charName, animName);

        // Update visual representation
        visualRep.UpdateAnimationClip(charName, animName);
        visualRep.totalFrames = animationFrameData.frames.Count;

        // Refresh/update UI values to reflect newly loaded data
        OnResetToFrameZero();
        frameEditorUIValues.UpdateUIForAnimationLoad();
    }

    public void OnPlayPauseAnimation()
    {
        if (isPlaying)
        {
            isPlaying = false;
            isPlayingAnimationTimer = 0f;
        }
        else
        {
            if(animationFrameData.frames.Count > 0)
            {
                isPlaying = true;
            }
        }
    }

    // Frame Manipulation

    void AdvanceFrame()
    {
        animationFrameData.AdvanceFrame();
        visualRep.currentFrame = animationFrameData.currentFrameNumber;
        visualRep.UpdateAnimationFrame(animationFrameData.currentFrameNumber);
    }

    void RewindFrame()
    {
        animationFrameData.RewindFrame();
        visualRep.currentFrame = animationFrameData.currentFrameNumber;
        visualRep.UpdateAnimationFrame(animationFrameData.currentFrameNumber);
    }

    public void GoToFrame(int newFrame)
    {
        animationFrameData.GoToFrame(newFrame);
        visualRep.currentFrame = animationFrameData.currentFrameNumber;
        visualRep.UpdateAnimationFrame(animationFrameData.currentFrameNumber);
    }

    // Hitbox Manipulation

    void AddHitbox()
    {
        if (animationFrameData.CurrentFrame.hitboxes.Count < 10)
        {
            animationFrameData.CurrentFrame.hitboxes.Add(new HitboxData());
        }
        else
        {
            Debug.Log("ADD HITBOX FAILED: Already at maximum number of hitboxes for a single frame (10).");
        }
    }

    void RemoveHitbox(int indexOfHitbox)
    {
        if (animationFrameData.CurrentFrame.hitboxes.Count > 0)
        {
            animationFrameData.CurrentFrame.hitboxes.RemoveAt(indexOfHitbox);
        }
        else
        {
            Debug.Log("REMOVE HITBOX FAILED: There are already zero hitboxes listed for this frame.");
        }
    }

    //public void EnsureValidActiveFrameNumber()
    //{
    //    if (activeFrame > animationFrameData.Count)
    //    {
    //        GoToFrame(animationFrameData.Count - 1);
    //    }
    //}
}
