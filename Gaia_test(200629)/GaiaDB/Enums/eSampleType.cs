namespace GaiaDB.Enums
{
    #region Enums

    /// <summary>
    /// Defines the eSampleType.
    /// </summary>
    public enum eSampleType
    {
        /// <summary>
        /// Defines the None.
        /// </summary>
        None = 4,
        /// <summary>
        /// 자연시료 Undisturbed Sample
        /// </summary>
        UD = 0,
        /// <summary>
        /// 흐트러진시료 Disturbed Sample
        /// </summary>
        DS,
        /// <summary>
        /// 흐트러진시료 No Sample
        /// </summary>
        NS,
        /// <summary>
        /// 코아시료 No Sample
        /// </summary>
        CS,
    }

    #endregion
}
