﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace Learning.Model
{
    [Table("Categories")]
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastUse { get; set; }
        public int Type { get; set; }
        [OneToMany]
        public List<Word> Words { get; set; }
    }
}
