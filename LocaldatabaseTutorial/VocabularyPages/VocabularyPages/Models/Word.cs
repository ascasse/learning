using System;

namespace VocabularyPages.Models
{
    public class Word
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime LastUse { get; set; }
        public int Views { get; set; }
    }
}
