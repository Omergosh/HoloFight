using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleVFXAnimEffect : MonoBehaviour
{
    public Animation animation;

    // Start is called before the first frame update
    void Start()
    {
        if (animation == null)
        {
            animation = GetComponent<Animation>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
