namespace Ilumisoft.Minesweeper.Editor
{
    using UnityEngine;

    public class BundleInfo : ScriptableObject
    {
        [SerializeField]
        public Texture BundleImage = null;

        [SerializeField]
        public string BundleTitle = string.Empty;

        [SerializeField]
        public bool ShowAtStartup;
    }
}