using UnityEditor;
using UnityEngine;

namespace Ilumisoft.Minesweeper.Editor
{
    public static class MenuItems
    {
        [MenuItem("Minesweeper/Welcome")]
        static void OpenWelcomeWindow()
        {
            BundleUtilityWindow.Init();
        }

        [MenuItem("Minesweeper/Documentation")]
        static void ShowDocumentation()
        {
            var config = EditorConfiguration.Find();

            if (config != null)
            {
                AssetDatabase.OpenAsset(config.Documentation);
            }
        }

        [MenuItem("Minesweeper/Rate")]
        static void Rate()
        {
            var config = EditorConfiguration.Find();

            if (config != null)
            {
                Application.OpenURL(config.RateURL);
            }
        }

        [MenuItem("Minesweeper/More Assets")]
        static void MoreAssets()
        {
            var config = EditorConfiguration.Find();

            if (config != null)
            {
                Application.OpenURL(config.MoreAssetsURL);
            }
        }
    }
}