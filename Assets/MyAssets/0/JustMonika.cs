using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Runtime.InteropServices;
using System.Diagnostics;
public class JustMonika : MonoBehaviour
{
    [SerializeField]
    private float moveTime = 5;
    private bool isControlCursor;
    [DllImport("user32.dll")]
    public static extern int SetCursorPos(int x, int y);
    private int destinationX = 0;
    private int destinationY = 0;
    private float accumulateTime;
    private Vector2 cursorPos;
    private void Start()
    {
        EventManager.Instance.AddEventRegister(EventDefinition.eventDialogEvent, EventDialogEvent);
    }
    private void Update()
    {
         cursorPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
         UnityEngine.Debug.Log(cursorPos);
    }
    private IEnumerator UpdateCursorPos()
    {
        isControlCursor = true;
        while (isControlCursor)
        {
            float t = accumulateTime / moveTime;
            cursorPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            destinationX = (int)Mathf.Lerp(cursorPos.x, 960, t);
            destinationY = (int)Mathf.Lerp(cursorPos.y, 420, t);
            SetCursorPos(destinationX, destinationY);
            accumulateTime += Time.deltaTime;
            UnityEngine.Debug.Log(cursorPos);
            yield return null;
        }
    }
    private void EventDialogEvent(params object[] args)
    {
        if ((string)args[0] == "JustMonika")
            StartCoroutine(UpdateCursorPos());
    }
}
