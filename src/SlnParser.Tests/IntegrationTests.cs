using FluentAssertions;
using SlnParser.Contracts;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Xunit;

namespace SlnParser.Tests
{
    public class IntegrationTests
    {
		[Fact]
		[Category("ParseSolution:SlnParser")]
		public void Should_Be_Able_To_Parse_SlnParser_Solution_Correctly()
		{
			var solutionFile = LoadSolution("SlnParser");
			var sut = new SolutionParser();

			var solution = sut.Parse(solutionFile);

			solution
				.Name
				.Should()
				.Be("SlnParser");
			solution
				.File
				.FullName
				.Should()
				.Contain(@"Solutions\SlnParser.sln");

			solution
				.FileFormatVersion
				.Should()
				.Be("12.00");

			solution
				.VisualStudioVersion
				.Version
				.Should()
				.Be("17.0.31410.414");

			solution
				.VisualStudioVersion
				.MinimumVersion
				.Should()
				.Be("10.0.40219.1");

			solution
				.AllProjects
				.Should()
				.HaveCount(3);

			// 1. Project - ClassLib
			solution
				.AllProjects
				.ElementAt(0)
				.Should()
				.BeOfType<SolutionProject>();
			solution
				.AllProjects
				.ElementAt(0)
				.Name
				.Should()
				.Be("SlnParser");
			solution
				.AllProjects
				.ElementAt(0)
				.As<SolutionProject>()
				.File
				.FullName
				.Should()
				.Contain(@"SlnParser\SlnParser.csproj");
			solution
				.AllProjects
				.ElementAt(0)
				.Type
				.Should()
				.Be(ProjectType.CSharpClassLibrary);

			// 2. Project - Solution Folder
			solution
				.AllProjects
				.ElementAt(1)
				.Should()
				.BeOfType<SolutionFolder>();
			solution
				.AllProjects
				.ElementAt(1)
				.Name
				.Should()
				.Be("Solution Items");
			solution
				.AllProjects
				.ElementAt(1)
				.As<SolutionFolder>()
				.Projects
				.Should()
				.BeEmpty();
			solution
				.AllProjects
				.ElementAt(1)
				.Type
				.Should()
				.Be(ProjectType.SolutionFolder);

			// 3. Project - Test Project
			solution
				.AllProjects
				.ElementAt(2)
				.Should()
				.BeOfType<SolutionProject>();
			solution
				.AllProjects
				.ElementAt(2)
				.Name
				.Should()
				.Be("SlnParser.Tests");
			solution
				.AllProjects
				.ElementAt(2)
				.As<SolutionProject>()
				.File
				.FullName
				.Should()
				.Contain(@"SlnParser.Tests\SlnParser.Tests.csproj");
			solution
				.AllProjects
				.ElementAt(2)
				.Type
				.Should()
				.Be(ProjectType.CSharpClassLibrary);
		}

		[Fact]
		[Category("ParseSolution:TestSln")]
		public void Should_Be_Able_To_Parse_TestSln_Solution_Correctly()
		{
			var solutionFile = LoadSolution("TestSln");
			var sut = new SolutionParser();
			
			var solution = sut.Parse(solutionFile);

			solution
				.AllProjects
				.Should()
				.HaveCount(8);

			solution
				.Projects
				.Should()
				.HaveCount(4);
		}

		private static FileInfo LoadSolution(string solutionName)
		{
			var solutionFileName = $"./Solutions/{solutionName}.sln";
			var solutionFile = new FileInfo(solutionFileName);

			if (!solutionFile.Exists)
				throw new FileNotFoundException();

			return solutionFile;
		}
    }
}
