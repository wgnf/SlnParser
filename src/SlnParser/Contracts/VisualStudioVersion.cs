namespace SlnParser.Contracts
{
    /// <summary>
    ///     A Visual Studio Version
    /// </summary>
    public class VisualStudioVersion
    {
        /// <summary>
        ///     The actually used Version of Visual Studio
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        ///     The minimum Version of Visual Studio that is compatible
        /// </summary>
        public string MinimumVersion { get; set; } = string.Empty;
    }
}
