using UnityEngine;

namespace Ilumisoft.Minesweeper.Editor
{
    public class EditorConfiguration : ScriptableObject
    {
        [SerializeField]
        Object documentation;

        [SerializeField]
        string rateURL;

        [SerializeField]
        string moreAssetsURL;

        public Object Documentation { get => this.documentation; set => this.documentation = value; }
        public string RateURL { get => this.rateURL; set => this.rateURL = value; }
        public string MoreAssetsURL { get => this.moreAssetsURL; set => this.moreAssetsURL = value; }

        public static EditorConfiguration Find() => ScriptableObjectUtility.Find<EditorConfiguration>();
    }
}