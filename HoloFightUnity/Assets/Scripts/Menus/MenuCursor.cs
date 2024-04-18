using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCursor : MonoBehaviour
{
    [SerializeField]
    Vector3 cursorOffset; // offset of cursor position from selected MenuOption
    [SerializeField]
    List<Image> imagesToColorChange;

    public void SetCursorPosition(MenuOption target)
    {
        transform.position = target.transform.position + cursorOffset;
        //RectTransform cursorRect = GetComponent<RectTransform>();
        //RectTransform targetRect = target.GetComponent<RectTransform>();
        //Debug.Log(GetComponent<RectTransform>().rect);
        //Debug.Log(target.GetComponent<RectTransform>().rect);
        //cursorRect.Translate(
        //                    targetRect.rect.x - cursorRect.rect.x,
        //                    targetRect.rect.y - cursorRect.rect.y,
        //                    0
        //                    );
    }

    public void SetColor(Color newColor)
    {
        if (imagesToColorChange != null)
        {
            foreach (Image image in imagesToColorChange)
            {
                image.color = newColor;
            }
        }
    }
}
