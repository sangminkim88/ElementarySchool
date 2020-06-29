namespace GAIA2020.Utilities
{
    using GaiaDB.Enums;
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;

    /// <summary>
    /// Defines the <see cref="KeyControlPair" />.
    /// </summary>
    public class KeyControlPair
    {
        #region Fields

        /// <summary>
        /// Defines the lazy.
        /// </summary>
        private static readonly Lazy<KeyControlPair> lazy =
            new Lazy<KeyControlPair>(() => new KeyControlPair());

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="KeyControlPair"/> class from being created.
        /// </summary>
        private KeyControlPair()
        {
            Rock.Add(new Tuple<string, string>("편마암", "rock0"));
            Rock.Add(new Tuple<string, string>("편암", "rock1"));
            Rock.Add(new Tuple<string, string>("각섬암", "rock2"));
            Rock.Add(new Tuple<string, string>("석회암", "rock3"));
            Rock.Add(new Tuple<string, string>("사암", "rock4"));
            Rock.Add(new Tuple<string, string>("휘록암", "rock5"));
            Rock.Add(new Tuple<string, string>("역암", "rock6"));
            Rock.Add(new Tuple<string, string>("화강암", "rock7"));
            Rock.Add(new Tuple<string, string>("섬록암", "rock8"));
            Rock.Add(new Tuple<string, string>("감람암", "rock9"));
            Rock.Add(new Tuple<string, string>("사문암", "rock10"));
            Rock.Add(new Tuple<string, string>("유문암", "rock11"));
            Rock.Add(new Tuple<string, string>("셰일", "rock12"));
            Rock.Add(new Tuple<string, string>("안산암", "rock13"));
            Rock.Add(new Tuple<string, string>("현무암", "rock14"));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Weathered, 0, "신선", "D1"));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Weathered, 1, "약간풍화", "D2"));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Weathered, 2, "보통풍화", "D3"));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Weathered, 3, "심한풍화", "D4"));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Weathered, 4, "완전풍화", "D5"));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Weathered, 5, "잔적토", "D6"));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Stiffness, 0, "매우강함", "S1"));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Stiffness, 1, "강함", "S2"));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Stiffness, 2, "보통강함", "S3"));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Stiffness, 3, "약함", "S4"));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Stiffness, 4, "매우약함", "S5"));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Rough, 0, "계단형/거침", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Rough, 1, "계단형/완만", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Rough, 2, "계단형/경면", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Rough, 3, "파동형/거침", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Rough, 4, "파동형/완만", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Rough, 5, "파동형/경면", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Rough, 6, "평면형/거침", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Rough, 7, "평면형/완면", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Rough, 8, "평면형/경면", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Gap, 0, "200cm 이상", "F1"));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Gap, 1, "60~200cm", "F2"));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Gap, 2, "20~60cm", "F3"));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Gap, 3, "6~20cm", "F4"));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Gap, 4, "6cm 이하", "F5"));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Wet, 0, "건조", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Wet, 1, "습윤", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Wet, 2, "젖음", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Wet, 3, "포화", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Density, 0, "매우느슨", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Density, 1, "느슨", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Density, 2, "보통조밀", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Density, 3, "조밀", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Density, 4, "매우조밀", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Density, 5, "매우연약", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Density, 6, "연약", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Density, 7, "보통견고", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Density, 8, "견고", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Density, 9, "매우견고", string.Empty));
            CheckBoxes.Add(new Tuple<eDescriptionKey, int, string, string>(eDescriptionKey.Density, 10, "고결", string.Empty));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Instance.
        /// </summary>
        public static KeyControlPair Instance
        {
            get { return lazy.Value; }
        }

        #endregion

        public List<Tuple<string, string>> Rock = new List<Tuple<string, string>>();
        public List<Tuple<eDescriptionKey, int , string, string>> CheckBoxes = new List<Tuple<eDescriptionKey, int, string, string>> ();
    }
}
