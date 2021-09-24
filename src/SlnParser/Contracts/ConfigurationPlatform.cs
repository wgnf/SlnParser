namespace SlnParser.Contracts
{
    /// <summary>
    ///     A Configuration of a Solution or Project describing which <see cref="BuildConfiguration" /> and
    ///     <see cref="BuildPlatform" /> is targeted
    /// </summary>
    public class ConfigurationPlatform
    {
        /// <summary>
        ///     Create a new instance of <see cref="ConfigurationPlatform" />
        /// </summary>
        /// <param name="name">The name of the <see cref="ConfigurationPlatform" /></param>
        /// <param name="configuration">The <see cref="BuildConfiguration" /> of the <see cref="ConfigurationPlatform" /></param>
        /// <param name="platform">The <see cref="BuildPlatform" /> of the <see cref="ConfigurationPlatform" /></param>
        public ConfigurationPlatform(
            string name,
            BuildConfiguration configuration,
            BuildPlatform platform)
        {
            Name = name;
            Configuration = configuration;
            Platform = platform;
        }

        /// <summary>
        ///     The name of <see cref="ConfigurationPlatform" />
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The <see cref="BuildConfiguration" /> the <see cref="ConfigurationPlatform" /> is targeting
        /// </summary>
        public BuildConfiguration Configuration { get; }

        /// <summary>
        ///     The <see cref="BuildPlatform" /> the <see cref="ConfigurationPlatform" /> is targeting
        /// </summary>
        public BuildPlatform Platform { get; }
    }
}
