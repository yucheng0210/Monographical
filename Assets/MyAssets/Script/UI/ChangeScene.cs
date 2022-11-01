using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChangeScene : MonoBehaviour
{
    [SerializeField]
    private string sceneName;

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(LoadScene);
    }

    private void LoadScene()
    {
        StartCoroutine(SceneController.Instance.Transition(sceneName));
    }
}
