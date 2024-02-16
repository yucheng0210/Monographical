using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ilumisoft.Minesweeper.UI
{
    [RequireComponent(typeof(Button))]
    public class LoadSceneButton : MonoBehaviour
    {
        [SerializeField]
        string sceneName = string.Empty;

        SceneLoader sceneLoader;

        Button button;

        private void Awake()
        {
            sceneLoader = FindObjectOfType<SceneLoader>();

            button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            if (sceneLoader != null)
            {
                sceneLoader.LoadScene(sceneName);
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}