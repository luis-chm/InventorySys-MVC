using System;
using System.Collections.Generic;

namespace InventorySys.Models;

public partial class TblAuditLog
{
    public int AuditLogId { get; set; }

    public string TableName { get; set; } = null!;

    public string Operation { get; set; } = null!;

    public int RecordId { get; set; }

    public int? UserId { get; set; }

    public DateTime OperationDateTime { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public virtual TblUser? User { get; set; }
}
