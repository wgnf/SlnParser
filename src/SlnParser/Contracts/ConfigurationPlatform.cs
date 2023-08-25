namespace SlnParser.Contracts
{
    /// <summary>
    ///     A Configuration of a Solution or Project describing which configuration and build-platform is targeted
    /// </summary>
    public class ConfigurationPlatform
    {
        /// <summary>
        ///     Create a new instance of <see cref="ConfigurationPlatform" />
        /// </summary>
        /// <param name="name">The name of the <see cref="ConfigurationPlatform" /></param>
        /// <param name="configuration">The configuration of the <see cref="ConfigurationPlatform" /></param>
        /// <param name="platform">The build-platform of the <see cref="ConfigurationPlatform" /></param>
        public ConfigurationPlatform(
            string name,
            string configuration,
            string platform)
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
        ///     The configuration the <see cref="ConfigurationPlatform" /> is targeting
        /// </summary>
        public string Configuration { get; }

        /// <summary>
        ///     The build-platform the <see cref="ConfigurationPlatform" /> is targeting
        /// </summary>
        public string Platform { get; }
    }
}
