using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBK.SaaS.Migrations
{
    /// <inheritdoc />
    public partial class Update_SLDQB_v23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderIndex",
                schema: "BBK",
                table: "CatUnits",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrderIndex",
                schema: "BBKCms",
                table: "AppCmsCats",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderIndex",
                schema: "BBK",
                table: "CatUnits");

            migrationBuilder.DropColumn(
                name: "OrderIndex",
                schema: "BBKCms",
                table: "AppCmsCats");
        }
    }
}
