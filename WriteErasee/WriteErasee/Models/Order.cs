using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WriteErasee.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateOnly? DateOrder { get; set; }

    public DateOnly? DateDelivery { get; set; }

    public int? IdPickUpPoint { get; set; }

    public int? IdClient { get; set; }

    public int? Code { get; set; }

    public int IdStatus { get; set; }

    public virtual User? IdClientNavigation { get; set; }

    public virtual PickUpPoint? IdPickUpPointNavigation { get; set; }

    public virtual Status IdStatusNavigation { get; set; } = null!;

    public virtual ICollection<OrdersProduct> OrdersProducts { get; set; } = new List<OrdersProduct>();

    [NotMapped]
	public string FIO { get; set; }

	[NotMapped]
	public string Color { get; set; }

	[NotMapped]
	public double Cost { get; set; }

	[NotMapped]
	public double Discount { get; set; }
}
