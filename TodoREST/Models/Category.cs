namespace TodoREST.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? LastUse { get; set; }
        public bool Completed { get; set; }
        public int Type { get; set; }
        public virtual ICollection<Item> Items { get; set; } = [];
        public Category() { }
        public Category(Category original)
        {
            Name = original.Name;
            LastUse = original.LastUse;
            Completed = original.Completed;
            Type = original.Type;
            Items = new List<Item>(original.Items); // Copies references. Elements are unique
        }
    }
}
