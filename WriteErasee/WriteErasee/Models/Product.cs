using System;
using System.Collections.Generic;

namespace WriteErasee.Models;

public partial class Product
{
    public string ArticleNumber { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int? IdUnit { get; set; }

    public int Cost { get; set; }

    public int? MaxDiscountAmount { get; set; }

    public int? IdManufacturer { get; set; }

    public int? IdSupplier { get; set; }

    public int? IdCategory { get; set; }

    public decimal CurrentDiscount { get; set; }

    public int? QuantityInStock { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public virtual Category? IdCategoryNavigation { get; set; }

    public virtual Manufacturer? IdManufacturerNavigation { get; set; }

    public virtual Supplier? IdSupplierNavigation { get; set; }

    public virtual Unit? IdUnitNavigation { get; set; }

    public virtual ICollection<OrdersProduct> OrdersProducts { get; set; } = new List<OrdersProduct>();
    public decimal FinalPrice => Cost * (1 - CurrentDiscount / 100);
    public bool HasDiscount => CurrentDiscount > 0;

}
