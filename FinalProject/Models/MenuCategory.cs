using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class MenuCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }
	public int? ParentCategoryId { get; set; }
	public virtual MenuCategory? ParentCategory { get; set; }
	public virtual ICollection<MenuCategory> SubCategories { get; set; } = new List<MenuCategory>();

	public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
