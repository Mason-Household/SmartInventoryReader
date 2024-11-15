namespace Inventory.Models;

public class Tag
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<Item> Items { get; set; } = [];
}