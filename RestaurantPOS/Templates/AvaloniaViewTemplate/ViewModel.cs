using CommunityToolkit.Mvvm.ComponentModel;

namespace TemplateNamespace.ViewModels;

public partial class TemplateNameViewModel : ObservableObject
{
    public string Title { get; set; } = "This is TemplateNameViewModel";
}
