using System;
using System.Collections.Generic;
using System.IO;

namespace BeautyPlanner
{
    public static class Constants
    {
        public const string DatabaseFilename = "BeautyPlannerSQLite.db3";

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
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilename);
            }
        }

        public static Dictionary<string, int> MonthsDictionary =  new Dictionary<string, int>
        {
            {"Январь", 31},
            {"Февраль", 28},
            {"Март", 31},
            {"Апрель", 30},
            {"Май", 31},
            {"Июнь", 30},
            {"Июль", 31},
            {"Август", 31},
            {"Сентябрь", 30},
            {"Октябрь", 31},
            {"Ноябрь", 30},
            {"Декабрь", 31}
        };

        public static Dictionary<int, string> MonthsNamesDictionary = new Dictionary<int, string>
        {
            { 1, "Январь"},
            { 2, "Февраль"},
            { 3, "Март"},
            { 4, "Апрель"},
            { 5, "Май"},
            { 6, "Июнь"},
            { 7, "Июль"},
            { 8, "Август"},
            { 9, "Сентябрь"},
            { 10, "Октябрь"},
            { 11, "Ноябрь"},
            { 12, "Декабрь"}
        };

        public static Dictionary<string, string> DaysDictionary = new Dictionary<string, string>
        {
            {"Monday", "Понедельник"},
            {"Tuesday", "Вторник"},
            {"Wednesday", "Среда"},
            {"Thursday", "Четверг"},
            {"Friday", "Пятница"},
            {"Saturday", "Суббота"},
            {"Sunday", "Воскресенье"}
        };

        public const string Manicure = "Маникюр"; //nothing by default
        public const string ManicurePedicure = "Маникюр + Педикюр"; //м + п
        public const string FullPedicure = "Полный педикюр"; //пп
        public const string Pedicure = "Педикюр"; //п

        public const string FreeDay = "Выходной";
        public const string ImportantFreeDay = "Выходной!";
    }
}
