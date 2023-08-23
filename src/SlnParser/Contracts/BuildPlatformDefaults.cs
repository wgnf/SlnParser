namespace SlnParser.Contracts
{
    /// <summary>
    ///     Common build-platforms defaults
    /// </summary>
    public static class BuildPlatformDefaults
    {
        /// <summary>
        ///     Any CPU-platform is targeted
        /// </summary>
        public const string AnyCpu = "Any CPU";
        
        /// <summary>
        ///     Only x64 (64 bit) CPU-platforms are targeted
        /// </summary>
        public const string X64 = "x64";
        
        /// <summary>
        ///     Only x86 (32 bit) CPU-platforms are targeted
        /// </summary>
        public const string X86 = "x86";
        
        /// <summary>
        ///     There is a mix of different platforms that are targeted
        /// </summary>
        public const string MixedPlatforms = "Mixed Platforms";
        
        /// <summary>
        ///     Only Windows 32 platforms are targeted
        /// </summary>
        public const string Win32 = "Win32";

        /// <summary>
        ///     Only Windows 7 platforms are targeted
        /// </summary>
        public const string Win7 = "Win7";
    }
}
