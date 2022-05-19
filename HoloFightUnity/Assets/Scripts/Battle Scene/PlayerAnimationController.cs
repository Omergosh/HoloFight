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
}
