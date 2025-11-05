-- =====================================================
-- TRIGGERS DE AUDITORÍA PARA TODAS LAS TABLAS
-- Sistema de Inventario de Porcelanatos
-- =====================================================

-- =====================================================
-- TABLA: tbl_Users (Usuarios)
-- =====================================================

CREATE TRIGGER trg_Users_Insert ON tbl_Users AFTER INSERT AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, UserID, OldValues, NewValues)
    SELECT 
        'tbl_Users',
        'INSERT',
        i.UserID,
        i.UserID,
        'N/A',
        CONCAT('Usuario:', i.UserName, '; Email:', i.UserEmail, '; Rol:', i.RoleID)
    FROM inserted i;
END;

CREATE TRIGGER trg_Users_Update ON tbl_Users AFTER UPDATE AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, UserID, OldValues, NewValues)
    SELECT 
        'tbl_Users',
        'UPDATE',
        i.UserID,
        i.UserID,
        CONCAT('Usuario:', d.UserName, '; Email:', d.UserEmail, '; Activo:', d.UserActive),
        CONCAT('Usuario:', i.UserName, '; Email:', i.UserEmail, '; Activo:', i.UserActive)
    FROM inserted i
    INNER JOIN deleted d ON i.UserID = d.UserID;
END;

CREATE TRIGGER trg_Users_Delete ON tbl_Users AFTER DELETE AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, OldValues)
    SELECT 
        'tbl_Users',
        'DELETE',
        d.UserID,
        CONCAT('Usuario eliminado:', d.UserName, '; Email:', d.UserEmail)
    FROM deleted d;
END;

-- =====================================================
-- TABLA: tbl_Roles (Roles)
-- =====================================================

CREATE TRIGGER trg_Roles_Insert ON tbl_Roles AFTER INSERT AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, OldValues, NewValues)
    SELECT 'tbl_Roles', 'INSERT', i.RoleID, 'N/A', i.RoleName FROM inserted i;
END;

CREATE TRIGGER trg_Roles_Update ON tbl_Roles AFTER UPDATE AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, OldValues, NewValues)
    SELECT 'tbl_Roles', 'UPDATE', i.RoleID, d.RoleName, i.RoleName 
    FROM inserted i 
    INNER JOIN deleted d ON i.RoleID = d.RoleID;
END;

CREATE TRIGGER trg_Roles_Delete ON tbl_Roles AFTER DELETE AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, OldValues)
    SELECT 'tbl_Roles', 'DELETE', d.RoleID, d.RoleName FROM deleted d;
END;

-- =====================================================
-- TABLA: tbl_Collections (Colecciones)
-- =====================================================

CREATE TRIGGER trg_Collections_Insert ON tbl_Collections AFTER INSERT AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, OldValues, NewValues)
    SELECT 'tbl_Collections', 'INSERT', i.CollectionID, 'N/A', i.CollectionName FROM inserted i;
END;

CREATE TRIGGER trg_Collections_Update ON tbl_Collections AFTER UPDATE AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, OldValues, NewValues)
    SELECT 'tbl_Collections', 'UPDATE', i.CollectionID, d.CollectionName, i.CollectionName 
    FROM inserted i 
    INNER JOIN deleted d ON i.CollectionID = d.CollectionID;
END;

CREATE TRIGGER trg_Collections_Delete ON tbl_Collections AFTER DELETE AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, OldValues)
    SELECT 'tbl_Collections', 'DELETE', d.CollectionID, d.CollectionName FROM deleted d;
END;

-- =====================================================
-- TABLA: tbl_Formats (Formatos)
-- =====================================================

CREATE TRIGGER trg_Formats_Insert ON tbl_Formats AFTER INSERT AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, OldValues, NewValues)
    SELECT 'tbl_Formats', 'INSERT', i.FormatID, 'N/A', CONCAT('Formato:', i.FormatName, '; Tamaño:', i.FormatSizeCM, 'cm') FROM inserted i;
END;

CREATE TRIGGER trg_Formats_Update ON tbl_Formats AFTER UPDATE AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, OldValues, NewValues)
    SELECT 'tbl_Formats', 'UPDATE', i.FormatID, CONCAT(d.FormatName, '; ', d.FormatSizeCM), CONCAT(i.FormatName, '; ', i.FormatSizeCM) 
    FROM inserted i 
    INNER JOIN deleted d ON i.FormatID = d.FormatID;
END;

CREATE TRIGGER trg_Formats_Delete ON tbl_Formats AFTER DELETE AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, OldValues)
    SELECT 'tbl_Formats', 'DELETE', d.FormatID, d.FormatName FROM deleted d;
END;

-- =====================================================
-- TABLA: tbl_Sites (Sitios)
-- =====================================================

CREATE TRIGGER trg_Sites_Insert ON tbl_Sites AFTER INSERT AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, OldValues, NewValues)
    SELECT 'tbl_Sites', 'INSERT', i.SiteID, 'N/A', i.SiteName FROM inserted i;
END;

CREATE TRIGGER trg_Sites_Update ON tbl_Sites AFTER UPDATE AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, OldValues, NewValues)
    SELECT 'tbl_Sites', 'UPDATE', i.SiteID, d.SiteName, i.SiteName 
    FROM inserted i 
    INNER JOIN deleted d ON i.SiteID = d.SiteID;
END;

CREATE TRIGGER trg_Sites_Delete ON tbl_Sites AFTER DELETE AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, OldValues)
    SELECT 'tbl_Sites', 'DELETE', d.SiteID, d.SiteName FROM deleted d;
END;

-- =====================================================
-- TABLA: tbl_Finitures (Acabados)
-- =====================================================

CREATE TRIGGER trg_Finitures_Insert ON tbl_Finitures AFTER INSERT AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, OldValues, NewValues)
    SELECT 'tbl_Finitures', 'INSERT', i.FinitureID, 'N/A', CONCAT('Código:', i.FinitureCode, '; ', i.FinitureName) FROM inserted i;
END;

CREATE TRIGGER trg_Finitures_Update ON tbl_Finitures AFTER UPDATE AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, OldValues, NewValues)
    SELECT 'tbl_Finitures', 'UPDATE', i.FinitureID, CONCAT(d.FinitureCode, '; ', d.FinitureName), CONCAT(i.FinitureCode, '; ', i.FinitureName) 
    FROM inserted i 
    INNER JOIN deleted d ON i.FinitureID = d.FinitureID;
END;

CREATE TRIGGER trg_Finitures_Delete ON tbl_Finitures AFTER DELETE AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, OldValues)
    SELECT 'tbl_Finitures', 'DELETE', d.FinitureID, CONCAT(d.FinitureCode, '; ', d.FinitureName) FROM deleted d;
END;

-- =====================================================
-- TABLA: tbl_MaterialTransactions (Transacciones de Materiales)
-- =====================================================

CREATE TRIGGER trg_MaterialTransactions_Insert ON tbl_MaterialTransactions AFTER INSERT AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, UserID, OldValues, NewValues)
    SELECT 'tbl_MaterialTransactions', 'INSERT', i.MaterialTransactionID, i.UserID, 'N/A', CONCAT('Tipo:', i.MaterialTransactionType, '; Cantidad:', i.MaterialTransactionQuantity) FROM inserted i;
END;

CREATE TRIGGER trg_MaterialTransactions_Update ON tbl_MaterialTransactions AFTER UPDATE AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, UserID, OldValues, NewValues)
    SELECT 'tbl_MaterialTransactions', 'UPDATE', i.MaterialTransactionID, i.UserID, CONCAT('Cantidad:', d.MaterialTransactionQuantity), CONCAT('Cantidad:', i.MaterialTransactionQuantity) 
    FROM inserted i 
    INNER JOIN deleted d ON i.MaterialTransactionID = d.MaterialTransactionID;
END;

CREATE TRIGGER trg_MaterialTransactions_Delete ON tbl_MaterialTransactions AFTER DELETE AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, OldValues)
    SELECT 'tbl_MaterialTransactions', 'DELETE', d.MaterialTransactionID, CONCAT('Tipo:', d.MaterialTransactionType, '; Cantidad:', d.MaterialTransactionQuantity) FROM deleted d;
END;

-- =====================================================
-- VISTA PARA CONSULTAR TODOS LOS LOGS
-- =====================================================

CREATE VIEW vw_AllAuditLogs AS
SELECT 
    AuditLogID AS 'ID Log',
    TableName AS 'Tabla',
    Operation AS 'Operación',
    RecordID AS 'ID Registro',
    CONVERT(VARCHAR(10), OperationDateTime, 103) AS 'Fecha',
    CONVERT(VARCHAR(8), OperationDateTime, 108) AS 'Hora',
    u.UserName AS 'Usuario',
    OldValues AS 'Valor Anterior',
    NewValues AS 'Valor Nuevo'
FROM tbl_AuditLog tbl
LEFT JOIN tbl_Users u ON tbl.UserID = u.UserID
ORDER BY OperationDateTime DESC;

-- =====================================================
-- CONSULTAS DE EJEMPLO PARA VER LOS LOGS
-- =====================================================

-- 1. Ver TODOS los logs
-- SELECT * FROM vw_AllAuditLogs;

-- 2. Ver logs de una tabla específica
-- SELECT * FROM vw_AllAuditLogs WHERE Tabla = 'tbl_Users';

-- 3. Ver solo eliminaciones
-- SELECT * FROM vw_AllAuditLogs WHERE Operación = 'DELETE';

-- 4. Ver logs de un usuario específico
-- SELECT * FROM vw_AllAuditLogs WHERE Usuario = 'admin_proyecto';

-- 5. Ver logs de los últimos 7 días
-- SELECT * FROM vw_AllAuditLogs WHERE Fecha >= CAST(GETDATE() - 7 AS DATE);

-- 6. Ver cambios en tbl_Materials
-- SELECT * FROM vw_AllAuditLogs WHERE Tabla = 'tbl_Materials';