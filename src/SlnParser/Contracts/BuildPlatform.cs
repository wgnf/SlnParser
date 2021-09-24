namespace SlnParser.Contracts
{
    /// <summary>
    ///     The Platform a Project or Solution Targets
    /// </summary>
    public enum BuildPlatform
    {
        /// <summary>
        ///     Any CPU-Platform is targeted
        /// </summary>
        AnyCpu,

        /// <summary>
        ///     Only x64 (64 Bit) CPU-Platforms are targeted
        /// </summary>
        X64,

        /// <summary>
        ///     Only x86 (32 Bit) CPU-Platforms are targeted
        /// </summary>
        X86
    }
}
