using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Runtime.InteropServices;
using System.Diagnostics;
public class JustMonika : MonoBehaviour
{
    [DllImport("user32.dll")]
    public static extern int SetCursorPos(int x, int y);

    private bool isControlCursor;
    private Vector2 cursorPos;
    private void Start()
    {
        EventManager.Instance.AddEventRegister(EventDefinition.eventDialogEvent, EventDialogEvent);
    }
    private IEnumerator UpdateCursorPos()
    {
        isControlCursor = true;
        cursorPos = new Vector2(Input.mousePosition.x, 1080 - Input.mousePosition.y);
        while (isControlCursor)
        {
            if (cursorPos.x > 920)
            {
                if (cursorPos.y > 420)
                    SetCursorPos((int)--cursorPos.x, (int)--cursorPos.y);
            }
            yield return null;
        }
    }
    private void EventDialogEvent(params object[] args)
    {
        if ((string)args[0] == "JustMonika")
            StartCoroutine(UpdateCursorPos());
    }
}
