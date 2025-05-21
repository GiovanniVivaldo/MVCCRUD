CREATE DATABASE		
USE ExamenDesarrollador

CREATE TABLE Usuarios(
ID_Usuario INT PRIMARY KEY IDENTITY(1,1),
Nombre VARCHAR(50),
PrimerApellido VARCHAR(50),
SegundoApellido VARCHAR(50),
FechaRegistro DATE,
Email VARCHAR(50),
Contraseña VARCHAR(50)
)

CREATE TABLE Productos(
ID_Producto INT PRIMARY KEY IDENTITY(1,1),
Nombre_Producto VARCHAR(50),
Marca VARCHAR(50),
Precio FLOAT,
Cantidad VARCHAR(50),
Categoria VARCHAR(50)
)

CREATE PROCEDURE inicioSesion
@Email VARCHAR(50),
@Contraseña VARCHAR(50),
@Existe INT OUTPUT
AS 
BEGIN
	IF EXISTS (SELECT 1 FROM Usuarios where Email=@Email AND Contraseña=@Contraseña) 
	BEGIN
		SET @Existe=1;
	END
	ELSE
	BEGIN
		SET @Existe=0
	END
END

CREATE PROCEDURE Registrar
@Nombre VARCHAR(50),
@PrimerApellido VARCHAR(50),
@SegundoApellido VARCHAR(50),
@Email VARCHAR(50),
@Contraseña VARCHAR(50),
@Existe INT OUTPUT
AS 
BEGIN
	IF EXISTS (SELECT 1 FROM Usuarios where Email=@Email) 
	BEGIN
		SET @Existe=0;
	END

	ELSE
	BEGIN
		INSERT INTO Usuarios(Nombre,PrimerApellido,SegundoApellido, FechaRegistro, Email, Contraseña)  
		VALUES (@Nombre,@PrimerApellido,@SegundoApellido,GETDATE(),@Email,@Contraseña);
		
		SET @Existe=1;
	END
END

CREATE PROCEDURE actualizarUsuario
    @ID_Usuario INT,
    @Nombre VARCHAR(50),
    @PrimerApellido VARCHAR(50),
    @SegundoApellido VARCHAR(50),
    @Email VARCHAR(50),
    @Contraseña VARCHAR(50),
    @Resultado INT OUTPUT
AS
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM Usuarios WHERE ID_Usuario = @ID_Usuario
    )
    BEGIN
        SET @Resultado = 0;
        RETURN;
    END

    IF EXISTS (
        SELECT 1 FROM Usuarios WHERE Email = @Email AND ID_Usuario != @ID_Usuario
    )
    BEGIN
        SET @Resultado = 2;
        RETURN;
    END

    UPDATE Usuarios SET Nombre = @Nombre, PrimerApellido = @PrimerApellido, SegundoApellido = @SegundoApellido, Email = @Email, Contraseña = @Contraseña
    WHERE ID_Usuario = @ID_Usuario;

    SET @Resultado = 1;
END;

CREATE PROCEDURE eliminarUsuario
    @ID_Usuario INT,
    @Resultado INT OUTPUT
AS
BEGIN
    IF EXISTS (
        SELECT 1 FROM Usuarios WHERE ID_Usuario = @ID_Usuario
    )
    BEGIN
        DELETE FROM Usuarios WHERE ID_Usuario = @ID_Usuario;
		
		SET @Resultado = 1; 
    END
    ELSE
    BEGIN
        SET @Resultado = 0;
    END
END;

CREATE PROCEDURE buscarUsuario
    @ID_Usuario INT = NULL,
    @Email VARCHAR(50) = NULL
AS
BEGIN
    SELECT 
        ID_Usuario,
        Nombre,
        PrimerApellido,
        SegundoApellido,
        FechaRegistro,
        Email,
        Contraseña
    FROM Usuarios
    WHERE 
        (@ID_Usuario IS NOT NULL AND ID_Usuario = @ID_Usuario)
        OR
        (@Email IS NOT NULL AND Email = @Email);
END;



DECLARE @Existe INT;
EXEC Registrar
    @Nombre = 'Daniela',
    @PrimerApellido = 'Martinez',
    @SegundoApellido = 'Vivaldo',
    @Email = 'daniela@gmail.com',
    @Contraseña = 'contraseña123',
    @Existe = @Existe OUTPUT;
SELECT @Existe AS Resultado;

DECLARE @Existe INT;
EXEC inicioSesion 
	@Email='giovanni@gmail.com',
	@Contraseña='contraseña123',
	@Existe=@Existe OUTPUT
SELECT @Existe AS Resultado

select * from Usuarios


CREATE PROCEDURE VisualizarProductos
	@ID_Producto INT = NULL,
	@Nombre_Producto VARCHAR(50) = NULL,
	@Marca VARCHAR(50) = NULL,
	@Precio FLOAT = NULL,
	@Cantidad VARCHAR(50) = NULL,
	@Categoria VARCHAR(50) = NULL
AS BEGIN
	SELECT
		ID_Producto,
		Nombre_Producto,
		Marca,
		Precio,
		Cantidad,
		Categoria
	FROM
		Productos
	WHERE
		(@ID_Producto IS NULL AND @Nombre_Producto IS NULL AND @Marca IS NULL AND @Precio IS NULL AND @Cantidad IS NULL AND @Categoria IS NULL)
		OR
		(@ID_Producto IS NOT NULL AND ID_Producto = @ID_Producto)
		OR
		(@Nombre_Producto IS NOT NULL AND Nombre_Producto = @Nombre_Producto)
		OR
		(@Marca IS NOT NULL AND Marca = @Marca)
		OR
		(@Precio IS NOT NULL AND Precio = @Precio)
		OR
		(@Cantidad IS NOT NULL AND Cantidad = @Cantidad)
		OR
		(@Categoria IS NOT NULL AND Categoria = @Categoria)
		
END;

CREATE PROCEDURE RegistrarProducto
	@Nombre_Producto VARCHAR(50) = NULL,
	@Marca VARCHAR(50) = NULL,
	@Precio FLOAT = NULL,
	@Cantidad VARCHAR(50) = NULL,
	@Categoria VARCHAR(50) = NULL,
	@Existe INT OUTPUT
AS 
BEGIN
	IF EXISTS (SELECT 1 FROM Productos where Nombre_Producto=@Nombre_Producto) 
	BEGIN
		SET @Existe=0;
	END

	ELSE
	BEGIN
		INSERT INTO Productos(Nombre_Producto,Marca,Precio, Cantidad, Categoria)  
		VALUES (@Nombre_Producto,@Marca,@Precio,@Cantidad,@Categoria);
		
		SET @Existe=1;
	END
END

DECLARE @Existe INT;
EXEC RegistrarProducto
    @Nombre_Producto = 'IMPRESORA TONER',
    @Marca = 'BROTHER',
    @Precio = 5000.00,
    @Cantidad = '10',
    @Categoria = 'Impresoras',
    @Existe = @Existe OUTPUT;
SELECT @Existe AS Resultado;
 

ALTER PROCEDURE VisualizarProductos
	@ID_Producto INT = NULL,
	@Nombre_Producto VARCHAR(50) = NULL,
	@Marca VARCHAR(50) = NULL,
	@Precio FLOAT = NULL,
	@Cantidad VARCHAR(50) = NULL,
	@Categoria VARCHAR(50) = NULL
AS BEGIN
	SELECT
		ID_Producto,
		Nombre_Producto,
		Marca,
		Precio,
		Cantidad,
		Categoria
	FROM
		Productos
	WHERE
		(@ID_Producto IS NULL OR ID_Producto = @ID_Producto) AND
		(@Nombre_Producto IS NULL OR Nombre_Producto = @Nombre_Producto) AND
		(@Marca IS NULL OR Marca = @Marca) AND
		(@Precio IS NULL OR Precio = @Precio) AND
		(@Cantidad IS NULL OR Cantidad = @Cantidad) AND
		(@Categoria IS NULL OR Categoria = @Categoria)
END


exec VisualizarProductos null,null,null,null,null,null
exec VisualizarProductos 1,'','','','',''


CREATE TABLE Ventas (
    ID_Venta INT PRIMARY KEY IDENTITY(1,1),
    ID_Usuario INT,
    ID_Producto INT,
    CantidadVendida INT,
    FechaVenta DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ID_Usuario) REFERENCES Usuarios(ID_Usuario),
    FOREIGN KEY (ID_Producto) REFERENCES Productos(ID_Producto)
);

CREATE PROCEDURE RegistrarVenta
    @ID_Usuario INT,
    @ID_Producto INT,
    @CantidadVendida INT,
    @Resultado INT OUTPUT
AS
BEGIN
    DECLARE @CantidadActual INT;

    SELECT @CantidadActual = CAST(Cantidad AS INT)
    FROM Productos
    WHERE ID_Producto = @ID_Producto;

    IF @CantidadActual IS NULL OR @CantidadActual < @CantidadVendida
    BEGIN
        SET @Resultado = 0; -- Stock insuficiente
        RETURN;
    END

    -- Registrar la venta
    INSERT INTO Ventas(ID_Usuario, ID_Producto, CantidadVendida)
    VALUES (@ID_Usuario, @ID_Producto, @CantidadVendida);

    -- Actualizar cantidad de producto
    UPDATE Productos
    SET Cantidad = CAST(Cantidad AS INT) - @CantidadVendida
    WHERE ID_Producto = @ID_Producto;

    SET @Resultado = 1; -- Éxito
END

select*from Ventas