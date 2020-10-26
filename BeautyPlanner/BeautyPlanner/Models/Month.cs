using System.Collections.Generic;
using SQLite;

namespace BeautyPlanner.Models
{
    public class Month 
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public int Year { get; set; }
        [Ignore]
        public IEnumerable<Day> Days { get; set; }
    }
}
