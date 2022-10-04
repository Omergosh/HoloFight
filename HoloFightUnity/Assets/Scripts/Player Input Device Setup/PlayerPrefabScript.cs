using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefabScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(PlayerConfigurationManager.instance.transform);
    }
}
