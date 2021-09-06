﻿using System.Collections.Generic;
using System.IO;

namespace SlnParser.Contracts
{
	/// <summary>
	///		A class representing all the information contained in a Visual Studio Solution File (sln)
	/// </summary>
    public class Solution
    {
		/// <summary>
		///		Creates a new instance of <see cref="Solution"/>
		/// </summary>
		public Solution()
		{
			Projects = new List<IProject>();
		}

		/// <summary>
		///		The name of the solution
		/// </summary>
        public string Name { get; set; }

		/// <summary>
		///		The File of the solution
		/// </summary>
        public FileInfo File { get; set; }

		/// <summary>
		///		The file format version of the solution
		/// </summary>
		public string FileFormatVersion { get; set; }

		/// <summary>
		///		The <see cref="VisualStudioVersion"/> of the solution
		/// </summary>
		public VisualStudioVersion VisualStudioVersion { get; set; }

		/// <summary>
		///		The <see cref="IProject"/>s contained in the solution
		/// </summary>
		public IReadOnlyCollection<IProject> Projects { get; internal set; }
    }
}
