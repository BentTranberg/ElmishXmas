using System;
using System.Windows;

namespace DemoAsync
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
            DemoAsync.Program.main(MainWindow);
        }
    }
}
