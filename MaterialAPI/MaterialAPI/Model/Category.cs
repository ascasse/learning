namespace MaterialAPI.Model
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastUse { get; set; }
        public bool Completed { get; set; }
        public int Type { get; set; }
        public virtual ICollection<Item> Items { get; set; }
        public Category() { }
        public Category(Category copy)
        {
            Id = copy.Id;
            Name = copy.Name;
            LastUse = copy.LastUse;
            Completed = copy.Completed;
            Type = copy.Type;
            Items = copy.Items;
        }
    }
}
