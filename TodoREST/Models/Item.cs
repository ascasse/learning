namespace TodoREST.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public int Views { get; set; }
        public DateTime? LastUse { get; set; }

        public Item() {}

        public Item(Item original)
        {
            Id = original.Id;
            Text = original.Text;
            Image = original.Image;
            Views = original.Views;
            LastUse = original.LastUse;
        }
    }
}
