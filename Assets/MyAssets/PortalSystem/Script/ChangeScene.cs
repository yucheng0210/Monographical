using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour
{
    [SerializeField]
    private string sceneName;

    [SerializeField]
    private bool immediatelyTrans;

    private void Start()
    {
        if (!immediatelyTrans)
            GetComponent<Button>().onClick.AddListener(LoadScene);
    }

    private void LoadScene()
    {
        StartCoroutine(SceneController.Instance.Transition(sceneName));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (immediatelyTrans && other.CompareTag("Player"))
            StartCoroutine(SceneController.Instance.Transition(sceneName));
    }
}
