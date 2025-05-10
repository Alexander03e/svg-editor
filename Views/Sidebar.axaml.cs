using Avalonia.Controls;
using SvgEditorApp.Models;
using SvgEditorApp.ViewModel;

namespace SvgEditorApp.Views
{
    public partial class Sidebar : UserControl
    {
        public Sidebar()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}