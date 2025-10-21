using System;
using System.Collections.Generic;

namespace InventorySys.Models;

public partial class TblSite
{
    public int SiteId { get; set; }

    public string SiteName { get; set; } = null!;

    public string SiteLocation { get; set; } = null!;

    public bool SiteActive { get; set; }

    public virtual ICollection<TblMaterial> TblMaterials { get; set; } = new List<TblMaterial>();
}
