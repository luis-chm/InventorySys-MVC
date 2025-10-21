using System;
using System.Collections.Generic;

namespace InventorySys.Models;

public partial class TblDetailMovement
{
    public int DetailMovId { get; set; }

    public int? MaterialTransactionId { get; set; }

    public decimal? DetInitBalance { get; set; }

    public decimal? DetCantEntry { get; set; }

    public decimal? DetCantExit { get; set; }

    public decimal? DetCurrentBalance { get; set; }

    public virtual TblMaterialTransaction? MaterialTransaction { get; set; }
}
