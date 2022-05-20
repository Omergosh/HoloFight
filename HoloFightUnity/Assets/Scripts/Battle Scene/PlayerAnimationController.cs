using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    // Player state
    public Player player;

    public string characterName = "INA";
    public string animationName;

    // Assets (animations)
    public CharacterAnimationSetScriptableObject characterAnimations;

    // References
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    // Collision box / hurtbox / hitbox visual representation references
    //public Transform collisionBoxRepresentation;
    public Transform hurtboxRepresentation;
    public Transform[] hitboxRepresentations;

    // Option / toggles
    public bool displayCollisionBoxes = true;
    public bool displayHurtboxes = true;
    public bool displayHitboxes = true;

    // Constants
    public const float pixelsInWorldUnit = 100f;

    // Start is called before the first frame update
    void Start()
    {
        if (animator != null)
        {
            animator.speed = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            UpdateVisuals();
        }
    }

    public void SetPlayer(ref Player playerToRepresent)
    {
        player = playerToRepresent;
    }

    public void UpdateVisuals()
    {
        if (player != null)
        {
            this.gameObject.transform.position = new Vector3(
                    player.position.x - (HfGame.bounds.width / 2),
                    player.position.y - (HfGame.bounds.height / 2)
                    ) / GameManagerScript.pixelsInWorldUnit;

            //if (spriteRenderer != null)
            //{
            //    spriteRenderer.flipX = !player.facingRight;
            //}
            transform.localScale = new Vector3(
                player.facingRight ? Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x) * -1,
                transform.localScale.y,
                transform.localScale.z
                );

            if(animator != null && characterAnimations != null)
            {
                UpdateAnimations();
            }

            UpdateBoxRepresentations();
        }
    }

    public void UpdateAnimations()
    {
        //Debug.Log("Animating!");

        string characterName = player.characterName.ToUpper();
        animationName = player.currentAnimationState.animationName;
        string animatorStateName = characterName.ToLower() + "_" + animationName;
        int currentFrame = player.currentAnimationState.currentFrameNumber;
        int totalFrames = player.currentAnimationState.frames.Count;
        animator.Play(animatorStateName, -1, (float)currentFrame / (float)totalFrames);
    }

    public void UpdateBoxRepresentations()
    {
        // Collision box (currently unimplemented in game state - nothing to display yet)
        // [DO LATER]

        // Hurtbox (blue box)
        if (hurtboxRepresentation != null)
        {
            if (displayHurtboxes)
            {
                hurtboxRepresentation.gameObject.SetActive(true);

                Rect hurtboxRect = player.GetHurtboxRelative();
                hurtboxRepresentation.localPosition = new Vector3(
                    (hurtboxRect.x + (hurtboxRect.width / 2f)) / pixelsInWorldUnit,
                    (hurtboxRect.y - (hurtboxRect.height / 2f)) / pixelsInWorldUnit,
                    hurtboxRepresentation.localPosition.z
                );
                hurtboxRepresentation.localScale = new Vector3(
                    hurtboxRect.width / pixelsInWorldUnit,
                    hurtboxRect.height / pixelsInWorldUnit,
                    hurtboxRepresentation.localScale.z
                );
            }
            else
            {
                hurtboxRepresentation.gameObject.SetActive(false);
            }
        }

        // Hitboxes (red boxes)
        for (int i = 0; i < hitboxRepresentations.Length; i++)
        {
            if (hitboxRepresentations[i] != null)
            {
                if (displayHitboxes)
                {
                    if (i < player.currentAnimationState.CurrentFrame.hitboxes.Count)
                    {
                        hitboxRepresentations[i].gameObject.SetActive(true);

                        Debug.Log("Display a hitbox");
                        Rect hitboxRect = player.currentAnimationState.CurrentFrame.hitboxes[i].hitboxRect;
                        Debug.Log(hitboxRect);
                        player.GetHitboxes();

                        //float facingRightMultiplier = (player.facingRight) ? 1f : -1f;
                        //Debug.Log(facingRightMultiplier);
                        //Debug.Log(player.facingRight);

                        hitboxRepresentations[i].localPosition = new Vector3(
                            ((hitboxRect.x + hitboxRect.width/2) / pixelsInWorldUnit)/* * facingRightMultiplier*/,
                            ((hitboxRect.y + hitboxRect.height/2) / pixelsInWorldUnit),
                            hitboxRepresentations[i].localPosition.z
                        );
                        hitboxRepresentations[i].localScale = new Vector3(
                            hitboxRect.width / pixelsInWorldUnit,
                            hitboxRect.height / pixelsInWorldUnit,
                            hitboxRepresentations[i].localScale.z
                        );
                    }
                    else
                    {
                        hitboxRepresentations[i].gameObject.SetActive(false);
                    }
                }
                else
                {
                    hitboxRepresentations[i].gameObject.SetActive(false);
                }
            }
        }
    }

    float ConvertFrameDataMeasureToUnityUnits(float frameDataSizeValue)
    {
        return frameDataSizeValue / pixelsInWorldUnit;
    }
}
