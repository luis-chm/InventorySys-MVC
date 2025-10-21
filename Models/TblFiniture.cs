using System;
using System.Collections.Generic;

namespace InventorySys.Models;

public partial class TblFiniture
{
    public int FinitureId { get; set; }

    public string FinitureCode { get; set; } = null!;

    public string FinitureName { get; set; } = null!;

    public bool FinitureActive { get; set; }

    public virtual ICollection<TblMaterial> TblMaterials { get; set; } = new List<TblMaterial>();
}
