using System;
using System.Runtime.Serialization;

namespace SlnParser.Contracts.Exceptions
{
	[Serializable]
	public class UnexpectedSolutionStructureException : Exception
	{
		public UnexpectedSolutionStructureException(string message) : base(message)
		{
		}

		public UnexpectedSolutionStructureException(string message, Exception inner) : base(message, inner)
		{
		}

		protected UnexpectedSolutionStructureException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}
}