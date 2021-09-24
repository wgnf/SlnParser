using System;
using System.IO;
using System.Runtime.Serialization;

namespace SlnParser.Contracts.Exceptions
{
    /// <summary>
    ///     Exception that is thrown when a solution could not be parsed
    /// </summary>
    [Serializable]
    public class ParseSolutionFailedException : Exception
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ParseSolutionFailedException" />
        /// </summary>
        /// <param name="solutionFile">The solution file that could not be parsed</param>
        /// <param name="inner">The <see cref="Exception" /> that caused the parsing to fail</param>
        public ParseSolutionFailedException(FileInfo solutionFile, Exception inner)
            : base(
                $"Could not parse provided Solution '{solutionFile.FullName}'. See inner exception for more information",
                inner)
        {
        }

        /// <inheritdoc />
        protected ParseSolutionFailedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
