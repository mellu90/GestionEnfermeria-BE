using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GestionEnfermeria.Migrations
{
    /// <inheritdoc />
    public partial class m1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Campo",
                columns: table => new
                {
                    Id_Campo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo_Campo = table.Column<string>(type: "text", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campo", x => x.Id_Campo);
                });

            migrationBuilder.CreateTable(
                name: "Enfermera",
                columns: table => new
                {
                    Id_Enfermera = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo_Enfermera = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Apellido_Paterno = table.Column<string>(type: "text", nullable: false),
                    Apellido_Materno = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enfermera", x => x.Id_Enfermera);
                });

            migrationBuilder.CreateTable(
                name: "Seguimiento",
                columns: table => new
                {
                    Id_Seguimiento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo_Seguro = table.Column<string>(type: "text", nullable: false),
                    Codigo_Seguimiento = table.Column<string>(type: "text", nullable: false),
                    Estado_Seguimiento = table.Column<string>(type: "text", nullable: false),
                    Fecha_Inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    Fecha_Final = table.Column<DateOnly>(type: "date", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seguimiento", x => x.Id_Seguimiento);
                });

            migrationBuilder.CreateTable(
                name: "Turno",
                columns: table => new
                {
                    Id_Turno = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo_Turno = table.Column<string>(type: "text", nullable: false),
                    Nombre_Turno = table.Column<string>(type: "text", nullable: false),
                    Hora_Inicio = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Hora_Final = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turno", x => x.Id_Turno);
                });

            migrationBuilder.CreateTable(
                name: "Detalle_Seguimiento",
                columns: table => new
                {
                    Id_Detalle_Seguimiento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_Seguimiento = table.Column<int>(type: "integer", nullable: false),
                    Id_Enfermera = table.Column<int>(type: "integer", nullable: false),
                    Codigo_Receta = table.Column<string>(type: "text", nullable: false),
                    Observacion = table.Column<string>(type: "text", nullable: true),
                    Fecha_Inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    Fecha_Final = table.Column<DateOnly>(type: "date", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detalle_Seguimiento", x => x.Id_Detalle_Seguimiento);
                    table.ForeignKey(
                        name: "FK_Detalle_Seguimiento_Enfermera_Id_Enfermera",
                        column: x => x.Id_Enfermera,
                        principalTable: "Enfermera",
                        principalColumn: "Id_Enfermera",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Detalle_Seguimiento_Seguimiento_Id_Seguimiento",
                        column: x => x.Id_Seguimiento,
                        principalTable: "Seguimiento",
                        principalColumn: "Id_Seguimiento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Asignar",
                columns: table => new
                {
                    Id_Asignar = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_Enfermera = table.Column<int>(type: "integer", nullable: false),
                    Id_Turno = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asignar", x => x.Id_Asignar);
                    table.ForeignKey(
                        name: "FK_Asignar_Enfermera_Id_Enfermera",
                        column: x => x.Id_Enfermera,
                        principalTable: "Enfermera",
                        principalColumn: "Id_Enfermera",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Asignar_Turno_Id_Turno",
                        column: x => x.Id_Turno,
                        principalTable: "Turno",
                        principalColumn: "Id_Turno",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Turno_Campo",
                columns: table => new
                {
                    Id_Turno_Campo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_Turno = table.Column<int>(type: "integer", nullable: false),
                    Id_Campo = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turno_Campo", x => x.Id_Turno_Campo);
                    table.ForeignKey(
                        name: "FK_Turno_Campo_Campo_Id_Campo",
                        column: x => x.Id_Campo,
                        principalTable: "Campo",
                        principalColumn: "Id_Campo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Turno_Campo_Turno_Id_Turno",
                        column: x => x.Id_Turno,
                        principalTable: "Turno",
                        principalColumn: "Id_Turno",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asignar_Id_Enfermera",
                table: "Asignar",
                column: "Id_Enfermera");

            migrationBuilder.CreateIndex(
                name: "IX_Asignar_Id_Turno",
                table: "Asignar",
                column: "Id_Turno");

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Seguimiento_Id_Enfermera",
                table: "Detalle_Seguimiento",
                column: "Id_Enfermera");

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Seguimiento_Id_Seguimiento",
                table: "Detalle_Seguimiento",
                column: "Id_Seguimiento");

            migrationBuilder.CreateIndex(
                name: "IX_Turno_Campo_Id_Campo",
                table: "Turno_Campo",
                column: "Id_Campo");

            migrationBuilder.CreateIndex(
                name: "IX_Turno_Campo_Id_Turno",
                table: "Turno_Campo",
                column: "Id_Turno");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asignar");

            migrationBuilder.DropTable(
                name: "Detalle_Seguimiento");

            migrationBuilder.DropTable(
                name: "Turno_Campo");

            migrationBuilder.DropTable(
                name: "Enfermera");

            migrationBuilder.DropTable(
                name: "Seguimiento");

            migrationBuilder.DropTable(
                name: "Campo");

            migrationBuilder.DropTable(
                name: "Turno");
        }
    }
}
