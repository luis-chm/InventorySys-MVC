using System;
using System.Collections.Generic;

namespace InventorySys.Models;

public partial class VwCriticalStockMaterial
{
    public int MaterialId { get; set; }

    public string MaterialCode { get; set; } = null!;

    public string MaterialDescription { get; set; } = null!;

    public decimal MaterialStock { get; set; }

    public string CollectionName { get; set; } = null!;

    public string SiteName { get; set; } = null!;

    public string StockStatus { get; set; } = null!;
}
