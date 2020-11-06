using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Plugin.Permissions;
//using Plugin.Permissions.Abstractions;
//using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;


namespace LocalDatabaseTutorial
{
    public partial class App : Application
    {
        static Database database;
        //static string basePath = "/storage/emulated/0/Material/";
        //static string materialPath = "/storage/emulated/0/Material/";
        //static string databaseFile = "people.db3";


        public static Database Database
        {
            get
            {
                if (database == null)
                {
                    //if (!Directory.Exists(materialPath))
                    //{
                    //    Directory.CreateDirectory(materialPath);
                    //}
                    //string dbPath = Path.Combine(materialPath, "people.db3");
                    
                    database = new Database(Constants.DatabasePath);
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
          
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        //private static async Task InitDatabase()
        //{
        //    //var storageEnabled = await CheckAndRequestPermissionAsync();
        //    //if (!storageEnabled)
        //    //{
        //    //    basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        //    //}
        //    materialPath = Path.Combine(basePath, "Material");
        //    if (!Directory.Exists(materialPath))
        //    {
        //        Directory.CreateDirectory(materialPath);
        //    }
        //    string dbPath = Path.Combine(materialPath, databaseFile);
        //    if (Database == null)
        //    {
        //        database = new Database(dbPath);
        //    }

        //}



    }
}
