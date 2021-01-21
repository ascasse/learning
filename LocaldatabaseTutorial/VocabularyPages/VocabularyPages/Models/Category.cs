using System;
using System.Collections.Generic;

namespace VocabularyPages.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastUse { get; set; }
        public List<Word> Words { get; set; }
    }
}
