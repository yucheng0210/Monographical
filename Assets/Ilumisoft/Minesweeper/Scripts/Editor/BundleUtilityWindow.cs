using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ilumisoft.Minesweeper.Editor
{
    public class BundleUtilityWindow : EditorWindow
    {
        private const string sessionKey = "Ilumisoft.Editor.Minesweeper.ShownStartupMessage";

        BundleInfo bundleInfo = null;

        Vector2 scrollPos = Vector2.zero;

        List<PackageData> packageList = new List<PackageData>();

        [InitializeOnLoadMethod]
        static void InitShowAtStartup()
        {
            EditorApplication.update += ShowAtStartup;
        }

        static void ShowAtStartup()
        {
            EditorApplication.update -= ShowAtStartup;

            var startupMessageAsset = ScriptableObjectUtility.Find<BundleInfo>();

            // Dont't show window if data asset not found or window disabled
            if (startupMessageAsset == null || startupMessageAsset.ShowAtStartup == false)
            {
                return;
            }

            // Don't show the window multiple times during the same editor session
            if (SessionState.GetBool(sessionKey, false))
            {
                return;
            }

            if (!Application.isPlaying)
            {
                Init();
            }
        }

        public static void Init()
        {
            var window = GetWindow<BundleUtilityWindow>(utility: true);

            window.titleContent = new GUIContent("Welcome");

            window.maxSize = new Vector2(400, 700);
            window.minSize = window.maxSize;

            // Remember that the window has been shown during this editor session
            SessionState.SetBool(sessionKey, true);
        }

        private void OnEnable()
        {
            packageList.Clear();

            bundleInfo = ScriptableObjectUtility.Find<BundleInfo>();

            LoadPackageList();
        }

        private void LoadPackageList()
        {
            packageList.Clear();

            var guids = AssetDatabase.FindAssets($"t: {typeof(PackageData)}");

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<PackageData>(assetPath);

                if (asset != null)
                {
                    packageList.Add(asset);
                }
            }
        }

        void OnGUI()
        {
            OnHeaderGUI();

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            OnDocumentationGUI();
            OnRateGUI();
            OnPackageListGUI();
            GUILayout.EndScrollView();

            OnFooterGUI();
        }

        void OnHeaderGUI()
        {
            var headerStyle = new GUIStyle(EditorStyles.label);
            headerStyle.wordWrap = true;

            GUILayout.Label(new GUIContent(bundleInfo.BundleImage));

            GUILayout.Label($"Welcome to {bundleInfo.BundleTitle}! We hope you enjoy this free package.\nTo get started check out the documentation. If you like the template, we would be happy if you would take the time to give us a rating and check out our other games and assets in the Asset Store!", headerStyle);
        }

        void OnDocumentationGUI()
        {
            GUILayout.Space(14);

            GUILayout.Label("Get Started", EditorStyles.boldLabel);

            GUILayout.Space(8);

            GUILayout.BeginHorizontal();

            var config = ScriptableObjectUtility.Find<EditorConfiguration>();

            GUILayout.Label(new GUIContent("Documentation"));

            if (GUILayout.Button("Open", GUILayout.Width(100)))
            {
                AssetDatabase.OpenAsset(config.Documentation);
            }

            GUILayout.EndHorizontal();
        }

        void OnRateGUI()
        {
            GUILayout.Space(14);

            GUILayout.Label("Rate", EditorStyles.boldLabel);

            GUILayout.Space(8);

            GUILayout.BeginHorizontal();

            var config = ScriptableObjectUtility.Find<EditorConfiguration>();

            GUILayout.Label(new GUIContent("Do you like the template?"));

            if (GUILayout.Button("Rate", GUILayout.Width(100)))
            {
                AssetDatabase.OpenAsset(config.Documentation);
            }

            GUILayout.EndHorizontal();
        }

        void OnPackageListGUI()
        {
            GUILayout.Space(14);

            GUILayout.Label("More Assets", EditorStyles.boldLabel);

            GUILayout.Space(8);

            foreach (var package in packageList)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(new GUIContent(package.Name));

                if (GUILayout.Button("Show", GUILayout.Width(100)))
                {
                    Application.OpenURL(package.URL);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(4);
        }

        void OnFooterGUI()
        {
            Rect line = GUILayoutUtility.GetRect(position.width, 1);

            EditorGUI.DrawRect(line, Color.black);

            using (new GUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                bool show = GUILayout.Toggle(bundleInfo.ShowAtStartup, " Show on startup");

                if (EditorGUI.EndChangeCheck())
                {
                    bundleInfo.ShowAtStartup = show;
                    EditorUtility.SetDirty(bundleInfo);
                    AssetDatabase.SaveAssets();
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Close"))
                {
                    Close();
                }
            }
        }
    }

}