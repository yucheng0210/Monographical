using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour
{
    [SerializeField]
    private string sceneName;

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(Load);
    }

    private void Load()
    {
        StartCoroutine(SceneController.Instance.LoadLevel(sceneName));
    }
}
