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
    public AudioClip hitJuicySound;
    public AudioClip hitCrowbarSound;
    public AudioClip hitHeavySound;
    public AudioClip hitVeryHeavySound;
    public AudioClip whooshQuickSound;
    public AudioClip whooshSlowSound;
    public AudioClip whooshHeavySound;

    // Misc/Unassigned sounds
    public AudioClip surfaceTapSound; //DM-CGS-44
    public AudioClip whooshSharpSound; //DM-CGS-46
    public AudioClip whooshBluntSound; //DM-CGS-47
    public AudioClip bounceSound; //DM-CGS-43
    public AudioClip fireCrossbowThwackSound; //DM-CGS-39
    public AudioClip sparkleSound; //DM-CGS-34
    public AudioClip popSound; //DM-CGS-32
    public AudioClip flashSparkleSound; //DM-CGS-33
    public AudioClip warpWeirdSound; //DM-CGS-29
    public AudioClip tickTockSound; //DM-CGS-04
    public AudioClip boomThinSound; //DM-CGS-05
    public AudioClip boomDeepSound; //DM-CGS-10


    // Ina SFX + voiceline sounds
    #endregion

    #region Audio methods for the public to invoke
    public void PlaySound(string soundTag)
    {
        switch (soundTag)
        {
            // Menu UI sounds
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

            // Battle SFX sounds
            case "hitJuicy":
                audioSource.PlayOneShot(hitJuicySound);
                break;
            case "hitCrowbar":
                audioSource.PlayOneShot(hitCrowbarSound);
                break;
            case "hitHeavy":
                audioSource.PlayOneShot(hitHeavySound);
                break;
            case "hitVeryHeavy":
                audioSource.PlayOneShot(hitVeryHeavySound);
                break;
            case "whooshQuick":
                audioSource.PlayOneShot(whooshQuickSound);
                break;
            case "whooshSlow":
                audioSource.PlayOneShot(whooshSlowSound);
                break;
            case "whooshHeavy":
                audioSource.PlayOneShot(whooshHeavySound);
                break;

            
            default:
                Debug.Log("Invalid sound effect attempted to be played.");
                break;
        }

        // WARNING: Although it helps compensate for sounds playing late,
        // this line may cause issues with sound clips that are incredibly short.
        // Consider investigating the issue with sounds playing late and finding an alternative, less risky solution.
        audioSource.time = 0.1f;
    }
    #endregion

}
