using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBK.SaaS.Migrations
{
    /// <inheritdoc />
    public partial class Update_SLDQB_v21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountCandidateMax",
                schema: "BBKProfile",
                table: "AppTradingSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CountRecruiterMax",
                schema: "BBKProfile",
                table: "AppTradingSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Describe",
                schema: "BBKProfile",
                table: "AppTradingSessions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImgUrl",
                schema: "BBKProfile",
                table: "AppTradingSessions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountCandidateMax",
                schema: "BBKProfile",
                table: "AppTradingSessions");

            migrationBuilder.DropColumn(
                name: "CountRecruiterMax",
                schema: "BBKProfile",
                table: "AppTradingSessions");

            migrationBuilder.DropColumn(
                name: "Describe",
                schema: "BBKProfile",
                table: "AppTradingSessions");

            migrationBuilder.DropColumn(
                name: "ImgUrl",
                schema: "BBKProfile",
                table: "AppTradingSessions");
        }
    }
}
