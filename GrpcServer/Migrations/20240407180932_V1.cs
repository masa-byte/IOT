using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GrpcServer.Migrations
{
    /// <inheritdoc />
    public partial class V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PondData",
                columns: table => new
                {
                    EntryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PondId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TempC = table.Column<float>(type: "real", nullable: false),
                    TurbidityNtu = table.Column<int>(type: "integer", nullable: false),
                    DissolvedOxygenGMl = table.Column<float>(type: "real", nullable: false),
                    Ph = table.Column<float>(type: "real", nullable: false),
                    AmmoniaGMl = table.Column<float>(type: "real", nullable: false),
                    NitriteGMl = table.Column<float>(type: "real", nullable: false),
                    Population = table.Column<int>(type: "integer", nullable: false),
                    FishLengthCm = table.Column<float>(type: "real", nullable: false),
                    FishWeightG = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PondData", x => x.EntryId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PondData");
        }
    }
}
