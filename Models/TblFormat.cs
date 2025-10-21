using System;
using System.Collections.Generic;

namespace InventorySys.Models;

public partial class TblFormat
{
    public int FormatId { get; set; }

    public string FormatName { get; set; } = null!;

    public decimal? FormatSizeCm { get; set; }

    public bool FormatActive { get; set; }

    public virtual ICollection<TblMaterial> TblMaterials { get; set; } = new List<TblMaterial>();
}
