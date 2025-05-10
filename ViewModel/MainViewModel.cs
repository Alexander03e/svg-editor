using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;
using SvgEditorApp.Models;

namespace SvgEditorApp.ViewModel;
public class MainViewModel : ReactiveObject
{
    private EMode mode = EMode.VIEW;
    public ObservableCollection<string> SvgFiles { get; } = new();
    public ReactiveCommand<Unit, Unit> LoadSvgCommand { get; }

    public EMode Mode
    {
        get => mode;
        set => this.RaiseAndSetIfChanged(ref mode, value);
    }

    public bool IsEditMode
    {
        get => Mode == EMode.EDIT;
        set => Mode = value ? EMode.EDIT : EMode.VIEW;
    }
}