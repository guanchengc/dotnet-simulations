namespace MauiApp1
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);

            // Manipulate Window object
            window.Height = 1200d;
            window.Width = 1200d;

            return window;
        }
    }
}