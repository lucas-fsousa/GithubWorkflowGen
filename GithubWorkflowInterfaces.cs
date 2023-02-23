using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubWorkflowGen {

  public interface IWorkflow {
    public IWorkflowName WithName(string name);
  }

  public interface IWorkflowName {
    public IWorkflowTrigger StartOn(string actionName);
  }

  public interface IWorkflowTrigger {
    public IWorkflowBranch InBranchs(params string[] branchsNames);
  }

  public interface IWorkflowBranch {
    public IWorkflowJobs AddJob(string jobName);
  }

  public interface IWorkflowJobs {
    public IWorkflowStep RunsOn(string platformName);
  }

  public interface IWorkflowStep {
    public IWorkflowStep AddStep(string name, string value);
    public IWorkflowStep AddStepRun(string name, string value);
    public IWorkflowStep AddStep(string uses);
    public IWorkflowStep With(string name, string value);
    public IWorkflowJobs AddNewJob(string jobName, params string[] needJobs);
    public IWorkflowBuild Build(string fileName = "main.yml");
  }

  public interface IWorkflowBuild { }
}
