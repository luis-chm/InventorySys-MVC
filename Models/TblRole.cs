using System;
using System.Collections.Generic;

namespace InventorySys.Models;

public partial class TblRole
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public bool RoleActive { get; set; }

    public virtual ICollection<TblUser> TblUsers { get; set; } = new List<TblUser>();
}
