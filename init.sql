-- Script de Inicialización de la Base de Datos para el Sistema de Gestión de Inventarios

CREATE TABLE IF NOT EXISTS Productos (
    Id SERIAL PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(255) NOT NULL,
    Categoria VARCHAR(50) NOT NULL,
    Imagen VARCHAR(500) NOT NULL,
    Precio DECIMAL(18, 2) NOT NULL CHECK (Precio >= 0),
    Stock INT NOT NULL CHECK (Stock >= 0)
);

CREATE TABLE IF NOT EXISTS Transacciones (
    Id SERIAL PRIMARY KEY,
    Fecha TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    TipoTransaccion VARCHAR(20) NOT NULL CHECK (TipoTransaccion IN ('COMPRA', 'VENTA')),
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL CHECK (Cantidad > 0),
    PrecioUnitario DECIMAL(18, 2) NOT NULL CHECK (PrecioUnitario >= 0),
    PrecioTotal DECIMAL(18, 2) NOT NULL CHECK (PrecioTotal >= 0),
    Detalle VARCHAR(255) NOT NULL,
    CONSTRAINT FK_Transacciones_Productos FOREIGN KEY (ProductoId) REFERENCES Productos(Id) ON DELETE RESTRICT
);

-- Datos iniciales de prueba (Seed Data)
INSERT INTO Productos (Nombre, Descripcion, Categoria, Imagen, Precio, Stock) VALUES
('Laptop Pro 15', 'Computadora portátil de alto rendimiento i7 16GB RAM', 'Tecnología', 'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=500', 1250.00, 15),
('Mouse Inalámbrico RGB', 'Mouse ergonómico con sensor óptico de alta precisión', 'Accesorios', 'https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=500', 25.50, 50),
('Teclado Mecánico Switch Blue', 'Teclado retroiluminado RGB para programación', 'Accesorios', 'https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=500', 65.00, 30),
('Monitor Gaming 27 IPS', 'Monitor QHD 165Hz tiempo de respuesta 1ms', 'Tecnología', 'https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?w=500', 320.00, 10),
('Silla Ergonómica Ejecutiva', 'Silla de oficina soporte lumbar ajustable', 'Mobiliario', 'https://nomadaware.com.ec/wp-content/uploads/NomadaWare_silla_corsair_tc100_relaxed_negro.webp', 180.00, 8);

INSERT INTO Transacciones (Fecha, TipoTransaccion, ProductoId, Cantidad, PrecioUnitario, PrecioTotal, Detalle) VALUES
(NOW() - INTERVAL '5 days', 'COMPRA', 1, 20, 1200.00, 24000.00, 'Lote inicial de reposición de Laptops'),
(NOW() - INTERVAL '4 days', 'VENTA', 1, 5, 1250.00, 6250.00, 'Venta a cliente corporativo'),
(NOW() - INTERVAL '3 days', 'COMPRA', 2, 60, 20.00, 1200.00, 'Compra a proveedor de accesorios'),
(NOW() - INTERVAL '2 days', 'VENTA', 2, 10, 25.50, 255.00, 'Ventas individuales en tienda online'),
(NOW() - INTERVAL '1 days', 'COMPRA', 3, 30, 50.00, 1500.00, 'Inventario inicial de teclados');
