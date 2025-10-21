using System;
using System.Collections.Generic;

namespace InventorySys.Models;

public partial class TblMaterialTransaction
{
    public int MaterialTransactionId { get; set; }

    public string MaterialTransactionType { get; set; } = null!;

    public decimal MaterialTransactionQuantity { get; set; }

    public DateTime MaterialTransactionDate { get; set; }

    public int? UserId { get; set; }

    public int? MaterialId { get; set; }

    public virtual TblMaterial? Material { get; set; }

    public virtual ICollection<TblDetailMovement> TblDetailMovements { get; set; } = new List<TblDetailMovement>();

    public virtual TblUser? User { get; set; }
}
