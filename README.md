# SlnParser

[![Latest Release](https://img.shields.io/nuget/v/SlnParser?style=for-the-badge)](https://www.nuget.org/packages/SlnParser/)
[![Downloads](https://img.shields.io/nuget/dt/SlnParser?style=for-the-badge)](https://www.nuget.org/packages/SlnParser/)

.NET: Easy (to use) Parser for your .NET Solution (.sln) Files. This project targets `netstandard2.0` so it can basically be used anywhere you want. I've not yet run any performance tests.

## Usage

### Parsing

```cs

var parser = new SolutionParser();
var parsedSolution = parser.Parse("path/to/your/solution.sln");

```

### Accessing the projects

```cs

// gives you a flat list of all the Projects/Solution-Folders in your Solution
var flat = parsedSolution.AllProjects;

// gives you a structured (Solution-Folders containing projects) of all the Projects/Solution-Folders in your solution
var structured = parsedSolution.Projects;

// this'll give you all the projects that are not a Solution-Folder
var noFolders = parsedSolution
  .AllProjects
  .OfType<SolutionProject>();

// this'll give you all the projects of the desired type (C# class libs in this case)
var csharpProjects = parsedSolution
  .AllProjects
  .Where(project => project.Type == ProjectType.CSharpClassLibrary);

```

## Help me

The current list of [project types](src/Contracts/../SlnParser/Contracts/ProjectType.cs) is not yet **complete**.  
  
If you encounter any Projects where the `Type` is `ProjectType.Unknown` and you know for sure which project type that is (providing an example would be best) create an _Issue_ providing the `TypeGuid` and what kind of project that is. Thanks! 😊  
  
Additionally if you encounter any project whose type has been misidentified, let me know as well!
