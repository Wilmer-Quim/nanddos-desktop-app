# Contexto Maestro del Proyecto NANDDOS

Fecha de extraccion: 21/07/2026 15:57

Este documento resume el estado tecnico actual de la solucion NANDDOS para entregar contexto maestro al Arquitecto de Software. La extraccion se realizo sobre la solucion activa ubicada en `Sistema_NANDDOS`.

## 1. Identificacion de la solucion

| Elemento | Valor |
|---|---|
| Carpeta raiz | `Sistema_NANDDOS` |
| Solucion | `ProyectoNANDDOS.slnx` |
| Proyecto principal | `ProyectoNANDDOS/ProyectoNANDDOS.csproj` |
| Namespace principal | `ProyectoNANDDOS` |
| Tipo de salida | `WinExe` |
| Framework | `.NET 10 Windows` (`net10.0-windows`) |
| Tecnologia de UI | Windows Forms (`UseWindowsForms=true`) |
| Base de datos | MySQL `proyecto_nanddos_db` |
| Carpeta de comprobantes | `ProyectoNANDDOS/PDF` |
| Carpeta de documentacion | `ProyectoNANDDOS/InformacionProyecto` |

Contenido actual del archivo de solucion:

```xml
<Solution>
  <Project Path="ProyectoNANDDOS/ProyectoNANDDOS.csproj" />
</Solution>
```

## 2. Arbol de directorios

Se excluyen carpetas generadas automaticamente (`bin`, `obj`, `.vs`) para mostrar la estructura mantenible del proyecto.

```text
Sistema_NANDDOS/
??? IMGNANDDOS/
?   ??? Captura de pantalla 2025-08-21 140242.png
??? ProyectoNANDDOS/
?   ??? InformacionProyecto/
?   ?   ??? BaseDeDatos.sql
?   ?   ??? DocumentacionBaseDatos.txt
?   ?   ??? DocumentacionTecnica.pdf
?   ?   ??? ExplicacionCompletaSistema.pdf
?   ?   ??? ExplicacionCompletaSistema.txt
?   ?   ??? INSTRUCCIONES_BASE_DATOS.txt
?   ??? PDF/
?   ?   ??? ENT-0005.pdf
?   ??? BienvenidaForm.cs
?   ??? ClientesForm.cs
?   ??? ConexionDB.cs
?   ??? DetalleEquipoForm.cs
?   ??? EntregaForm.cs
?   ??? GeneradorCodigo.cs
?   ??? ImagenEmpresa.cs
?   ??? ListaEquiposForm.cs
?   ??? LoginForm.cs
?   ??? LoginForm.resx
?   ??? MenuPrincipalForm.cs
?   ??? Program.cs
?   ??? ProyectoNANDDOS.csproj
?   ??? ProyectoNANDDOS.csproj.user
?   ??? RegistrarEquipoForm.cs
??? Scripts/
?   ??? CrearBaseDatos.sql
??? INSTRUCCIONES_BASE_DATOS.txt
??? ProyectoNANDDOS.slnx
```

## 3. Separacion logica de capas MVC

El proyecto no esta fisicamente separado en carpetas `Models`, `Views` y `Controllers`; actualmente usa una estructura plana dentro de `ProyectoNANDDOS`. Sin embargo, la responsabilidad del codigo puede interpretarse en una separacion MVC logica:

| Capa MVC logica | Elementos actuales | Responsabilidad |
|---|---|---|
| Modelo | Tablas MySQL `usuarios`, `clientes`, `estados`, `nomenclaturas`, `equipos`, `entregas`; clases internas `DatosComprobante`, `EstadoItem`, `NomenclaturaItem` | Representar datos persistentes, datos de UI y objetos auxiliares para combos/PDF. |
| Vista | `LoginForm`, `BienvenidaForm`, `MenuPrincipalForm`, `RegistrarEquipoForm`, `ListaEquiposForm`, `ClientesForm`, `EntregaForm`, `DetalleEquipoForm` | Pantallas WinForms, controles, layout, mensajes y captura de datos. |
| Controlador | Eventos `Click`, `SelectedIndexChanged`, `CellClick`, `CellDoubleClick`, `Resize`; metodos como `BuscarClientes`, `GuardarEquipo`, `CargarEquipos`, `GenerarEntrega` | Coordinar interaccion del usuario, validaciones, consultas, transacciones y navegacion. |
| Data Access | `ConexionDB` y consultas `MySqlCommand` distribuidas en formularios | Abrir conexiones MySQL y ejecutar SELECT/INSERT/UPDATE/DELETE. |
| Servicios/Helpers | `GeneradorCodigo`, `ImagenEmpresa`, generacion PDF dentro de `EntregaForm` | Generacion de codigos, carga de logo y comprobantes PDF. |

Observacion arquitectonica: para una evolucion futura convendria mover consultas SQL a una capa `Data`/`Repositories`, DTOs a `Models` y logica de negocio a `Services`. Hoy la solucion funciona como WinForms monolitico organizado por formularios.

## 4. Dependencias y paquetes NuGet

### Paquetes directos declarados en `.csproj`

| Paquete | Version | Uso principal |
|---|---:|---|
| `BCrypt.Net-Next` | `4.1.0` | Verificacion de contrasenas hasheadas en `LoginForm`. |
| `iTextSharp.LGPLv2.Core` | `3.7.12` | Generacion de comprobantes PDF en `EntregaForm`. |
| `MySql.Data` | `9.7.0` | Conexion y comandos MySQL con `MySql.Data.MySqlClient`. |

### Paquetes transitivos resueltos

| Paquete | Version | Origen funcional |
|---|---:|---|
| `BouncyCastle.Cryptography` | `2.6.2` | Criptografia requerida por librerias de PDF/MySQL. |
| `Google.Protobuf` | `3.32.0` | Soporte interno requerido por `MySql.Data`. |
| `K4os.Compression.LZ4` | `1.3.8` | Soporte interno requerido por `MySql.Data`. |
| `K4os.Compression.LZ4.Streams` | `1.3.8` | Soporte interno requerido por `MySql.Data`. |
| `K4os.Hash.xxHash` | `1.0.8` | Soporte interno requerido por `MySql.Data`. |
| `SkiaSharp` | `3.119.1` | Soporte grafico usado por dependencias de PDF/imagenes. |
| `SkiaSharp.NativeAssets.macOS` | `3.119.1` | Soporte grafico usado por dependencias de PDF/imagenes. |
| `SkiaSharp.NativeAssets.Win32` | `3.119.1` | Soporte grafico usado por dependencias de PDF/imagenes. |
| `ZstdSharp.Port` | `0.8.6` | Soporte interno requerido por `MySql.Data`. |

## 5. Script DDL actual de la base de datos MySQL

Archivo fuente: `Scripts/CrearBaseDatos.sql`

```sql
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
```

### Tablas y relaciones detectadas

| Tabla | Proposito | Relaciones principales |
|---|---|---|
| `usuarios` | Credenciales del sistema; almacena usuario y `password_hash` BCrypt. | Sin claves foraneas. |
| `clientes` | Datos de clientes y codigo visible `CLI-0001`. | Relacion 1:N con `equipos`. |
| `estados` | Catalogo de estados del ciclo de servicio. | Relacion 1:N con `equipos` mediante `equipos.estado_id`. |
| `nomenclaturas` | Catalogo de tipos de equipo y prefijos `LP`, `PC`, `IM`. | Relacion 1:N con `equipos` mediante `equipos.nomenclatura_id`. |
| `equipos` | Equipos ingresados por clientes, estado actual y descripcion del problema. | `cliente_id -> clientes.id`, `nomenclatura_id -> nomenclaturas.id`, `estado_id -> estados.id`; relacion con `entregas`. |
| `entregas` | Entrega final, diagnostico, repuestos usados, costo y ruta PDF. | `equipo_id -> equipos.id`. |

### Restricciones principales

- Todas las tablas usan `ENGINE=InnoDB`, necesario para claves foraneas y transacciones.
- La codificacion definida es `utf8mb4` con `utf8mb4_unicode_ci`, adecuada para acentos y caracteres amplios.
- Los campos `codigo`, `usuario`, `nombre` y `prefijo` relevantes usan `UNIQUE` para evitar duplicados funcionales.
- Las claves internas son `INT AUTO_INCREMENT PRIMARY KEY`; el usuario final trabaja con codigos visibles, no con IDs.
- `equipos` mantiene integridad hacia clientes, nomenclaturas y estados mediante claves foraneas.
- `entregas` mantiene integridad hacia equipos mediante `fk_entregas_equipos`.

## 6. Formularios WinForms y componentes visuales

### LoginForm

**Archivo:** `ProyectoNANDDOS/LoginForm.cs`

**Objetivo:** Pantalla de autenticacion inicial. Valida usuario y contrasena contra MySQL y BCrypt antes de permitir acceso al sistema.

**Componentes visuales principales:**
- txtUsuario: entrada de usuario.
- txtPassword: entrada de contrasena con PasswordChar.
- btnLogin: ejecuta IniciarSesion().
- Logo NANDDOS: se carga con ImagenEmpresa.CrearLogoCentrado().
- Labels Usuario y Contrase?a: identifican campos.

**Campos visuales declarados como atributos:**
- `txtUsuario` (`TextBox`).
- `txtPassword` (`TextBox`).
- `btnLogin` (`Button`).

**Eventos conectados:**
- `txtPassword.KeyDown`.
- `btnLogin.Click`.

**Flujo funcional:** El boton Iniciar sesion consulta usuarios.password_hash por usuario. Si BCrypt.Verify es correcto, abre BienvenidaForm y oculta el login hasta cerrar el flujo posterior.

### BienvenidaForm

**Archivo:** `ProyectoNANDDOS/BienvenidaForm.cs`

**Objetivo:** Pantalla intermedia posterior al login, minimalista, con saludo al usuario y boton Siguiente hacia el menu principal.

**Componentes visuales principales:**
- Logo NANDDOS centrado.
- Labels de bienvenida y descripcion.
- RoundedPanel: contenedor visual suave.
- RoundedButton btnSiguiente: abre MenuPrincipalForm.

**Eventos conectados:**
- `panelBoton.Resize`.
- `btnSiguiente.Click`.

**Flujo funcional:** Recibe el usuario autenticado. Al presionar Siguiente instancia MenuPrincipalForm(usuario), oculta la bienvenida y la restaura al cerrar menu.

### MenuPrincipalForm

**Archivo:** `ProyectoNANDDOS/MenuPrincipalForm.cs`

**Objetivo:** Ventana contenedora principal con barra lateral para navegar entre modulos.

**Componentes visuales principales:**
- panelContenido: area central donde se incrustan formularios.
- barraLateral: panel de navegacion.
- Logo NANDDOS.
- usuarioLabel: muestra usuario activo.
- Botones Registrar Equipo, Lista de Equipos, Clientes, Entrega.

**Campos visuales declarados como atributos:**
- `panelContenido` (`Panel`).

**Eventos conectados:**
- `boton.Click`.

**Flujo funcional:** Cada boton crea un Form hijo, lo marca TopLevel=false, lo acopla al panelContenido y actualiza el boton activo.

### RegistrarEquipoForm

**Archivo:** `ProyectoNANDDOS/RegistrarEquipoForm.cs`

**Objetivo:** Flujo guiado para buscar cliente existente, registrar cliente nuevo si no existe y registrar equipo con codigo visible.

**Componentes visuales principales:**
- txtBuscarCliente: busqueda por nombre o telefono.
- dgvClientes: resultados cuando hay mas de una coincidencia.
- panelResultado: datos del cliente encontrado o captura de cliente nuevo.
- btnRegistrarNuevoEquipo: continua con equipo para cliente existente.
- btnNuevoCliente: habilita captura completa de cliente nuevo.
- cmbTipoEquipo: tipos desde nomenclaturas.
- dtpFechaIngreso: fecha de ingreso.
- txtMarca/txtModelo/txtSerial: datos del equipo.
- txtProblema: descripcion del problema.
- txtRepuestosNecesarios: repuestos requeridos.
- panelScroll: evita cortes visuales con AutoScroll.

**Campos visuales declarados como atributos:**
- `txtBuscarCliente` (`TextBox`).
- `dgvClientes` (`DataGridView`).
- `panelResultado` (`Panel`).
- `panelFormulario` (`Panel`).
- `panelScroll` (`Panel`).
- `filaTablaClientes` (`RowStyle`).
- `btnRegistrarNuevoEquipo` (`Button`).
- `btnNuevoCliente` (`Button`).
- `txtNombresEncontrado` (`TextBox`).
- `txtApellidosEncontrado` (`TextBox`).
- `txtTelefonoEncontrado` (`TextBox`).
- `txtEmailEncontrado` (`TextBox`).
- `txtNombresNuevo` (`TextBox`).
- `txtApellidosNuevo` (`TextBox`).
- `txtTelefonoNuevo` (`TextBox`).
- `txtEmailNuevo` (`TextBox`).
- `grupoClienteNuevo` (`GroupBox`).
- `grupoClienteResumen` (`GroupBox`).
- `lblClienteResumen` (`Label`).
- `cmbTipoEquipo` (`ComboBox`).
- `dtpFechaIngreso` (`DateTimePicker`).
- `txtMarca` (`TextBox`).
- `txtModelo` (`TextBox`).
- `txtSerial` (`TextBox`).
- `txtProblema` (`TextBox`).
- `txtRepuestosNecesarios` (`TextBox`).

**Eventos conectados:**
- `panelScroll.Resize`.
- `txtBuscarCliente.KeyDown`.
- `btnBuscar.Click`.
- `btnRegistrarNuevoEquipo.Click`.
- `btnNuevoCliente.Click`.
- `dgvClientes.CellClick`.
- `dgvClientes.CellDoubleClick`.
- `btnGuardar.Click`.
- `btnCancelar.Click`.
- `txtNombresEncontrado.TextChanged`.
- `txtApellidosEncontrado.TextChanged`.
- `txtTelefonoEncontrado.TextChanged`.
- `txtEmailEncontrado.TextChanged`.

**Flujo funcional:** BuscarClientes consulta clientes. Si existe, muestra datos y permite registrar nuevo equipo. Si no existe, permite crear cliente y luego equipo en una transaccion. El equipo inicia en estado En diagnostico.

### ListaEquiposForm

**Archivo:** `ProyectoNANDDOS/ListaEquiposForm.cs`

**Objetivo:** Consulta operativa de equipos con busqueda, filtro por estado, acciones sobre seleccion y detalle ampliado.

**Componentes visuales principales:**
- txtBusqueda: busca por codigo, cliente, telefono, marca o problema.
- cmbEstados: filtro por estados.
- dgvEquipos: tabla de equipos con columnas fijas.
- btnBuscar: recarga listado.
- btnCopiarCodigo: copia codigo visible al portapapeles.
- btnCambiarEstado: abre modal para actualizar estado.
- btnVerDetalles: abre DetalleEquipoForm.
- btnEditar: edita datos del equipo sin cambiar estado.
- btnEliminar: elimina equipo y entregas asociadas dentro de transaccion.

**Campos visuales declarados como atributos:**
- `txtBusqueda` (`TextBox`).
- `cmbEstados` (`ComboBox`).
- `dgvEquipos` (`DataGridView`).

**Eventos conectados:**
- `txtBusqueda.KeyDown`.
- `cmbEstados.SelectedIndexChanged`.
- `btnBuscar.Click`.
- `btnCopiarCodigo.Click`.
- `btnCambiarEstado.Click`.
- `btnVerDetalles.Click`.
- `btnEditar.Click`.
- `btnEliminar.Click`.

**Flujo funcional:** CargarEstados llena ComboBox. CargarEquipos arma consulta con filtros. Las acciones usan el id interno oculto y mantienen visibles solo codigos como LP-0001.

### ClientesForm

**Archivo:** `ProyectoNANDDOS/ClientesForm.cs`

**Objetivo:** Administracion de clientes registrados.

**Componentes visuales principales:**
- txtBusqueda: filtro por codigo, nombres, apellidos, email o telefono.
- dgvClientes: tabla de clientes con columnas fijas.
- btnBuscar: recarga listado.
- btnEditar: abre modal de edicion.
- btnEliminar: elimina si no tiene equipos asociados.

**Campos visuales declarados como atributos:**
- `txtBusqueda` (`TextBox`).
- `dgvClientes` (`DataGridView`).

**Eventos conectados:**
- `txtBusqueda.KeyDown`.
- `btnBuscar.Click`.
- `btnEditar.Click`.
- `btnEliminar.Click`.

**Flujo funcional:** CargarClientes consulta clientes. EditarCliente actualiza datos. EliminarCliente valida primero que no existan equipos relacionados para proteger integridad.

### EntregaForm

**Archivo:** `ProyectoNANDDOS/EntregaForm.cs`

**Objetivo:** Registrar entrega final de equipo, impedir entregas duplicadas y generar/regenerar comprobante PDF.

**Componentes visuales principales:**
- txtCodigoBusqueda: busqueda por codigo visible del equipo.
- txtCliente/txtTelefono/txtEmail: datos autocompletados.
- txtEquipo/txtProblema: datos del equipo.
- txtDiagnostico: diagnostico final.
- txtRepuestosUsados: repuestos usados.
- nudCostoTotal: costo numerico.
- dtpFechaEntrega: fecha final.
- txtResumen: resumen antes de confirmar.
- btnGenerar: inserta entrega, actualiza estado y genera PDF.

**Campos visuales declarados como atributos:**
- `txtCodigoBusqueda` (`TextBox`).
- `txtCliente` (`TextBox`).
- `txtTelefono` (`TextBox`).
- `txtEmail` (`TextBox`).
- `txtEquipo` (`TextBox`).
- `txtProblema` (`TextBox`).
- `txtDiagnostico` (`TextBox`).
- `txtRepuestosUsados` (`TextBox`).
- `nudCostoTotal` (`NumericUpDown`).
- `dtpFechaEntrega` (`DateTimePicker`).
- `txtResumen` (`TextBox`).
- `btnGenerar` (`Button`).

**Eventos conectados:**
- `btnGenerar.Click`.
- `txtCodigoBusqueda.KeyDown`.
- `btnBuscar.Click`.

**Flujo funcional:** BuscarEquipo recupera datos por codigo. GenerarEntrega crea codigo ENT, inserta entrega, actualiza estado a Entregado y guarda PDF en ProyectoNANDDOS/PDF. Si ya existe entrega, ofrece regenerar comprobante sin duplicar registro.

### DetalleEquipoForm

**Archivo:** `ProyectoNANDDOS/DetalleEquipoForm.cs`

**Objetivo:** Ventana de solo lectura para ver toda la informacion de un equipo seleccionado.

**Componentes visuales principales:**
- Panel con AutoScroll para textos largos.
- GroupBox Cliente, Equipo y Servicio.
- TextBox de solo lectura para cada campo.
- btnCerrar.

**Eventos conectados:**
- `scroll.Resize`.

**Flujo funcional:** Recibe un DataRow desde ListaEquiposForm y distribuye valores en secciones legibles sin permitir edicion.

## 7. Inventario de clases principales

| Archivo | Clases detectadas | Responsabilidad | Metodos principales detectados | Imports explicitos |
|---|---|---|---|---|
| `ProyectoNANDDOS/BienvenidaForm.cs` | `BienvenidaForm`, `RoundedPanel`, `RoundedButton` | Pantalla intermedia posterior al login, minimalista, con saludo al usuario y boton Siguiente hacia el menu principal. | `BienvenidaForm`, `InicializarComponentes`, `CrearContenidoPrincipal`, `CentrarBoton`, `AbrirMenuPrincipal`, `RoundedPanel`, `OnPaint`, `RoundedButton`, `OnMouseEnter`, `OnMouseLeave`, `OnPaint`, `CrearRectanguloRedondeado` | `System.Drawing.Drawing2D` |
| `ProyectoNANDDOS/ClientesForm.cs` | `ClientesForm` | Administracion de clientes registrados. | `ClientesForm`, `InicializarComponentes`, `CrearBoton`, `PrepararGrid`, `CargarClientes`, `CONCAT`, `ConfigurarColumnasClientes`, `ConfigurarColumna`, `ObtenerClienteSeleccionado`, `EditarCliente`, `EliminarCliente`, `ObtenerCliente`, `CrearFormularioEdicion`, `CrearTextBox`, `CrearEtiqueta` | `MySql.Data.MySqlClient`, `System.Data` |
| `ProyectoNANDDOS/ConexionDB.cs` | `ConexionDB` | Capa de acceso basica a MySQL. Centraliza server, port, database, user y password. Construye la cadena con MySqlConnectionStringBuilder, abre conexiones y prueba SELECT 1. | `ObtenerConexion`, `ProbarConexion` | `MySql.Data.MySqlClient` |
| `ProyectoNANDDOS/DetalleEquipoForm.cs` | `DetalleEquipoForm` | Ventana de solo lectura para ver toda la informacion de un equipo seleccionado. | `DetalleEquipoForm`, `InicializarComponentes`, `CrearGrupoCliente`, `CrearGrupoEquipo`, `CrearGrupoServicio`, `CrearGrupo`, `CrearTabla`, `AgregarCampo`, `Valor`, `Fecha`, `Costo` | `System.Data` |
| `ProyectoNANDDOS/EntregaForm.cs` | `EntregaForm`, `DatosComprobante`, `EventoComprobantePdf` | Registrar entrega final de equipo, impedir entregas duplicadas y generar/regenerar comprobante PDF. | `EntregaForm`, `InicializarComponentes`, `CrearBarraBusqueda`, `CrearDatosEquipo`, `CrearDatosEntrega`, `CrearEtiqueta`, `PrepararSoloLectura`, `PrepararTexto`, `BuscarEquipo`, `CONCAT`, `CONCAT`, `GenerarEntrega`, `PreguntarRegenerarComprobante`, `RegenerarComprobanteExistente`, `ObtenerDatosComprobanteExistente`, `CONCAT`, `ActualizarRutaPdfEntrega`, `ObtenerTexto`, ... | `MySql.Data.MySqlClient`, `System.Data`, `System.Diagnostics`, `System.Drawing.Imaging`, `PdfDocument = iTextSharp.text.Document`, `PdfParagraph = iTextSharp.text.Paragraph`, `PdfPTable = iTextSharp.text.pdf.PdfPTable`, `PdfPCell = iTextSharp.text.pdf.PdfPCell`, `PdfPhrase = iTextSharp.text.Phrase`, `PdfImage = iTextSharp.text.Image`, `PdfWriter = iTextSharp.text.pdf.PdfWriter`, `PdfBaseColor = iTextSharp.text.BaseColor`, `PdfFont = iTextSharp.text.Font`, `PdfElement = iTextSharp.text.Element`, `PdfPageEventHelper = iTextSharp.text.pdf.PdfPageEventHelper`, `PdfGState = iTextSharp.text.pdf.PdfGState` |
| `ProyectoNANDDOS/GeneradorCodigo.cs` | `GeneradorCodigo` | Servicio helper para generar codigos visibles consecutivos: CLI, LP, PC, IM y ENT. Consulta el mayor codigo existente con el prefijo correspondiente. | `GenerarCodigoCliente`, `Generar`, `GenerarCodigoEquipo`, `Generar`, `GenerarCodigoEntrega`, `Generar`, `Generar`, `IFNULL`, `EsPrefijoValido` | `MySql.Data.MySqlClient` |
| `ProyectoNANDDOS/ImagenEmpresa.cs` | `ImagenEmpresa` | Helper de recursos visuales. Busca imagen oficial en IMGNANDDOS desde varias rutas candidatas, crea PictureBox centrado y devuelve ruta para PDF. | `CrearLogoCentrado`, `Centrar`, `ObtenerRutaImagen`, `BuscarRutaImagen`, `BuscarRutaImagen`, `ObtenerCarpetasCandidatas` | Usa `ImplicitUsings`/namespace global de WinForms. |
| `ProyectoNANDDOS/ListaEquiposForm.cs` | `ListaEquiposForm`, `EstadoItem` | Consulta operativa de equipos con busqueda, filtro por estado, acciones sobre seleccion y detalle ampliado. | `ListaEquiposForm`, `InicializarComponentes`, `CrearBoton`, `PrepararGrid`, `CargarEstados`, `CargarEquipos`, `CONCAT`, `ConfigurarColumnasEquipos`, `ConfigurarColumna`, `ObtenerEquipoSeleccionado`, `ObtenerCodigoEquipoSeleccionado`, `CopiarCodigoEquipo`, `EditarEquipo`, `CambiarEstadoEquipo`, `VerDetallesEquipo`, `EliminarEquipo`, `ObtenerEquipo`, `CONCAT`, ... | `MySql.Data.MySqlClient`, `System.Data` |
| `ProyectoNANDDOS/LoginForm.cs` | `LoginForm` | Pantalla de autenticacion inicial. Valida usuario y contrasena contra MySQL y BCrypt antes de permitir acceso al sistema. | `LoginForm`, `InicializarComponentes`, `IniciarSesion` | `MySql.Data.MySqlClient` |
| `ProyectoNANDDOS/MenuPrincipalForm.cs` | `MenuPrincipalForm` | Ventana contenedora principal con barra lateral para navegar entre modulos. | `MenuPrincipalForm`, `InicializarComponentes`, `CrearBotonMenu`, `AbrirFormulario` | Usa `ImplicitUsings`/namespace global de WinForms. |
| `ProyectoNANDDOS/Program.cs` | `Program` | Punto de entrada. Inicializa configuracion visual de WinForms, prueba ConexionDB.ProbarConexion() y abre LoginForm. Si MySQL no responde, muestra mensaje claro sin crear base de datos. | `Main` | Usa `ImplicitUsings`/namespace global de WinForms. |
| `ProyectoNANDDOS/RegistrarEquipoForm.cs` | `RegistrarEquipoForm`, `NomenclaturaItem` | Flujo guiado para buscar cliente existente, registrar cliente nuevo si no existe y registrar equipo con codigo visible. | `RegistrarEquipoForm`, `InicializarComponentes`, `AjustarAncho`, `CrearEncabezado`, `CrearPanelBusqueda`, `CrearPanelResultado`, `CrearPanelFormulario`, `CrearCamposClienteNuevo`, `CrearGrupoEquipo`, `CrearGrupo`, `AgregarCampo`, `CrearEtiqueta`, `PrepararTextBox`, `PrepararSoloLectura`, `VincularCamposClienteNuevo`, `SincronizarCampoClienteNuevo`, `ConfigurarCamposResultadoClienteNuevo`, `CrearBotonPrincipal`, ... | `MySql.Data.MySqlClient`, `System.Data` |

## 8. Flujos funcionales relevantes

### Autenticacion
1. `Program.Main()` llama `ConexionDB.ProbarConexion()`.
2. Si MySQL responde, abre `LoginForm`.
3. `LoginForm.IniciarSesion()` consulta `usuarios.password_hash`.
4. `BCrypt.Net.BCrypt.Verify()` valida la contrasena ingresada.
5. Si el acceso es correcto, se abre `BienvenidaForm`.

### Registro de cliente/equipo
1. `RegistrarEquipoForm` carga tipos desde `nomenclaturas`.
2. El usuario busca cliente por nombre o telefono.
3. Si existe, el formulario usa `clienteSeleccionadoId` y no pide datos del cliente otra vez.
4. Si no existe, habilita captura de cliente nuevo.
5. `GuardarEquipo()` valida campos, inicia transaccion, genera codigos y guarda cliente/equipo.
6. El estado inicial se toma de `estados.nombre = En diagn?stico`.

### Lista y administracion de equipos
1. `ListaEquiposForm.CargarEquipos()` consulta equipos con cliente, nomenclatura y estado.
2. El `DataGridView` muestra codigo, cliente, equipo, problema, estado y fecha; el id interno queda oculto.
3. Acciones disponibles: copiar codigo, cambiar estado, ver detalles, editar datos y eliminar.

### Entrega y PDF
1. `EntregaForm.BuscarEquipo()` localiza equipo por codigo visible.
2. Si ya existe entrega, `PreguntarRegenerarComprobante()` permite reimprimir sin insertar otro registro.
3. Si no existe entrega, `GenerarEntrega()` crea codigo `ENT`, inserta entrega, actualiza estado a Entregado y genera PDF.
4. `CrearRutaPdf()` guarda comprobantes en `ProyectoNANDDOS/PDF`.

## 9. Recursos, PDF e informacion del proyecto

- `IMGNANDDOS/`: contiene la imagen oficial usada en Login, Bienvenida, Menu y PDF.
- `ProyectoNANDDOS/PDF/`: contiene comprobantes generados, por ejemplo `ENT-0005.pdf`.
- `ProyectoNANDDOS/InformacionProyecto/`: contiene documentacion tecnica, documentacion de base de datos, instrucciones SQL y este contexto maestro.
- `Scripts/CrearBaseDatos.sql`: script operativo principal de base de datos.

## 10. Observaciones para el Arquitecto de Software

- La aplicacion ya no crea la base de datos automaticamente; asume que `proyecto_nanddos_db` existe.
- La conexion esta centralizada en `ConexionDB.cs`, con credenciales editables en constantes.
- La aplicacion usa codigos visibles personalizados y oculta IDs internos en la interfaz.
- La capa de datos esta acoplada a formularios. Para crecer, se recomienda introducir repositorios o servicios por entidad.
- `EntregaForm.cs` concentra mucha responsabilidad: UI, reglas de entrega, consultas SQL y generacion PDF. Es el primer candidato a separacion futura.
- `RegistrarEquipoForm.cs` tambien contiene flujo de UI y transacciones; podria dividirse en servicio de clientes/equipos.
- Los scripts y documentacion ya estan dentro de `InformacionProyecto`, lo que facilita entrega academica y revision arquitectonica.

## 11. Checklist de contexto entregado

- [x] Arbol de directorios.
- [x] Separacion logica de capas MVC.
- [x] Script DDL actual completo.
- [x] Formularios WinForms y componentes visuales.
- [x] Inventario de clases principales y responsabilidad.
- [x] Dependencias y paquetes NuGet directos/transitivos.
