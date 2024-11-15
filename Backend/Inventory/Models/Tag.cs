namespace Inventory.Models;

public class Tag
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public List<Item> Items { get; set; } = [];
}