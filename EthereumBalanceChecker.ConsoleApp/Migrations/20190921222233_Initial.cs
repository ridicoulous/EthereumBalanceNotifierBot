using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EthereumBalanceChecker.ConsoleApp.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Balance = table.Column<decimal>(nullable: false),
                    LastUpdate = table.Column<DateTime>(nullable: false),
                    IsNotificationEnabled = table.Column<bool>(nullable: false),
                    BalanceGreaterThan = table.Column<decimal>(nullable: true),
                    BalanceLowerThan = table.Column<decimal>(nullable: true),
                    BalanceChanged = table.Column<bool>(nullable: false),
                    EveryTx = table.Column<bool>(nullable: false),
                    IsMessageSended = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => new { x.Id, x.UserId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");
        }
    }
}
