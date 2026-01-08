using AwesomeAssertions;
using SlnParser.Contracts;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Xunit;

namespace SlnParser.Tests;

public class IntegrationTests
{
    [Fact]
    [Category("ParseSolution:Empty")]
    public void Parse_WithEmptySolutionFile_IsParsedCorrectly()
    {
        var solutionFile = LoadSolution("Empty");

        var sut = new SolutionParser();

        var solution = sut.Parse(solutionFile);

        solution
            .FileFormatVersion
            .Should()
            .Be(string.Empty);

        var visualStudioVersion = solution.VisualStudioVersion;

        visualStudioVersion
            .MinimumVersion
            .Should()
            .Be(string.Empty);

        visualStudioVersion
            .Version
            .Should()
            .Be(string.Empty);

        solution
            .Guid
            .Should()
            .Be(null);

        solution
            .ConfigurationPlatforms
            .Should()
            .HaveCount(0);

        solution
            .AllProjects
            .Should()
            .HaveCount(0);

        solution
            .Projects
            .Should()
            .HaveCount(0);
    }

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

        // -- Solution Configuration Platforms

        solution
            .ConfigurationPlatforms
            .Should()
            .HaveCount(6);

        solution
            .ConfigurationPlatforms
            .ElementAt(0)
            .Name
            .Should()
            .Be("Debug|Any CPU");

        solution
            .ConfigurationPlatforms
            .ElementAt(0)
            .Configuration
            .Should()
            .Be("Debug");

        solution
            .ConfigurationPlatforms
            .ElementAt(0)
            .Platform
            .Should()
            .Be("Any CPU");

        solution
            .ConfigurationPlatforms
            .ElementAt(1)
            .Name
            .Should()
            .Be("Debug|x64");

        solution
            .ConfigurationPlatforms
            .ElementAt(1)
            .Configuration
            .Should()
            .Be("Debug");

        solution
            .ConfigurationPlatforms
            .ElementAt(1)
            .Platform
            .Should()
            .Be("x64");

        solution
            .ConfigurationPlatforms
            .ElementAt(2)
            .Name
            .Should()
            .Be("Debug|x86");

        solution
            .ConfigurationPlatforms
            .ElementAt(2)
            .Configuration
            .Should()
            .Be("Debug");

        solution
            .ConfigurationPlatforms
            .ElementAt(2)
            .Platform
            .Should()
            .Be("x86");

        solution
            .ConfigurationPlatforms
            .ElementAt(3)
            .Name
            .Should()
            .Be("Release|Any CPU");

        solution
            .ConfigurationPlatforms
            .ElementAt(3)
            .Configuration
            .Should()
            .Be("Release");

        solution
            .ConfigurationPlatforms
            .ElementAt(3)
            .Platform
            .Should()
            .Be("Any CPU");

        solution
            .ConfigurationPlatforms
            .ElementAt(4)
            .Name
            .Should()
            .Be("Release|x64");

        solution
            .ConfigurationPlatforms
            .ElementAt(4)
            .Configuration
            .Should()
            .Be("Release");

        solution
            .ConfigurationPlatforms
            .ElementAt(4)
            .Platform
            .Should()
            .Be("x64");

        solution
            .ConfigurationPlatforms
            .ElementAt(5)
            .Name
            .Should()
            .Be("Release|x86");

        solution
            .ConfigurationPlatforms
            .ElementAt(5)
            .Configuration
            .Should()
            .Be("Release");

        solution
            .ConfigurationPlatforms
            .ElementAt(5)
            .Platform
            .Should()
            .Be("x86");

        // -- Projects
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
            .Be(ProjectType.CSharp);

        solution
            .AllProjects
            .ElementAt(0)
            .As<SolutionProject>()
            .ConfigurationPlatforms
            .Should()
            .Contain(config => config.Name.Equals("Debug|Any CPU.ActiveCfg"));

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
            .Be(ProjectType.CSharp);

        solution
            .AllProjects
            .ElementAt(2)
            .As<SolutionProject>()
            .ConfigurationPlatforms
            .Should()
            .Contain(config => config.Name.Equals("Debug|x86.Build.0"));
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

        var firstSolutionFolder = solution
            .AllProjects
            .OfType<SolutionFolder>()
            .FirstOrDefault(folder => folder.Name == "SolutionFolder1");

        Assert.NotNull(firstSolutionFolder);

        firstSolutionFolder
            .Files
            .Should()
            .Contain(file => file.Name == "something.txt" ||
                             file.Name == "test123.txt" ||
                             file.Name == "test456.txt");

        var nestedSolutionFolder = solution
            .AllProjects
            .OfType<SolutionFolder>()
            .FirstOrDefault(folder => folder.Name == "NestedSolutionFolder");

        Assert.NotNull(nestedSolutionFolder);

        nestedSolutionFolder
            .Files
            .Should()
            .Contain(file => file.Name == "testNested1.txt");
    }

    [Fact]
    [Category("ParseSolution:ProjectWithoutPlatform")]
    public void Parse_WithProjectWithoutPlatform_IsParsedCorrectly()
    {
        var solutionFile = LoadSolution("ProjectWithoutPlatform");

        var sut = new SolutionParser();

        var solution = sut.Parse(solutionFile);

        solution
            .ConfigurationPlatforms
            .Should()
            .HaveCount(1);

        var configurationPlatform = solution
            .ConfigurationPlatforms
            .Single();

        configurationPlatform
            .Configuration
            .Should()
            .Be("SolutionConfigurationName");

        configurationPlatform
            .Platform
            .Should()
            .Be("SolutionPlatformName");

        solution
            .AllProjects
            .Should()
            .HaveCount(1);

        solution
            .Projects
            .Should()
            .HaveCount(1);

        var project = solution.Projects.Single();
        project.Id.Should().Be("D5BDBC46-CEAF-4C92-8335-31450B76914F");
        project.Name.Should().Be("Test");
        project.TypeGuid.Should().Be("D183A3D8-5FD8-494B-B014-37F57B35E655");
        project.Type.Should().Be(ProjectType.Unknown);
    }
 
    [Fact]
    [Category("ParseSolution:SolutionGuid")]
    public void Parse_WithSolutionGuid_IsParsedCorrectly()
    {
        var solutionFile = LoadSolution("SolutionGuid");
          
        var sut = new SolutionParser();

        var solution = sut.Parse(solutionFile);

        solution
            .Guid
            .Should()
            .Be("7F92F20E-4C3D-4316-BF60-105559EFEAFF");
    }
        
    [Fact]
    [Category("ParseSlnxSolution:Empty")]
    public void ParseSlnx_WithEmptySolutionFile_IsParsedCorrectly()
    {
        var solutionFile = LoadXmlSolution("Empty");

        var sut = new SolutionParser();

        var solution = sut.Parse(solutionFile);

        solution
            .FileFormatVersion
            .Should()
            .Be(string.Empty);

        var visualStudioVersion = solution.VisualStudioVersion;

        visualStudioVersion
            .MinimumVersion
            .Should()
            .Be(string.Empty);

        visualStudioVersion
            .Version
            .Should()
            .Be(string.Empty);

        solution
            .Guid
            .Should()
            .Be(null);

        solution
            .ConfigurationPlatforms
            .Should()
            .HaveCount(0);

        solution
            .AllProjects
            .Should()
            .HaveCount(0);

        solution
            .Projects
            .Should()
            .HaveCount(0);
    }

    [Fact]
    [Category("ParseSlnxSolution:SlnParser")]
    public void Should_Be_Able_To_Parse_SlnParser_SlnxSolution_Correctly()
    {
        var solutionFile = LoadXmlSolution("SlnParser");
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

        // -- Solution Configuration Platforms

        solution
            .ConfigurationPlatforms
            .Should()
            .HaveCount(6);

        solution
            .ConfigurationPlatforms
            .ElementAt(0)
            .Name
            .Should()
            .Be("Debug|Any CPU");

        solution
            .ConfigurationPlatforms
            .ElementAt(0)
            .Configuration
            .Should()
            .Be("Debug");

        solution
            .ConfigurationPlatforms
            .ElementAt(0)
            .Platform
            .Should()
            .Be("Any CPU");

        solution
            .ConfigurationPlatforms
            .ElementAt(1)
            .Name
            .Should()
            .Be("Debug|x64");

        solution
            .ConfigurationPlatforms
            .ElementAt(1)
            .Configuration
            .Should()
            .Be("Debug");

        solution
            .ConfigurationPlatforms
            .ElementAt(1)
            .Platform
            .Should()
            .Be("x64");

        solution
            .ConfigurationPlatforms
            .ElementAt(2)
            .Name
            .Should()
            .Be("Debug|x86");

        solution
            .ConfigurationPlatforms
            .ElementAt(2)
            .Configuration
            .Should()
            .Be("Debug");

        solution
            .ConfigurationPlatforms
            .ElementAt(2)
            .Platform
            .Should()
            .Be("x86");

        solution
            .ConfigurationPlatforms
            .ElementAt(3)
            .Name
            .Should()
            .Be("Release|Any CPU");

        solution
            .ConfigurationPlatforms
            .ElementAt(3)
            .Configuration
            .Should()
            .Be("Release");

        solution
            .ConfigurationPlatforms
            .ElementAt(3)
            .Platform
            .Should()
            .Be("Any CPU");

        solution
            .ConfigurationPlatforms
            .ElementAt(4)
            .Name
            .Should()
            .Be("Release|x64");

        solution
            .ConfigurationPlatforms
            .ElementAt(4)
            .Configuration
            .Should()
            .Be("Release");

        solution
            .ConfigurationPlatforms
            .ElementAt(4)
            .Platform
            .Should()
            .Be("x64");

        solution
            .ConfigurationPlatforms
            .ElementAt(5)
            .Name
            .Should()
            .Be("Release|x86");

        solution
            .ConfigurationPlatforms
            .ElementAt(5)
            .Configuration
            .Should()
            .Be("Release");

        solution
            .ConfigurationPlatforms
            .ElementAt(5)
            .Platform
            .Should()
            .Be("x86");

        // -- Projects
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
            .Be(ProjectType.CSharp);

        solution
            .AllProjects
            .ElementAt(0)
            .As<SolutionProject>()
            .ConfigurationPlatforms
            .Should()
            .Contain(config => config.Name.Equals("Debug|Any CPU.ActiveCfg"));

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
            .Be(ProjectType.CSharp);

        solution
            .AllProjects
            .ElementAt(2)
            .As<SolutionProject>()
            .ConfigurationPlatforms
            .Should()
            .Contain(config => config.Name.Equals("Debug|x86.Build.0"));
    }

    [Fact]
    [Category("ParseSlnxSolution:TestSln")]
    public void Should_Be_Able_To_Parse_TestSln_SlnxSolution_Correctly()
    {
        var solutionFile = LoadXmlSolution("TestSln");
        var sut = new SolutionParser();

        var solution = sut.Parse(solutionFile);

        solution
            .AllProjects
            .Should()
            .HaveCount(8);

        solution
            .Projects
            .Should()
            .HaveCount(8);

        var firstSolutionFolder = solution
            .AllProjects
            .OfType<SolutionFolder>()
            .FirstOrDefault(folder => folder.Name == "SolutionFolder1");

        Assert.NotNull(firstSolutionFolder);

        firstSolutionFolder
            .Files
            .Should()
            .Contain(file => file.Name == "something.txt" ||
                             file.Name == "test123.txt" ||
                             file.Name == "test456.txt");

        var nestedSolutionFolder = solution
            .AllProjects
            .OfType<SolutionFolder>()
            .FirstOrDefault(folder => folder.Name == "NestedSolutionFolder");

        Assert.NotNull(nestedSolutionFolder);

        nestedSolutionFolder
            .Files
            .Should()
            .Contain(file => file.Name == "testNested1.txt");
    }

    [Fact]
    [Category("ParseSlnxSolution:ProjectWithoutPlatform")]
    public void ParseSlnx_WithProjectWithoutPlatform_IsParsedCorrectly()
    {
        var solutionFile = LoadXmlSolution("ProjectWithoutPlatform");

        var sut = new SolutionParser();

        var solution = sut.Parse(solutionFile);

        solution
            .ConfigurationPlatforms
            .Should()
            .HaveCount(1);

        var configurationPlatform = solution
            .ConfigurationPlatforms
            .Single();

        configurationPlatform
            .Configuration
            .Should()
            .Be("SolutionConfigurationName");

        configurationPlatform
            .Platform
            .Should()
            .Be("SolutionPlatformName");

        solution
            .AllProjects
            .Should()
            .HaveCount(1);

        solution
            .Projects
            .Should()
            .HaveCount(1);

        var project = solution.Projects.Single();
        project.Id.Should().Be("D5BDBC46-CEAF-4C92-8335-31450B76914F");
        project.Name.Should().Be("Test");
        project.TypeGuid.Should().Be("D183A3D8-5FD8-494B-B014-37F57B35E655");
        project.Type.Should().Be(ProjectType.Unknown);
    }
   
    private static FileInfo LoadSolution(string solutionName)
    {
        var solutionFileName = $"./Solutions/{solutionName}.sln";
        var solutionFile = new FileInfo(solutionFileName);

        return !solutionFile.Exists ? throw new FileNotFoundException() : solutionFile;
    }
        
    private static FileInfo LoadXmlSolution(string solutionName)
    {
        var solutionFileName = $"./Solutions/{solutionName}.slnx";
        var solutionFile = new FileInfo(solutionFileName);

        return !solutionFile.Exists ? throw new FileNotFoundException() : solutionFile;
    }
}
