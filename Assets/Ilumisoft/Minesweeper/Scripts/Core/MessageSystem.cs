using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Ilumisoft.Minesweeper
{
    public static class MessageSystem
    {
        public static void Send<T>(UnityAction<T> action)
        {
            var listeners = Object.FindObjectsOfType<MonoBehaviour>().OfType<T>();

            foreach (T listener in listeners)
            {
                action?.Invoke(listener);
            }
        }
    }
}