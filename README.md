# 🛠️ SlnParser


[![GitHub license](https://img.shields.io/badge/Unlicense-MIT-blue.svg)](LICENSE)
[![GitHub stars](https://img.shields.io/github/stars/OptiSchmopti/CsvProc9000?style=social)](https://github.com/OptiSchmopti/CsvProc9000/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/OptiSchmopti/CsvProc9000?style=social)](https://github.com/OptiSchmopti/CsvProc9000/network/members)
[![GitHub watchers](https://img.shields.io/github/watchers/OptiSchmopti/CsvProc9000?style=social)](https://github.com/OptiSchmopti/CsvProc9000/watchers)  
[![Latest Release](https://img.shields.io/nuget/v/SlnParser?style=for-the-badge)](https://www.nuget.org/packages/SlnParser/)
[![Downloads](https://img.shields.io/nuget/dt/SlnParser?style=for-the-badge)](https://www.nuget.org/packages/SlnParser/)

🛠️ .NET: Easy (to use) Parser for your .NET Solution (.sln) Files. This project targets `netstandard2.0` so it can basically be used anywhere you want. I've not yet run any performance tests.

## 💻 Usage

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

## ⌨️ Developing

To develop and work with SlnParser you just need to clone this Repo somewhere on your PC and then open the Solution or the complete Source-Folder (under `src`) with your favorite IDE. No additional tools required.  
  
Before you can start, you should restore all NuGet-Packages using `dotnet restore` if that's not done for you by your IDE.

## 👋 Want to Contribute?

Cool! We're always welcoming anyone that wants to contribute to this project! Take a look at the [Contributing Guidelines](CONTRIBUTING.md), which helps you get started. You can also look at the [Open Issues](https://github.com/wgnf/SlnParser/issues) for getting more info about current or upcoming tasks.

## 💬 Want to discuss?

If you have any questions, doubts, ideas, problems or you simply want to present your opinions and views, feel free to hop into [Discussions](https://github.com/wgnf/SlnParser/discussions) and write about what you care about. We'd love to hear from you!

