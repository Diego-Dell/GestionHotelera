# Gestión Hotelera

Aplicación web para gestionar huéspedes, habitaciones, reservas y servicios adicionales de un hotel, desarrollada con ASP.NET Core MVC, Entity Framework Core y SQL Server.

## Integrantes

| Integrante | Usuario de GitHub |
|---|---|
| Walkyria Nicole Audivert Lino | waudivert |
| Diego Aaron Dell Fernandez | Diego-Dell |
| Mikaela Gutierrez Urquiza | MikaGu06 |
| Pablo Ricardo Quiroz Rodriguez | pabloquirozxd |

## Requisitos

- SDK de .NET 10.
- SQL Server.
- Visual Studio compatible con .NET 10 y la carga de trabajo de desarrollo web.

## Pasos para ejecutar la aplicación

1. Clonar el repositorio.
2. Abrir `GestionHotelera.slnx` en Visual Studio.
3. Restaurar los paquetes NuGet de la solución.
4. Configurar y crear la base de datos siguiendo las instrucciones del apartado correspondiente.
5. Seleccionar **Compilar → Compilar solución**.
6. Ejecutar la aplicación con **Ctrl + F5**.
7. Acceder a los módulos desde el menú de navegación.

P2
## Gestión de Habitaciones, Servicios y Base de Datos

### Gestión de Habitaciones

Se implementó el módulo de administración de habitaciones utilizando ASP.NET Core MVC y Entity Framework Core.

Funcionalidades:

- Registro de habitaciones.
- Listado de habitaciones.
- Visualización de detalles.
- Edición de información.
- Eliminación de habitaciones sin reservas asociadas.
- Búsqueda por número de habitación.
- Filtro por tipo de habitación.
- Paginación de 10 registros por página.
- Validación de campos obligatorios.
- Validación de precios mayores que cero.
- Comprobación de números de habitación duplicados.

### Gestión de Servicios

Se implementó el módulo de administración del catálogo de servicios del hotel.

Funcionalidades:

- Registro de servicios.
- Listado de servicios.
- Visualización de detalles.
- Edición de nombre y precio.
- Eliminación de servicios sin reservas asociadas.
- Búsqueda de servicios por nombre.
- Paginación de 10 registros por página.
- Validación de nombre obligatorio.
- Validación de precios mayores que cero.
- Comprobación de nombres duplicados, incluyendo diferencias de mayúsculas y espacios.

### Gestión de Base de Datos

Se trabajó con SQL Server y Entity Framework Core mediante `AppDbContext`.

Responsabilidades:

- Revisión de las entidades y relaciones del modelo Entidad-Relación.
- Conservación de las claves primarias y foráneas.
- Gestión centralizada de las migraciones.
- Configuración de la conexión a SQL Server.
- Persistencia de los registros mediante Entity Framework Core.


