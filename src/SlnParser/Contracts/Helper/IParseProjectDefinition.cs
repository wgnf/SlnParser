namespace SlnParser.Contracts.Helper
{
    internal interface IParseProjectDefinition
    {
        bool TryParseProjectDefinition(
            Solution solution,
            string projectDefinition,
            out IProject project);
    }
}
