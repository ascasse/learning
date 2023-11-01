namespace MaterialAPI.Model
{
    public class Item
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public int Views { get; set; }
        public DateTime? LastUse { get; set; }
    }
}
