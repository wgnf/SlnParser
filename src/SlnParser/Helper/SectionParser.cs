using SlnParser.Contracts.Helper;
using System.Collections.Generic;
using System.Linq;

namespace SlnParser.Helper
{
    internal class SectionParser : ISectionParser
    {
        public IEnumerable<string> GetFileContentsInGlobalSection(
            IEnumerable<string> fileContents,
            string sectionName)
        {
            var startSection = $"GlobalSection({sectionName}";
            const string endSection = "EndGlobalSection";

            return GetFileContentsInSection(fileContents, startSection, endSection);
        }

        private static IEnumerable<string> GetFileContentsInSection(
            IEnumerable<string> fileContents,
            string startSection,
            string endSection)
        {
            var section = fileContents
                .SkipWhile(line => !line.StartsWith(startSection))
                .TakeWhile(line => !line.StartsWith(endSection))
                .Where(line => !line.StartsWith(startSection))
                .Where(line => !line.StartsWith(endSection))
                .Where(line => !string.IsNullOrWhiteSpace(line));

            return section.ToList();
        }
    }
}
