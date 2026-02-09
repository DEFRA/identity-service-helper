#addin nuget:?package=Cake.Coverlet&version=5.1.1
#tool dotnet:?package=GitVersion.Tool&version=6.5.1
#tool dotnet:?package=dotnet-reportgenerator-globaltool&version=5.5.1

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
const string TEST_COVERAGE_OUTPUT_DIR = "coverage";
const string SolutionFileName = "IdentityServiceHelper.slnx";

Task("Clean")
    .Does(() => {
 
   if (BuildSystem.GitHubActions.IsRunningOnGitHubActions)
    {
      Information("Nothing to clean on Azure Pipelines.");
    }
    else
    {
        DotNetClean(SolutionFileName);
    }
});

Task("Restore")
    .IsDependentOn("Clean")
    .Description("Restoring the solution dependencies")
    .Does(() => {
    
    Information("Restoring the solution dependencies");
      var settings =  new DotNetRestoreSettings
        {
          Verbosity = DotNetVerbosity.Minimal,
          Sources = new [] { 
             "https://api.nuget.org/v3/index.json",
          }
        };
   GetFiles("./**/**/*.csproj").ToList().ForEach(project => {
       Information($"Restoring {project.ToString()}");
       DotNetRestore(project.ToString(), settings);
     });
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() => {
     var buildSettings = new DotNetBuildSettings {
                        Configuration = configuration,
                       };
     GetFiles("./**/**/*.csproj").ToList().ForEach(project => {
         Information($"Building {project.ToString()}");
         DotNetBuild(project.ToString(),buildSettings);
     });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
       
       var testSettings = new DotNetTestSettings  {
                 Configuration = configuration,
                 NoBuild = true,
       };
        var coverageOutput = Directory(TEST_COVERAGE_OUTPUT_DIR);             
     
       GetFiles("./tests/**/*.csproj").ToList().ForEach(project => {
          Information($"Testing Project : {project.ToString()}");
            
          var codeCoverageOutputName = $"{project.GetFilenameWithoutExtension()}.cobertura.xml";
          var coverletSettings = new CoverletSettings {
              CollectCoverage = true,
               CoverletOutputFormat = CoverletOutputFormat.cobertura,
               CoverletOutputDirectory =  coverageOutput,
               CoverletOutputName =codeCoverageOutputName,
               ArgumentCustomization = args => args.Append($"--logger trx")
          };
                  
          Information($"Running Tests : { project.ToString()}");
          DotNetTest(project.ToString(), testSettings, coverletSettings );        
        });
         Information($"Directory Path : { coverageOutput.ToString()}");
                  
              var glob = new GlobPattern($"./{ coverageOutput}/*.cobertura.xml");
                 
              Information($"globpattern : { glob.ToString()}");
              var outputDirectory = Directory("./coverage/reports");
             
             Information($"output Directory : { outputDirectory}");
              var reportSettings = new ReportGeneratorSettings
              {
                 ArgumentCustomization = args => args.Append($"-reportTypes:HtmlInline_AzurePipelines_Dark;Cobertura")
              };
                 
              ReportGenerator(glob, outputDirectory, reportSettings);
                  
              /* if (BuildSystem.AzurePipelines.IsRunningOnAzurePipelines)
              {
                var coverageFile = $"coverage/reports/Cobertura.xml";
                var coverageData = new AzurePipelinesPublishCodeCoverageData
                {
                  CodeCoverageTool = AzurePipelinesCodeCoverageToolType.Cobertura,
                  SummaryFileLocation = coverageFile,
                  ReportDirectory = "coverage/reports"
                };
                Information($"Publishing Test Coverage : {coverageFile}");
                BuildSystem.AzurePipelines.Commands.PublishCodeCoverage(coverageData);
              } */
});

Task("Publish")
    .IsDependentOn("Test")
    .Does(() => {
       var outputDirectory = Directory("./artifacts");
       var settings = new DotNetPublishSettings
       {
          Configuration = configuration,
          OutputDirectory = outputDirectory
       };
       DotNetPublish("./src/Api/Api.csproj", settings);
});

Task("Default")
       .IsDependentOn("Clean")
       .IsDependentOn("Restore")
       .IsDependentOn("Build")
       .IsDependentOn("Test")
       .IsDependentOn("Publish");

RunTarget(target);