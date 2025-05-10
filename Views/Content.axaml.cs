using System;
using Avalonia.Controls;

namespace SvgEditorApp.Views
{
    public partial class Content : UserControl
    {
        public Content()
        {
            
            InitializeComponent();
            MyButton.Click += (sender, e) =>
            {
                Console.WriteLine(sender);
                Console.WriteLine(e);
                MyButton.Content = "clicked";
            };
        }
    }
}