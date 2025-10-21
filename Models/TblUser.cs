using System;
using System.Collections.Generic;

namespace InventorySys.Models;

public partial class TblUser
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string UserEmail { get; set; } = null!;

    public string UserEncryptedPassword { get; set; } = null!;

    public int? RoleId { get; set; }

    public bool UserActive { get; set; }

    public virtual TblRole? Role { get; set; }

    public virtual ICollection<TblAuditLog> TblAuditLogs { get; set; } = new List<TblAuditLog>();

    public virtual ICollection<TblMaterialTransaction> TblMaterialTransactions { get; set; } = new List<TblMaterialTransaction>();

    public virtual ICollection<TblMaterial> TblMaterials { get; set; } = new List<TblMaterial>();
}
