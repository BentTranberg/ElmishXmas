using System;
using System.Windows;

namespace ListView
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
            ListView.Program.main(MainWindow);
        }
    }
}
