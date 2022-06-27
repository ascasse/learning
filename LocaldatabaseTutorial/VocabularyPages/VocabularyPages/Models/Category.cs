using System;
using System.Collections.Generic;

namespace VocabularyPages.Models
{
    public class Category
    {
        public Category() { }

        public int Id { get; set; }

        public string Name { get; set; }

        public string LastUse { get; set; }

        public List<Word> Words { get; set; }

        public Category(Vocabulary.Model.Category category)
        {
            Id = category.Id;
            Name = category.Name;
            LastUse = category.LastUse != DateTime.MinValue ? category.LastUse.ToString("dd/MM/yyyy") : "Not used";
            Words = new List<Word>();
            if (category.Words != null)
            {
                foreach (Vocabulary.Model.Word w in category.Words)
                {
                    Words.Add(new Word()
                    {
                        Id = w.Id,
                        Text = w.Text,
                        LastUse = w.LastUse,
                        Views = w.Views
                    });
                }
            }
        }
    }
}
