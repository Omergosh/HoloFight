using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;

    #region AudioClip assets
    // Menu UI sounds
    public AudioClip menuMoveSound;
    public AudioClip menuInvalidSound;
    public AudioClip menuConfirmSound;
    public AudioClip menuBackSound;
    public AudioClip deviceSetupJoinSound;
    public AudioClip deviceSetupCancelSound;
    public AudioClip charSelectReadySound;
    public AudioClip charSelectUnreadySound;

    // Battle SFX sounds


    // Ina SFX + voiceline sounds
    #endregion

    #region Audio methods for the public to invoke
    public void PlaySound(string soundTag)
    {
        switch (soundTag)
        {
            case "menuMove":
                audioSource.PlayOneShot(menuMoveSound);
                break;
            case "menuInvalid":
                audioSource.PlayOneShot(menuInvalidSound);
                break;
            case "menuConfirm":
                audioSource.PlayOneShot(menuConfirmSound);
                break;
            case "menuBack":
                audioSource.PlayOneShot(menuBackSound);
                break;
            case "deviceSetupJoin":
                audioSource.PlayOneShot(deviceSetupJoinSound);
                break;
            case "deviceSetupCancel":
                audioSource.PlayOneShot(deviceSetupCancelSound);
                break;
            case "charSelectReady":
                audioSource.PlayOneShot(charSelectReadySound);
                break;
            case "charSelectUnready":
                audioSource.PlayOneShot(charSelectUnreadySound);
                break;
            
            
            
            default:
                Debug.Log("Invalid sound effect attempted to be played.");
                break;
        }
    }
    #endregion

}
