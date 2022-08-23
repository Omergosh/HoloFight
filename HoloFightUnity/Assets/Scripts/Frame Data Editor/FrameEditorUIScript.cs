using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FrameEditorUIScript : MonoBehaviour
{
    public FrameDataEditorScript frameDataInfoSource;

    // Text/Integer Field references (to retrieve values from / set values)
    public TMP_InputField currentCharacterInputField;
    public TMP_InputField currentAnimationInputField;
    public TMP_Text currentFrameNumberTextDisplay;
    public TMP_InputField currentFrameTotalCountInputField;

    // Dropdown references
    public TMP_Dropdown hitboxDropdown;

    // Slider references
    public Slider currentFrameSlider;
    public Slider hitboxPositionXSlider;
    public Slider hitboxPositionYSlider;
    public Slider hitboxSizeXSlider;
    public Slider hitboxSizeYSlider;

    // Internal variables to keep track of UI values
    // (to help transition between UI slider values and game/data stored values)
    //Rect hitboxRect = new Rect();

    // Editor state
    public int currentHitboxToEdit = 0;

    void Update()
    {
        UpdateFrameDataUsingUIValues();
    }

    public void UpdateUIForAnimationLoad()
    {
        // Update text/number fields with loaded values.
        // Done only right when an animation is loaded.
        currentFrameTotalCountInputField.text = frameDataInfoSource.animationFrameData.frames.Count.ToString();
    }

    public void UpdateUIForFrame()
    {
        // Retrieve relevant values/references from object containing frame data
        AnimationStateData currentAnimation = frameDataInfoSource.animationFrameData;
        FrameData currentFrame = currentAnimation.CurrentFrame;

        // Current Frame slider
        currentFrameSlider.minValue = currentAnimation.frames.Count > 0 ? 1 : 0;
        currentFrameSlider.maxValue = currentAnimation.frames.Count;
        currentFrameSlider.SetValueWithoutNotify(currentAnimation.currentFrameNumber + 1);

        currentFrameNumberTextDisplay.text = (currentAnimation.currentFrameNumber + 1).ToString();

        // Hitbox Position + Size sliders
        if (currentFrame.hitboxes.Count > 0)
        {
            hitboxPositionXSlider.SetValueWithoutNotify(currentFrame.hitboxes[currentHitboxToEdit].hitboxRect.position.x);
            hitboxPositionYSlider.SetValueWithoutNotify(currentFrame.hitboxes[currentHitboxToEdit].hitboxRect.position.y);
            hitboxSizeXSlider.SetValueWithoutNotify(currentFrame.hitboxes[currentHitboxToEdit].hitboxRect.size.x);
            hitboxSizeYSlider.SetValueWithoutNotify(currentFrame.hitboxes[currentHitboxToEdit].hitboxRect.size.y);
        }
    }


    //public void OnFrameCountSubmitChanges()
    //{
    //    // When the frame count field is changed and the user 'submits' those changes,
    //    // the current/active animation's number of frames is changed to the new value.
    //    // Either existing frames are truncated, or new 'blank' frames are added.
    //    int newFrameCount = int.Parse(currentFrameTotalCountInputField.text);
    //    if (newFrameCount > frameDataSource.frameData.Count)
    //    {
    //        for (int i = 0; i < newFrameCount - frameDataSource.frameData.Count; i++)
    //        {
    //            frameDataInfoSource.frameData.Add(new FrameDataSingle());
    //        }
    //    }
    //    else if (newFrameCount < frameDataSource.frameData.Count)
    //    {
    //        for (int i = frameDataInfoSource.frameData.Count; i > frameDataInfoSource.frameData.Count - newFrameCount; i--)
    //        {
    //            frameDataInfoSource.frameData.RemoveAt(i - 1);
    //        }

    //        // Make sure that if the active frame no longer exists, a new active frame is selected.
    //        frameDataInfoSource.EnsureValidActiveFrameNumber();
    //    }
    //}

    //// Two commented out functions that actually just broke things instead of adding functionality.
    public void OnCurrentFrameSliderValueChange()
    {
        Debug.Log("current frame slider change");
        Debug.Log(currentFrameSlider.value);
        frameDataInfoSource.GoToFrame((int)currentFrameSlider.value - 1);
    }

    public void OnHitboxSliderValueChange()
    {
        Debug.Log("hitbox value slider change");
        UpdateFrameDataUsingUIValues();
        AnimationStateData currentAnimation = frameDataInfoSource.animationFrameData;
        FrameData currentFrame = currentAnimation.CurrentFrame;
        currentFrame.hitboxes[currentHitboxToEdit].hitboxRect.Set(
            hitboxPositionXSlider.value,
            hitboxPositionYSlider.value,
            hitboxSizeXSlider.value,
            hitboxSizeYSlider.value);
        //hitboxRect.Set(hitboxPositionXSlider.value,
        //    hitboxPositionYSlider.value,
        //    hitboxSizeXSlider.value,
        //    hitboxSizeYSlider.value);
    }

    public void OnAddHitboxButton()
    {
        Debug.Log("add new hitbox to current frame");
        AnimationStateData currentAnimation = frameDataInfoSource.animationFrameData;
        FrameData currentFrame = currentAnimation.CurrentFrame;
    }

    public void OnRemoveHitboxButton()
    {
        Debug.Log("remove currently selected hitbox");
        AnimationStateData currentAnimation = frameDataInfoSource.animationFrameData;
        FrameData currentFrame = currentAnimation.CurrentFrame;
    }

    public void UpdateFrameDataUsingUIValues()
    {
        AnimationStateData currentAnimation = frameDataInfoSource.animationFrameData;
        FrameData currentFrame = currentAnimation.CurrentFrame;

        // Only one hitbox is selected to edit its traits (via the sliders, etc.) at a time
        if (currentFrame.hitboxes.Count > 0)
        {
            currentFrame.hitboxes[currentHitboxToEdit].hitboxRect.Set(hitboxPositionXSlider.value,
                hitboxPositionYSlider.value,
                hitboxSizeXSlider.value,
                hitboxSizeYSlider.value);
        }
    }

    ////public void SetUIValues() { }
}
