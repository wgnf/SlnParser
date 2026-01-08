using System.IO;

namespace SlnParser.Contracts
{
    /// <summary>
    ///     A parser that can parse the information of a provided solution file
    /// </summary>
    public interface ISolutionParser
    {
        /// <summary>
        ///     Parses the provided solution file (sln or slnx)
        /// </summary>
        /// <param name="solutionFileName">The path to the solution file that you want to parse</param>
        /// <returns>The parsed <see cref="Solution" /></returns>
        ISolution Parse(string solutionFileName);

        /// <summary>
        ///     Parses the provided solution file (sln or slnx)
        /// </summary>
        /// <param name="solutionFile">The path to the solution file that you want to parse</param>
        /// <returns>The parsed <see cref="Solution" /></returns>
        ISolution Parse(FileInfo solutionFile);

        /// <summary>
        ///     Safely Parses the provided solution file (sln or slnx)
        /// </summary>
        /// <param name="solutionFileName">The path to the solution file that you want to parse</param>
        /// <param name="solution">The parsed <see cref="Solution" /></param>
        /// <returns>If the solution could be successfully parsed or not</returns>
        bool TryParse(string solutionFileName, out ISolution? solution);

        /// <summary>
        ///     Safely Parses the provided solution file (sln or slnx)
        /// </summary>
        /// <param name="solutionFile">The path to the solution file that you want to parse</param>
        /// <param name="solution">The parsed <see cref="Solution" /></param>
        /// <returns>If the solution could be successfully parsed or not</returns>
        bool TryParse(FileInfo solutionFile, out ISolution? solution);
    }
}
