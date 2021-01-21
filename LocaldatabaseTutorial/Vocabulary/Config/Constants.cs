using System.IO;

namespace Vocabulary.Config
{
    public static class Constants
    {
        public const string DatabaseFilename = "Vocabulary.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath
        {
            get
            {
                var basePath = "/storage/emulated/0/Material/";
                return Path.Combine(basePath, DatabaseFilename);
            }
        }
    }
}
