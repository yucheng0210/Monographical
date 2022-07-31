using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField]
    private string sceneName;

    public void Load()
    {
        StartCoroutine(SceneController.Instance.LoadLevel(sceneName));
    }
}
