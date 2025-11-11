CREATE DATABASE InventorySYS
GO
USE InventorySYS
GO

-- Tabla tbl_Roles
CREATE TABLE tbl_Roles (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName VARCHAR(50) NOT NULL,
    RoleActive BIT NOT NULL
);

-- Tabla tbl_Users
CREATE TABLE tbl_Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY, --1 CAMPO ID AUTOINCREMENTAL
    UserName VARCHAR(50) NOT NULL,
    UserEmail VARCHAR(500) NOT NULL,
    UserEncryptedPassword VARCHAR(100) NOT NULL,
    UserActive BIT NOT NULL, --1 CAMPO DE ESTADO
    RoleID INT,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleID) REFERENCES tbl_Roles(RoleID),
    CONSTRAINT CHK_UserEmail_Format CHECK (UserEmail LIKE '%@%.%') -- 1 CONSTRAINT DE CHECK
);

-- Tabla tbl_Collections
CREATE TABLE tbl_Collections (
    CollectionID INT IDENTITY(1,1) PRIMARY KEY,
    CollectionName VARCHAR(100) NOT NULL,
    CollectionEffect VARCHAR(200) NOT NULL,
    CollectionActive BIT NOT NULL
);

-- Tabla tbl_Formats
CREATE TABLE tbl_Formats (
    FormatID INT IDENTITY(1,1) PRIMARY KEY,
    FormatName VARCHAR(100) NOT NULL,
    FormatSizeCM DECIMAL(8, 2),
    FormatActive BIT NOT NULL
);

-- Tabla tbl_Sites
CREATE TABLE tbl_Sites (
    SiteID INT IDENTITY(1,1) PRIMARY KEY,
    SiteName VARCHAR(100) NOT NULL,
    SiteLocation VARCHAR(200) NOT NULL,
    SiteActive BIT NOT NULL
);

-- Tabla tbl_Finitures
CREATE TABLE tbl_Finitures (
    FinitureID INT IDENTITY(1,1) PRIMARY KEY,
    FinitureCode VARCHAR(50) NOT NULL,
    FinitureName VARCHAR(100) NOT NULL,
    FinitureActive BIT NOT NULL
);

-- Tabla tbl_Materials
CREATE TABLE tbl_Materials (
    MaterialID INT IDENTITY(1,1) PRIMARY KEY,
    MaterialCode VARCHAR(50) NOT NULL,
    MaterialDescription VARCHAR(200) NOT NULL,
    CollectionID INT,
    FinitureID INT,
    FormatID INT,
    SiteID INT,
    MaterialIMG VARCHAR(MAX) NULL,
    MaterialReceivedDate DATE NOT NULL,
    MaterialStock DECIMAL(18, 2) NOT NULL,
    UserID INT,
    RecordInsertDateTime DATETIME NOT NULL DEFAULT GETDATE(), -- 1 CAMPO DE FECHA Y HORA DE INSERCIÓN DE REGISTRO
    CONSTRAINT FK_Materials_Collections FOREIGN KEY (CollectionID) REFERENCES tbl_Collections(CollectionID),
    CONSTRAINT FK_Materials_Finitures FOREIGN KEY (FinitureID) REFERENCES tbl_Finitures(FinitureID),
    CONSTRAINT FK_Materials_Formats FOREIGN KEY (FormatID) REFERENCES tbl_Formats(FormatID),
    CONSTRAINT FK_Materials_Sites FOREIGN KEY (SiteID) REFERENCES tbl_Sites(SiteID),
    CONSTRAINT FK_Materials_Users FOREIGN KEY (UserID) REFERENCES tbl_Users(UserID)
);

-- Tabla tbl_MaterialTransactions
CREATE TABLE tbl_MaterialTransactions (
    MaterialTransactionID INT IDENTITY(1,1) PRIMARY KEY,
    MaterialTransactionType VARCHAR(50) NOT NULL,
    MaterialTransactionQuantity DECIMAL(18, 2) NOT NULL,
    MaterialTransactionDate DATETIME NOT NULL,
    UserID INT,
    MaterialID INT NULL,
    CONSTRAINT FK_MaterialTransactions_Users FOREIGN KEY (UserID) REFERENCES tbl_Users(UserID),
    CONSTRAINT FK_MaterialTransactions_Materials FOREIGN KEY (MaterialID) REFERENCES tbl_Materials(MaterialID) -- NUEVO
);

-- Tabla tbl_DetailMovements
CREATE TABLE tbl_DetailMovements (
    DetailMovID INT PRIMARY KEY IDENTITY(1,1),
    MaterialTransactionID INT,
    DetInitBalance DECIMAL(18, 2),
    DetCantEntry DECIMAL(18, 2),
    DetCantExit DECIMAL(18, 2),
    DetCurrentBalance DECIMAL(18, 2),
    CONSTRAINT FK_DetailMovements_MaterialTransactions FOREIGN KEY (MaterialTransactionID) REFERENCES tbl_MaterialTransactions(MaterialTransactionID)
);

-- tabla para auditoria (1 TABLA DE BITACORA)

CREATE TABLE tbl_AuditLog (
    AuditLogID INT IDENTITY(1,1) PRIMARY KEY,
    TableName VARCHAR(50) NOT NULL,
    Operation VARCHAR(10) NOT NULL,
    RecordID INT NOT NULL,
    UserID INT NULL,
    OperationDateTime DATETIME NOT NULL DEFAULT GETDATE(),
    OldValues VARCHAR(MAX) NULL,
    NewValues VARCHAR(MAX) NULL,
    CONSTRAINT FK_AuditLog_Users FOREIGN KEY (UserID) REFERENCES tbl_Users(UserID)
);
----------------------------------------------------------------------------------------------------------------

-- Crear función para encriptar contraseñas con SHA256
CREATE FUNCTION dbo.EncryptPassword(@password NVARCHAR(100))
RETURNS NVARCHAR(100)
AS
BEGIN
    DECLARE @hashedPassword NVARCHAR(100)
    
    -- Usar HASHBYTES con SHA2_256
    SET @hashedPassword = CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', @password), 2)
    
    RETURN @hashedPassword
END
-------------------------------------------------------------------------------------------------------------------------

-- Vista tabla materiales (1 VISTA)

-- Crear nueva vista optimizada para gestión de stock crítico
CREATE VIEW vw_CriticalStockMaterials AS
SELECT 
    m.MaterialID,
    m.MaterialCode,
    m.MaterialDescription,
    m.MaterialStock,
    c.CollectionName,
    s.SiteName,
    CASE 
        WHEN m.MaterialStock = 0 THEN 'SIN STOCK'
        WHEN m.MaterialStock < 50 THEN 'CRITICO'
        WHEN m.MaterialStock < 75 THEN 'BAJO'
        ELSE 'NORMAL'
    END AS StockStatus
FROM tbl_Materials m
INNER JOIN tbl_Collections c ON m.CollectionID = c.CollectionID
INNER JOIN tbl_Sites s ON m.SiteID = s.SiteID;

-- EJEMPLOS DE USO DE LA VISTA:

-- 1. Ver todos los materiales con stock crítico
SELECT * FROM vw_CriticalStockMaterials 
WHERE StockStatus = 'CRITICO';

-- 2. Materiales sin stock (emergencia)
SELECT * FROM vw_CriticalStockMaterials 
WHERE StockStatus = 'SIN STOCK';

-- 2. Materiales Bajo
SELECT * FROM vw_CriticalStockMaterials 
WHERE StockStatus = 'BAJO';

-------------------------------------------------------------------------------------------------------------------------

-- TRIGGERS para tabla auditoria

-- INSERT (1 TRIGGER INSERT)
CREATE TRIGGER trg_Materials_Insert
ON tbl_Materials                    -- En la tabla de materiales
AFTER INSERT                        -- DESPUÉS de insertar
AS
BEGIN
    SET NOCOUNT ON;
    PRINT 'TRIGGER INSERT ejecutado - Nuevo material agregado';

    -- Automáticamente guarda en la bitácora lo que se insertó
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, UserID, OldValues, NewValues)
    SELECT 
        'tbl_Materials',                                    -- Tabla afectada
        'INSERT',                                           -- Operación
        i.MaterialID,                                       -- ID del material insertado
        i.UserID,                                          -- Usuario que lo insertó
		'N/A',                                             -- No hay registros anteriores
        CONCAT('Código:', i.MaterialCode, 
               '; Descripción:', i.MaterialDescription, 
               '; Stock:', i.MaterialStock)                 -- Datos nuevos
    FROM inserted i;  -- 'inserted' es una tabla especial que contiene los datos que se insertaron
END;

-- UPDATE (1 TRIGGER UPDATE)
CREATE TRIGGER trg_Materials_Update
ON tbl_Materials
AFTER UPDATE                        -- DESPUÉS de actualizar
AS
BEGIN
    SET NOCOUNT ON;
    PRINT 'TRIGGER UPDATE ejecutado - Material modificado';

    -- Guarda tanto los valores anteriores como los nuevos
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, UserID, OldValues, NewValues)
    SELECT 
        'tbl_Materials',
        'UPDATE',
        i.MaterialID,
        i.UserID,
        CONCAT('Stock anterior:', d.MaterialStock, 
               '; Descripción anterior:', d.MaterialDescription),    -- Datos viejos
        CONCAT('Stock nuevo:', i.MaterialStock, 
               '; Descripción nueva:', i.MaterialDescription)       -- Datos nuevos
    FROM inserted i                                         -- Datos nuevos
    INNER JOIN deleted d ON i.MaterialID = d.MaterialID;   -- Datos anteriores
END;

--DELETE (1 TRIGGER DELETE)
CREATE TRIGGER trg_Materials_Delete
ON tbl_Materials
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    PRINT 'TRIGGER DELETE ejecutado - Material eliminado';
    
    -- Guarda qué se borró, INCLUYENDO el UserID
    INSERT INTO tbl_AuditLog (TableName, Operation, RecordID, UserID, OldValues, NewValues)
    SELECT 
        'tbl_Materials',
        'DELETE',
        d.MaterialID,                                       -- ID del material borrado
        d.UserID,                                          -- ← AGREGAR ESTA LÍNEA
        CONCAT('Código borrado:', d.MaterialCode, 
               '; Descripción borrada:', d.MaterialDescription, 
               '; Stock que tenía:', d.MaterialStock),      -- Datos que se perdieron
        'N/A'                                              -- No hay valores nuevos en DELETE
    FROM deleted d;  -- 'deleted' contiene los datos que se borraron
END;

-- Verificar movimientos Triggers 
SELECT * FROM tbl_AuditLog WHERE Operation = 'INSERT';

SELECT * FROM tbl_AuditLog
ORDER BY OperationDateTime DESC
-------------------------------------------------------------------------------------------------------------------------
-- PRUEBA DE CONSTRAINT

-- Esto DEBE FALLAR no lleva @ ni punto
INSERT INTO tbl_Users (UserName, UserEmail, UserEncryptedPassword, UserActive, RoleID)
VALUES ('Test User', 'emailinvalido', 'password123', 1, 1);

-- Esto DEBE FUNCIONAR lleva @ y punto
INSERT INTO tbl_Users (UserName, UserEmail, UserEncryptedPassword, UserActive, RoleID)
VALUES ('Test User', 'usuario@gmail.com', 'password123', 1, 1);