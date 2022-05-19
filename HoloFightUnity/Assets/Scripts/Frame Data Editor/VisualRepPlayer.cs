using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// Front-end visual representation of a player based on their current in-game state
// (position, animation, etc.)
public class VisualRepPlayer : MonoBehaviour
{
    public Player playerToRepresent;
    public float pixelsInWorldUnit = 100f;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    // Variables to store sprites/images
    public Sprite[] sprites;
    public int currentFrame;
    public int totalFrames;

    // Asset set for this character's animations
    public CharacterAnimationSetScriptableObject characterAnimations;
    public string characterName;
    public string animationName;

    void Awake()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();
        animator.speed = 0;
    }

    //void Update()
    //{
    //    if(characterAnimations != null)
    //    {
    //        //Debug.Log(characterAnimations.idle);
    //        //Debug.Log(characterAnimations.run);
    //    }
    //}

    //void UpdateVisuals()
    //{
    //    //UpdateAnimationFrame();
    //}

    public void UpdateAnimationClip(string newCharacterName, string newAnimationName)
    {
        if (characterAnimations != null)
        {
            // Check if animation data associated with name exists in loaded AnimationSet
            //System.Type loadedAnimation = characterAnimations.GetType();
            //System.Reflection.FieldInfo loadedAnimation = characterAnimations.GetType().GetField(newAnimationName);
            //object loadedAnimation = characterAnimations.GetType().GetField(newAnimationName).GetValue(characterAnimations);
            AnimationClip loadedAnimation = (AnimationClip)characterAnimations.GetType().GetField(newAnimationName).GetValue(characterAnimations);
            Debug.Log(loadedAnimation);

            characterName = newCharacterName;
            animationName = newAnimationName;
            string setAnimatorAnimationString = characterName.ToLower() + "_" + animationName;
            animator.Play(setAnimatorAnimationString);

            //Debug.Log("Character animation not found.");

            // By default, set current frame to 0.
        }
    }

    public void UpdateAnimationFrame(int frameNumber)
    {
        if (characterAnimations != null)
        {
            // Check if current animation frame exists in loaded AnimationClip - if not, Clamp it.
            // Set visual animation

            //Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0]);
            //Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip);
            Debug.Log(animator.GetCurrentAnimatorStateInfo(0));
            Debug.Log(animator.GetCurrentAnimatorStateInfo(0).speed);
            Debug.Log(animator.GetCurrentAnimatorStateInfo(0).IsName(characterName.ToLower() + "_" + animationName));

            string stateName = characterName.ToLower() + "_" + animationName;
            Debug.Log(currentFrame);
            Debug.Log(totalFrames);
            Debug.Log((float)currentFrame / (float)totalFrames);
            animator.Play(stateName, -1, ((float)currentFrame / (float)totalFrames));
            //animator.PlayInFixedTime(stateName, -1, ((float)currentFrame / (float)totalFrames) * totalAnimationTime);
        }
    }

    // || Old method: grabbing individual sprites from files to use, instead of using the Animation asset type ||

    //void UpdateAnimationFrame()
    //{
    //    Sprite newSprite = sprites[playerToRepresent.currentAnimationState.currentFrameNumber];
    //    spriteRenderer.sprite = newSprite;
    //}

    //public void LoadCurrentAnimationFrames(string character = "INA", string animation = "run", int frameCount = 2)
    //{
    //    currentFrame = 0;
    //    AsyncOperationHandle<Sprite> spriteHandle = Addressables.LoadAssetAsync<Sprite>("Assets/Characters/Sprites/INA/ina_run0000.png");
    //    spriteHandle.Completed += LoadFrameSpriteWhenReady;
    //    spriteHandle = Addressables.LoadAssetAsync<Sprite>("Assets/Characters/Sprites/INA/ina_run0001.png");
    //    spriteHandle.Completed += LoadFrameSpriteWhenReady;
    //}

    //void LoadFrameSpriteWhenReady(AsyncOperationHandle<Sprite> handleToCheck)
    //{
    //    if(handleToCheck.Status == AsyncOperationStatus.Succeeded)
    //    {
    //        sprites[currentFrame] = handleToCheck.Result;
    //    }
    //    //currentFrame++;
    //    //if(currentFrame < sprites.Length) {}
    //}

    //void LoadAnimationFramesFromFile(string characterName, string animationName, int frameCount)
    //{
    //    // Load current values (for animation currently being viewed) from a json file
    //    // in the frame data / game data directory
    //    // (Assets -> Characters -> FrameData -> INA)
    //    string str;

    //    //using (FileStream fs = new FileStream("Assets/Characters/FrameData/FBK/testFrameData.json", FileMode.Open))

    //    using (FileStream fs = new FileStream($"Assets/Characters/Sprites/{characterName}/{animationName}.png", FileMode.Open))
    //    {
    //        using (StreamReader reader = new StreamReader(fs))
    //        {
    //            str = reader.ReadLine();
    //            //reader.Close();
    //            reader.Dispose();
    //        }
    //        //fs.Close();
    //        fs.Dispose();
    //    }

    //    var serializerSettings = new JsonSerializerSettings()
    //    {
    //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    //    };
    //    var loadedFrameData = JsonConvert.DeserializeObject<AnimationStateData>(str, serializerSettings);
    //    Debug.Log(loadedFrameData);
    //    animationFrameData = loadedFrameData;

    //    Debug.Log($"Loaded current animation's sprites! Character: {characterName} Animation: {animationName}");
    //}

}
