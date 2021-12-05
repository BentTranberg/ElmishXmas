using System;
using System.Windows;

namespace ListBox
{
    public partial class App : Application
    {
        public App()
        {
            this.Activated += StartElmish;
        }

        private void StartElmish(object sender, EventArgs e)
        {
            this.Activated -= StartElmish;
            ListBox.Program.main(MainWindow);
        }
    }
}
