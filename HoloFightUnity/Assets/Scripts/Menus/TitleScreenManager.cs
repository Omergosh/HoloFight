using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerConfigurationManager.instance.InitializeOnTitleScreen();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
