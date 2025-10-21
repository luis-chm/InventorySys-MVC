namespace InventorySys.Helpers;

public static class RolePermissionHelper
{
    // Enum para módulos
    public enum SystemModule
    {
        Inicio,
        Dashboard,
        Users,
        Roles,
        Collections,
        Formats,
        Sites,
        Finitures,
        Materials,
        MaterialTransactions,
        DetailMovements,
        ReporteMaterials,
        ReporteTransactions,
        Catalogo,
        Stock,
        Reportes,
        Administrador
    }

    // Enum para acciones
    public enum Permission
    {
        View,
        Create,
        Edit,
        Delete,
        Export
    }

    // Verificar si tiene acceso al módulo
    public static bool CanAccessModule(string roleName, SystemModule module)
    {
        if (string.IsNullOrEmpty(roleName)) return false;

        switch (roleName)
        {
            case "Super Administrador":
                return true;

            case "Administrador":
                return module != SystemModule.Roles;

            case "Soporte Tecnico":
                return module == SystemModule.Inicio ||
                       module == SystemModule.Dashboard ||
                       module == SystemModule.Users ||
                       module == SystemModule.Administrador;

            case "Supervisor de Bodega":
                return module == SystemModule.Inicio ||
                       module == SystemModule.Dashboard ||
                       module == SystemModule.Materials ||
                       module == SystemModule.MaterialTransactions ||
                       module == SystemModule.DetailMovements ||
                       module == SystemModule.Stock ||
                       module == SystemModule.Catalogo ||
                       module == SystemModule.Collections ||
                       module == SystemModule.Formats ||
                       module == SystemModule.Finitures ||
                       module == SystemModule.Sites ||
                       module == SystemModule.ReporteMaterials ||
                       module == SystemModule.ReporteTransactions ||
                       module == SystemModule.Reportes;

            case "Operario de Bodega":
                return module == SystemModule.Inicio ||
                       module == SystemModule.Dashboard ||
                       module == SystemModule.Materials ||
                       module == SystemModule.MaterialTransactions ||
                       module == SystemModule.DetailMovements ||
                       module == SystemModule.Stock;

            case "Gerencia":
                return module == SystemModule.Inicio ||
                       module == SystemModule.Dashboard ||
                       module == SystemModule.ReporteMaterials ||
                       module == SystemModule.ReporteTransactions ||
                       module == SystemModule.Reportes;

            default:
                return false;
        }
    }

    // Verificar permiso específico
    public static bool HasPermission(string roleName, SystemModule module, Permission permission)
    {
        if (string.IsNullOrEmpty(roleName)) return false;

        // Super Admin tiene todos los permisos
        if (roleName == "Super Administrador") return true;

        // ADMINISTRADOR
        if (roleName == "Administrador")
        {
            // Usuarios: Create, Edit, Delete, View
            if (module == SystemModule.Users)
                return permission == Permission.Create ||
                       permission == Permission.Edit ||
                       permission == Permission.Delete ||
                       permission == Permission.View;

            // Catálogo: Solo lectura
            if (module == SystemModule.Collections ||
                module == SystemModule.Formats ||
                module == SystemModule.Finitures ||
                module == SystemModule.Sites)
                return permission == Permission.View;

            // Stock: Solo lectura
            if (module == SystemModule.Materials ||
                module == SystemModule.MaterialTransactions ||
                module == SystemModule.DetailMovements)
                return permission == Permission.View;

            // Reportes: View y Export
            if (module == SystemModule.ReporteMaterials ||
                module == SystemModule.ReporteTransactions)
                return permission == Permission.View || permission == Permission.Export;

            return false;
        }

        // SOPORTE TÉCNICO
        if (roleName == "Soporte Tecnico")
        {
            // Usuarios: View y Edit
            if (module == SystemModule.Users)
                return permission == Permission.View || permission == Permission.Edit;

            return false;
        }

        // SUPERVISOR DE BODEGA
        if (roleName == "Supervisor de Bodega")
        {
            // Catálogo: Create, Edit, View
            if (module == SystemModule.Collections ||
                module == SystemModule.Formats ||
                module == SystemModule.Finitures ||
                module == SystemModule.Sites)
                return permission == Permission.Create ||
                       permission == Permission.Edit ||
                       permission == Permission.View;

            // Materiales: Create, Edit, View
            if (module == SystemModule.Materials)
                return permission == Permission.Create ||
                       permission == Permission.Edit ||
                       permission == Permission.View;

            // Transacciones: Create, Edit, View
            if (module == SystemModule.MaterialTransactions)
                return permission == Permission.Create ||
                       permission == Permission.Edit ||
                       permission == Permission.View;

            // Detalles: Solo View
            if (module == SystemModule.DetailMovements)
                return permission == Permission.View;

            // Reportes: View y Export
            if (module == SystemModule.ReporteMaterials ||
                module == SystemModule.ReporteTransactions)
                return permission == Permission.View || permission == Permission.Export;

            return false;
        }

        // OPERARIO DE BODEGA
        if (roleName == "Operario de Bodega")
        {
            // Materiales: Solo View
            if (module == SystemModule.Materials)
                return permission == Permission.View;

            // Transacciones: Create, Edit, View
            if (module == SystemModule.MaterialTransactions)
                return permission == Permission.Create ||
                       permission == Permission.Edit ||
                       permission == Permission.View;

            // Detalles: Solo View
            if (module == SystemModule.DetailMovements)
                return permission == Permission.View;

            return false;
        }

        // GERENCIA
        if (roleName == "Gerencia")
        {
            // Reportes: View y Export
            if (module == SystemModule.ReporteMaterials ||
                module == SystemModule.ReporteTransactions)
                return permission == Permission.View || permission == Permission.Export;

            return false;
        }

        return false;
    }

    // Métodos auxiliares
    public static bool CanCreate(string roleName, SystemModule module)
    {
        return HasPermission(roleName, module, Permission.Create);
    }

    public static bool CanEdit(string roleName, SystemModule module)
    {
        return HasPermission(roleName, module, Permission.Edit);
    }

    public static bool CanDelete(string roleName, SystemModule module)
    {
        return HasPermission(roleName, module, Permission.Delete);
    }

    public static bool CanView(string roleName, SystemModule module)
    {
        return HasPermission(roleName, module, Permission.View);
    }

    public static bool CanExport(string roleName, SystemModule module)
    {
        return HasPermission(roleName, module, Permission.Export);
    }

    // Obtener lista de roles disponibles
    public static List<string> GetAvailableRoles()
    {
        return new List<string>
        {
            "Super Administrador",
            "Administrador",
            "Soporte Técnico",
            "Supervisor de Bodega",
            "Operario de Bodega",
            "Gerencia"
        };
    }
}
