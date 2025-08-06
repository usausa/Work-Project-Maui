namespace WorkSample.PageModels;
using CommunityToolkit.Mvvm.Input;

using WorkSample.Models;

public interface IProjectTaskPageModel
{
    IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
    bool IsBusy { get; }
}