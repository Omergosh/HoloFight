using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        
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
        }
    }
}
