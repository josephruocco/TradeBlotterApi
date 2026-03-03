using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeBlotterApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitBlotter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TradeTimeUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Desk = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Product = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Symbol = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Side = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Counterparty = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Trader = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trades");
        }
    }
}
