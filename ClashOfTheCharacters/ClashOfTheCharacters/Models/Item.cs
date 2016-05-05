using ClashOfTheCharacters.Helpers;

namespace ClashOfTheCharacters.Models
{
    public class Item
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public ItemType ItemType { get; set; }

        public Rarity Rarity { get; set; }
    }
}