using System.Collections.Generic;

namespace SlnParser.Contracts.Helper
{
	internal interface IProjectParser
	{
		void Enrich(Solution solution, IEnumerable<string> fileContents);
	}
}