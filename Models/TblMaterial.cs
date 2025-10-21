using System;
using System.Collections.Generic;

namespace InventorySys.Models;

public partial class TblMaterial
{
    public int MaterialId { get; set; }

    public string MaterialCode { get; set; } = null!;

    public string MaterialDescription { get; set; } = null!;

    public int? CollectionId { get; set; }

    public int? FinitureId { get; set; }

    public int? FormatId { get; set; }

    public int? SiteId { get; set; }

    public string? MaterialImg { get; set; }

    public DateOnly MaterialReceivedDate { get; set; }

    public decimal MaterialStock { get; set; }

    public int? UserId { get; set; }

    public DateTime RecordInsertDateTime { get; set; }

    public virtual TblCollection? Collection { get; set; }

    public virtual TblFiniture? Finiture { get; set; }

    public virtual TblFormat? Format { get; set; }

    public virtual TblSite? Site { get; set; }

    public virtual ICollection<TblMaterialTransaction> TblMaterialTransactions { get; set; } = new List<TblMaterialTransaction>();

    public virtual TblUser? User { get; set; }
}
