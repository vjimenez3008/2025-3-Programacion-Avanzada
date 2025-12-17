using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelmasCarga.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    IdCliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.IdCliente);
                });

            migrationBuilder.CreateTable(
                name: "TiposHabitacion",
                columns: table => new
                {
                    IdTipoHabitacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposHabitacion", x => x.IdTipoHabitacion);
                });

            migrationBuilder.CreateTable(
                name: "ColasEspera",
                columns: table => new
                {
                    IdCola = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    IdTipoHabitacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColasEspera", x => x.IdCola);
                    table.ForeignKey(
                        name: "FK_ColasEspera_Clientes_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ColasEspera_TiposHabitacion_IdTipoHabitacion",
                        column: x => x.IdTipoHabitacion,
                        principalTable: "TiposHabitacion",
                        principalColumn: "IdTipoHabitacion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Habitaciones",
                columns: table => new
                {
                    IdHabitacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false),
                    IdTipoHabitacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habitaciones", x => x.IdHabitacion);
                    table.ForeignKey(
                        name: "FK_Habitaciones_TiposHabitacion_IdTipoHabitacion",
                        column: x => x.IdTipoHabitacion,
                        principalTable: "TiposHabitacion",
                        principalColumn: "IdTipoHabitacion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Disponibilidades",
                columns: table => new
                {
                    IdDisponibilidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstaDisponible = table.Column<bool>(type: "bit", nullable: false),
                    IdHabitacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disponibilidades", x => x.IdDisponibilidad);
                    table.ForeignKey(
                        name: "FK_Disponibilidades_Habitaciones_IdHabitacion",
                        column: x => x.IdHabitacion,
                        principalTable: "Habitaciones",
                        principalColumn: "IdHabitacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    IdReserva = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    IdHabitacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.IdReserva);
                    table.ForeignKey(
                        name: "FK_Reservas_Clientes_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservas_Habitaciones_IdHabitacion",
                        column: x => x.IdHabitacion,
                        principalTable: "Habitaciones",
                        principalColumn: "IdHabitacion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialesReserva",
                columns: table => new
                {
                    IdHistorial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Accion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdReserva = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialesReserva", x => x.IdHistorial);
                    table.ForeignKey(
                        name: "FK_HistorialesReserva_Reservas_IdReserva",
                        column: x => x.IdReserva,
                        principalTable: "Reservas",
                        principalColumn: "IdReserva",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColasEspera_IdCliente",
                table: "ColasEspera",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_ColasEspera_IdTipoHabitacion",
                table: "ColasEspera",
                column: "IdTipoHabitacion");

            migrationBuilder.CreateIndex(
                name: "IX_Disponibilidades_IdHabitacion",
                table: "Disponibilidades",
                column: "IdHabitacion");

            migrationBuilder.CreateIndex(
                name: "IX_Habitaciones_IdTipoHabitacion",
                table: "Habitaciones",
                column: "IdTipoHabitacion");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesReserva_IdReserva",
                table: "HistorialesReserva",
                column: "IdReserva");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_IdCliente",
                table: "Reservas",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_IdHabitacion",
                table: "Reservas",
                column: "IdHabitacion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColasEspera");

            migrationBuilder.DropTable(
                name: "Disponibilidades");

            migrationBuilder.DropTable(
                name: "HistorialesReserva");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Habitaciones");

            migrationBuilder.DropTable(
                name: "TiposHabitacion");
        }
    }
}
