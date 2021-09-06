namespace SlnParser.Contracts
{
	/// <summary>
	///		The well-known <see cref="ProjectType"/>s
	/// </summary>
	public enum ProjectType
	{
		/// <summary>
		///		The <see cref="ProjectType"/> is not known
		/// </summary>
		Unknown,
		
		/// <summary>
		///		A Solution Folder
		/// </summary>
		SolutionFolder,
		
		/// <summary>
		///		A C# Class Library
		/// </summary>
		CSharpClassLibrary
	}
}
