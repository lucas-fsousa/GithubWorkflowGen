using PublicUtility.Extension;
using System.Text;

namespace GithubWorkflowGen {
  public class App {
    static void Main(string[] args) {
      var workflow =
        GithubWorkflowBuilder.Configure()
        .WithName(".NET Workflow")
        .StartOn("push")
        .InBranchs("main", "dev", "master")
        .AddJob("Dotnet Build")
        .RunsOn("ubuntu-latest")
        .AddStep("actions/checkout@v3")
        .AddStep("Setup .NET", "actions/setup-dotnet@v1")
        .With("dotnet-version", "7.0.x")
        .AddStepRun("Restore dependencies", "dotnet restore")
        .AddStepRun("Build", "dotnet build")
        .AddStepRun("Test", "dotnet test --no-build --verbosity normal")

        .AddNewJob("Dotnet Deploy", "BilyDin", "bilyHein")
        .RunsOn("ubuntu-latest")
        .AddStepRun("Release package", "dotnet pack --configuration Release")
        .AddStepRun("Publish", "dotnet nuget push projectPath")
        .With("HueBr", "huehueheueh")
        .Build();
    }
  }

  public class GithubWorkflowBuilder:
    IWorkflow, IWorkflowName,
    IWorkflowTrigger, IWorkflowBranch,
    IWorkflowJobs, IWorkflowStep,
    IWorkflowBuild {

    private readonly StringBuilder tempFile;
    private bool stepsFlag = false;

    private GithubWorkflowBuilder() {
      tempFile = new StringBuilder();
    }

    private static string Tab(int countTab) {
      string tabs = string.Empty;
      for(int i = 0; i < countTab; i++) {
        tabs += '\t';
      }

      return tabs;
    }

    public static GithubWorkflowBuilder Configure() => new();

    public IWorkflowName WithName(string name) {
      name = name.ValueOrExeption().Trim();
      tempFile.AppendLine($"name: {name}");

      return this;
    }

    public IWorkflowTrigger StartOn(string actionName) {
      actionName = actionName.ValueOrExeption().Trim();

      tempFile.AppendLine($"on:");
      tempFile.Append(Tab(1));
      tempFile.AppendLine($"{actionName}:");

      return this;
    }

    public IWorkflowBranch InBranchs(params string[] branchsNames) {
      tempFile.AppendLine($"{Tab(2)}branches:");

      for(int i = 0; i < branchsNames.Length; i++) {
        branchsNames[i] = branchsNames[i].ValueOrExeption().Trim();
        tempFile.AppendLine($"{Tab(3)}- {branchsNames[i]}");
      }

      return this;
    }

    public IWorkflowJobs AddJob(string jobName) {
      stepsFlag = false;
      jobName = jobName.ValueOrExeption().Trim();

      tempFile.AppendLine("jobs:");
      tempFile.Append(Tab(1));
      tempFile.AppendLine($"{jobName}:");

      return this;
    }
    public IWorkflowStep RunsOn(string platformName) {
      platformName = platformName.ValueOrExeption().Trim();

      tempFile.Append(Tab(2));
      tempFile.AppendLine($"runs-on: {platformName}");

      return this;
    }

    public IWorkflowStep AddStep(string uses) {
      uses = uses.ValueOrExeption().Trim();

      if(!stepsFlag) {
        tempFile.Append(Tab(2));
        tempFile.AppendLine($"steps:");
        stepsFlag = true;
      }

      tempFile.Append(Tab(3));
      tempFile.AppendLine($"- uses: {uses}");

      return this;
    }

    public IWorkflowStep AddStep(string name, string value) {
      name = name.ValueOrExeption().Trim();
      value = value.ValueOrExeption().Trim();

      if(!stepsFlag) {
        tempFile.Append(Tab(2));
        tempFile.AppendLine($"steps:");
        stepsFlag = true;
      }

      tempFile.Append(Tab(3));
      tempFile.AppendLine($"- name: {name}");

      tempFile.Append(Tab(4));
      tempFile.AppendLine($"uses: {value}");

      return this;
    }

    public IWorkflowStep AddStepRun(string name, string value) {
      name = name.ValueOrExeption().Trim();
      value = value.ValueOrExeption().Trim();

      if(!stepsFlag) {
        tempFile.Append(Tab(2));
        tempFile.AppendLine($"steps:");
        stepsFlag = true;
      }

      tempFile.Append(Tab(3));
      tempFile.AppendLine($"- name: {name}");

      tempFile.Append(Tab(4));
      tempFile.AppendLine($"run: {value}");

      return this;
    }

    public IWorkflowStep With(string name, string value) {
      name = name.ValueOrExeption().Trim();
      value = value.ValueOrExeption().Trim();

      tempFile.Append(Tab(4));
      tempFile.AppendLine("with: ");

      tempFile.Append(Tab(5));
      tempFile.AppendLine($"{name}: {value}");

      return this;
    }

    public IWorkflowJobs AddNewJob(string jobName, params string[] needJobs) {
      stepsFlag = false;
      jobName = jobName.ValueOrExeption().Trim();

      tempFile.Append(Tab(1));
      tempFile.AppendLine($"{jobName}:");

      if(needJobs.IsFilled()) {
        tempFile.Append(Tab(2));
        tempFile.AppendLine("needs: ");

        foreach(var job in needJobs) {
          tempFile.Append(Tab(3));
          tempFile.AppendLine($"- {job}");
        }

      }

      return this;
    }

    public IWorkflowBuild Build(string fileName) {
      if(!fileName.ValueOrExeption().EndsWith(".yaml"))
        fileName += ".yaml";

      File.WriteAllText(fileName, tempFile.ToString());
      return this;
    }
  }
}