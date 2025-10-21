using System;
using System.Collections.Generic;

namespace InventorySys.Models;

public partial class TblCollection
{
    public int CollectionId { get; set; }

    public string CollectionName { get; set; } = null!;

    public string CollectionEffect { get; set; } = null!;

    public bool CollectionActive { get; set; }

    public virtual ICollection<TblMaterial> TblMaterials { get; set; } = new List<TblMaterial>();
}
