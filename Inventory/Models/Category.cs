namespace Inventory.Models;

public class Category : AuditableEntity
{
    public new long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public long? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = [];
    public ICollection<Item> Items { get; set; } = [];
}
