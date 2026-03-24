-- Crear la base de datos.
CREATE DATABASE BeachSA;
GO 

-- Usar la base de datos.
USE BeachSA;
GO

-- Tabla Roles: almacena los perfiles o roles de usuario.
CREATE TABLE Roles (
IdRol INT IDENTITY(1,1) PRIMARY KEY, 
NombreRol VARCHAR(50) NOT NULL   
);
-- Insertar roles iniciales. 
INSERT INTO Roles (NombreRol) VALUES ('Admin'), ('Cliente');

-- Tabla TiposCedula: tipos de identificación para clientes.
CREATE TABLE TiposCedula (
IdTipoCedula INT IDENTITY(1,1) PRIMARY KEY, 
Descripcion VARCHAR(50) NOT NULL                   
);
-- Insertar tipos de cédula comunes.
INSERT INTO TiposCedula (Descripcion) VALUES ('Nacional'), ('Residencia'), ('Pasaporte');

-- Tabla Clientes: información de los clientes del hotel.
CREATE TABLE Clientes (
IdCliente INT IDENTITY(1,1) PRIMARY KEY, 
IdTipoCedula INT NOT NULL,                  
Cedula VARCHAR(20) NOT NULL,                  
NombreCompleto VARCHAR(100) NOT NULL,                  
Telefono VARCHAR(20) NULL,                      
CorreoElectronico VARCHAR(150) NOT NULL,                  
Direccion VARCHAR(300) NULL                       
);
-- Clave foránea: tipo de cédula.
ALTER TABLE Clientes
ADD CONSTRAINT FK_Clientes_TiposCedula 
FOREIGN KEY (IdTipoCedula) REFERENCES TiposCedula(IdTipoCedula);

-- Tabla Usuarios: credenciales de acceso de usuarios.
CREATE TABLE Usuarios (
IdUsuario INT IDENTITY(1,1) PRIMARY KEY, 
Username VARCHAR(50) NOT NULL,                  
Contrasenna VARCHAR(100) NOT NULL,                  
IdRol INT NOT NULL,                  
IdCliente INT NULL                      
);
-- Claves foráneas: Rol y Cliente. 
ALTER TABLE Usuarios
ADD CONSTRAINT FK_Usuarios_Roles 
FOREIGN KEY (IdRol) REFERENCES Roles(IdRol);
ALTER TABLE Usuarios
ADD CONSTRAINT FK_Usuarios_Clientes 
FOREIGN KEY (IdCliente) REFERENCES Clientes(IdCliente);

-- Insertar un usuario administrador. 
INSERT INTO Usuarios (Username, Contrasenna, IdRol)
VALUES ('admin', 'Admin123', 1); 

-- Tabla MetodosPago: métodos de pago disponibles.
CREATE TABLE MetodosPago (
IdMetodoPago INT IDENTITY(1,1) PRIMARY KEY, 
NombreMetodo VARCHAR(50) NOT NULL,                  
AplicaDescuento BIT NOT NULL,                  
RequiereDetalleCheque BIT NOT NULL                   
);
-- Insertar métodos de pago iniciales. 
INSERT INTO MetodosPago (NombreMetodo, AplicaDescuento, RequiereDetalleCheque) 
VALUES ('Efectivo', 1, 0), ('Tarjeta', 0, 0), ('Cheque', 0, 1);     

-- Tabla Paquetes: define los paquetes vacacionales ofrecidos por el hotel.
CREATE TABLE Paquetes (
IdPaquete INT IDENTITY(1,1) PRIMARY KEY, 
NombrePaquete VARCHAR(100) NOT NULL,                
Destino VARCHAR(100) NOT NULL,                 
FechaInicio DATE NULL,                   
FechaFin DATE NULL,                      
PrecioPorNoche DECIMAL(10,2) NOT NULL,                 
PorcentajePrima DECIMAL(5,2) NOT NULL,                  
Mensualidades INT NOT NULL                  
);
-- Insertar paquetes predeterminados.  
INSERT INTO Paquetes (NombrePaquete, Destino, PrecioPorNoche, PorcentajePrima, Mensualidades)
VALUES ('Todo Incluido', 'Destino General', 450.00, 0.45, 24), ('Alimentación', 'Destino General', 275.00, 0.35, 18), ('Hospedaje', N'Destino General', 210.00, 0.15, 12);  

-- Tabla Reservaciones: registra las reservaciones realizadas por clientes.
CREATE TABLE Reservaciones (
IdReservacion INT IDENTITY(1,1) PRIMARY KEY, 
IdCliente INT NOT NULL,                  
IdPaquete INT NOT NULL,                  
CantidadNoches INT NOT NULL,                  
IdMetodoPago INT NOT NULL,                 
Prima DECIMAL(10,2) NOT NULL,                  
Mensualidades DECIMAL(10,2) NOT NULL,                  
Descuento DECIMAL(10,2) NOT NULL,                  
Impuestos DECIMAL(10,2) NOT NULL,                  
MontoTotal DECIMAL(10,2) NOT NULL,                  
TipoCambio DECIMAL(10,4) NOT NULL,                  
ValorColones DECIMAL(10,2) NOT NULL,                  
ValorDolares DECIMAL(10,2) NOT NULL,                  
PDFEnviado BIT NOT NULL DEFAULT 0         
);
-- Claves foráneas: Cliente, Paquete, Método de pago.
ALTER TABLE Reservaciones
ADD CONSTRAINT FK_Reservaciones_Clientes 
FOREIGN KEY (IdCliente) REFERENCES Clientes(IdCliente);
ALTER TABLE Reservaciones
ADD CONSTRAINT FK_Reservaciones_Paquetes 
FOREIGN KEY (IdPaquete) REFERENCES Paquetes(IdPaquete);
ALTER TABLE Reservaciones
ADD CONSTRAINT FK_Reservaciones_MetodosPago 
FOREIGN KEY (IdMetodoPago) REFERENCES MetodosPago(IdMetodoPago);

-- Tabla DetallesPago: detalles adicionales del pago, en caso de ser con cheque.
CREATE TABLE DetallesPago (
IdDetallePago INT IDENTITY(1,1) PRIMARY KEY, 
IdReservacion INT NOT NULL,                  
NumeroCheque VARCHAR(20) NULL,                      
Banco VARCHAR(100) NULL                       
);
-- Clave foránea: Reservación (solo se registra si el método de pago es cheque). 
ALTER TABLE DetallesPago
ADD CONSTRAINT FK_DetallesPago_Reservaciones 
FOREIGN KEY (IdReservacion) REFERENCES Reservaciones(IdReservacion)
ON DELETE CASCADE;

-- Tabla Auditoria: registra las transacciones o acciones efectuadas por los usuarios.
CREATE TABLE Auditorias (
    IdAuditoria INT IDENTITY(1,1) PRIMARY KEY,
    Usuario VARCHAR(100),
    Rol VARCHAR(50),
    Fecha DATETIME,
    TablaAfectada VARCHAR(100),
    Accion VARCHAR(50),
    IdRegistro INT,
    Descripcion VARCHAR(255)
);
