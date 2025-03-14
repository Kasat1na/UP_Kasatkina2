using System;
using System.Collections.Generic;

namespace WriteErasee.Models;

public partial class OrdersProduct
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string ProductArticleNumber { get; set; } = null!;

    public int Count { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product ProductArticleNumberNavigation { get; set; } = null!;
}
