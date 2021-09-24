using System;

namespace SlnParser.Contracts
{
    /// <summary>
    ///     A project that can be contained in a <see cref="Solution" />
    /// </summary>
    public interface IProject
    {
        /// <summary>
        ///     The Id of the Project
        /// </summary>
        Guid Id { get; }

        /// <summary>
        ///     The Name of the Project
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The Id of the Project-Type
        /// </summary>
        Guid TypeGuid { get; }

        /// <summary>
        ///     The well-known <see cref="Type" />
        /// </summary>
        ProjectType Type { get; }
    }
}
