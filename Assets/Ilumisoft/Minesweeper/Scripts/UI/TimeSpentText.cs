using UnityEngine;
using UnityEngine.UI;

namespace Ilumisoft.Minesweeper.UI
{
    public class TimeSpentText : MonoBehaviour
    {
        [SerializeField]
        Text text = null;

        Timer timer;

        private void Awake()
        {
            timer = FindObjectOfType<Timer>();
        }

        private void OnEnable()
        {
            if (timer != null && text != null)
            {
                text.text = "TIME: "+timer.GetTime();
            }
        }
    }
}