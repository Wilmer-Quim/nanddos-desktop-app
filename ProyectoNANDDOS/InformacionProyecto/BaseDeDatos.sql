CREATE DATABASE proyecto_nanddos_db
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE proyecto_nanddos_db;

CREATE TABLE usuarios (
    id INT AUTO_INCREMENT PRIMARY KEY,
    usuario VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(100) NOT NULL,
    fecha_creacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE clientes (
    id INT AUTO_INCREMENT PRIMARY KEY,
    codigo VARCHAR(20) NOT NULL UNIQUE,
    nombres VARCHAR(100) NOT NULL,
    apellidos VARCHAR(100) NOT NULL,
    email VARCHAR(120) NULL,
    telefono VARCHAR(30) NULL,
    fecha_creacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE estados (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(60) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE nomenclaturas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    prefijo VARCHAR(5) NOT NULL UNIQUE,
    descripcion VARCHAR(80) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE equipos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    codigo VARCHAR(20) NOT NULL UNIQUE,
    cliente_id INT NOT NULL,
    nomenclatura_id INT NOT NULL,
    estado_id INT NOT NULL,
    fecha_ingreso DATE NOT NULL,
    marca VARCHAR(80) NULL,
    modelo VARCHAR(80) NULL,
    serial VARCHAR(100) NULL,
    descripcion_problema TEXT NOT NULL,
    repuestos_necesarios TEXT NULL,
    fecha_creacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_equipos_clientes FOREIGN KEY (cliente_id) REFERENCES clientes(id),
    CONSTRAINT fk_equipos_nomenclaturas FOREIGN KEY (nomenclatura_id) REFERENCES nomenclaturas(id),
    CONSTRAINT fk_equipos_estados FOREIGN KEY (estado_id) REFERENCES estados(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE entregas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    codigo VARCHAR(20) NOT NULL UNIQUE,
    equipo_id INT NOT NULL,
    diagnostico TEXT NOT NULL,
    repuestos_usados TEXT NULL,
    costo_total DECIMAL(10,2) NOT NULL,
    fecha_entrega DATE NOT NULL,
    pdf_path VARCHAR(255) NULL,
    fecha_creacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_entregas_equipos FOREIGN KEY (equipo_id) REFERENCES equipos(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO estados (nombre) VALUES
    ('En diagnóstico'),
    ('En reparación'),
    ('Esperando repuesto'),
    ('Listo para entregar'),
    ('Entregado');

INSERT INTO nomenclaturas (prefijo, descripcion) VALUES
    ('LP', 'Laptop'),
    ('PC', 'Computadora de escritorio'),
    ('IM', 'Impresora');

INSERT INTO usuarios (usuario, password_hash) VALUES
    ('NANDDOS', '$2a$11$VztN8cLdsoUNxS142BIAdezanA9Sd5DmvG/Pf1L.3x9yopQ/C88zS');
