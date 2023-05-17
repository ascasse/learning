using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Learning.Model
{
    [Table("Items")]
    public class Item
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public DateTime LastUse { get; set; }
        public int Views { get; set; }

        [ForeignKey(typeof(Category))]
        public int CategoryId { get; set; }
    }
}
