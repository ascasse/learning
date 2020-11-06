using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleVocabulary.Model
{
    [Table("Words")]
    public class Word
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime LastUse { get; set; }
        public int Views { get; set; }
        [ForeignKey(typeof(Category))]
        public int CategoryId { get; set; }
    }
}
