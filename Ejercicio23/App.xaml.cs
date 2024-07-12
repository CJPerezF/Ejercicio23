namespace Ejercicio23
{
    public partial class App : Application
    {
        private static Controllers.DBProc _instance;

        public static Controllers.DBProc Instance
        {
            get
            {
                if (_instance is null)
                {
                    InitializeDBProc();
                }

                return _instance;
            }
        }

        private static void InitializeDBProc()
        {
            string dbName = "AudiosDB.db3";
            string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string dbFullPath = Path.Combine(dbPath, dbName);
            _instance = new Controllers.DBProc(dbFullPath);
        }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}
