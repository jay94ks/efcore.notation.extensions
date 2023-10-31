using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Efcore.Notations.Extensions.Example.Migrations
{
    /// <inheritdoc />
    public partial class AddSomeEntityA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TableForSomeEntityA",
                columns: table => new
                {
                    Guid = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    RemoteAddress = table.Column<string>(type: "TEXT", maxLength: 48, nullable: true),
                    Text = table.Column<string>(type: "LONGTEXT", nullable: true),
                    Password = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOMEENTITYA", x => new { x.Guid, x.Number });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TableForSomeEntityA");
        }
    }
}
