using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBK.SaaS.Migrations
{
    /// <inheritdoc />
    public partial class Update_SLDQB_v15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStatic",
                schema: "BBKCms",
                table: "AppTopics",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PageTemplateType",
                schema: "BBKCms",
                table: "AppTopics",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStatic",
                schema: "BBKCms",
                table: "AppTopics");

            migrationBuilder.DropColumn(
                name: "PageTemplateType",
                schema: "BBKCms",
                table: "AppTopics");
        }
    }
}
