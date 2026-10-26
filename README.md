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
4. Configurar y crear la base de datos ejecutando el comando `Update-Database` en la Consola del Administrador de Paquetes.
5. Seleccionar **Compilar → Compilar solución**.
6. Ejecutar la aplicación con **Ctrl + F5**.
7. Acceder a los módulos desde el menú de navegación.

---

## P2 - Gestión de Habitaciones, Servicios y Base de Datos

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

---

## P4 - Gestión de Servicios Contratados y Reportes

### Gestión de Servicios por Reserva (ReservaServicio)

Se implementó el módulo que permite asociar servicios adicionales del catálogo a una reserva específica.

Funcionalidades:
- Registro de un servicio contratado para una reserva, indicando fecha y hora.
- Listado de todos los servicios contratados, mostrando la reserva, el huésped y el servicio.
- Eliminación (quitar) de un servicio de una reserva sin eliminarlo del catálogo general.
- Validación en el servidor: La fecha del servicio debe estar estrictamente dentro del período de estadía de la reserva (entre FechaInicio y FechaFin).
- Prevención de duplicados: Gracias a la clave compuesta, un mismo servicio no puede asociarse dos veces a la misma reserva.

### Reporte 1: Ocupación de Habitaciones por Fecha
- **¿Qué muestra?** Un panel con el total de habitaciones, cuántas están ocupadas, cuántas disponibles y el porcentaje de ocupación. Además, lista cada habitación con su estado (Ocupada/Disponible).
- **Filtros:** Fecha específica y tipo de habitación (Simple, Doble, Suite).
- **Utilidad:** Es una herramienta operativa para recepción. Permite saber de forma inmediata qué habitaciones están libres para asignar a clientes que llegan sin reserva, y ayuda a evaluar la demanda del hotel en fechas específicas. El cálculo se hace en tiempo real basándose en las reservas no canceladas.

### Reporte 2: Servicios Solicitados por Período
- **¿Qué muestra?** Una tabla con la cantidad de veces que se ha contratado cada servicio, junto con un total general de contrataciones.
- **Filtros:** Rango de fechas (Desde/Hasta) y un servicio específico.
- **Utilidad:** Permite al área administrativa identificar cuáles son los servicios más solicitados o rentables (ej. desayuno, masajes). Este reporte **excluye automáticamente** las reservas en estado "Cancelada" para reflejar únicamente los consumos reales que generaron ingresos.