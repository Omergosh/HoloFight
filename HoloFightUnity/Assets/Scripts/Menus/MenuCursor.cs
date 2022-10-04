using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCursor : MonoBehaviour
{
    [SerializeField]
    Vector3 cursorOffset; // offset of cursor position from selected MenuOption

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCursorPosition(MenuOption target)
    {
        transform.position = target.transform.position + cursorOffset;
        RectTransform cursorRect = GetComponent<RectTransform>();
        RectTransform targetRect = target.GetComponent<RectTransform>();
        Debug.Log(GetComponent<RectTransform>().rect);
        Debug.Log(target.GetComponent<RectTransform>().rect);
        cursorRect.Translate(
                            targetRect.rect.x - cursorRect.rect.x,
                            targetRect.rect.y - cursorRect.rect.y,
                            0
                            );
    }
}
