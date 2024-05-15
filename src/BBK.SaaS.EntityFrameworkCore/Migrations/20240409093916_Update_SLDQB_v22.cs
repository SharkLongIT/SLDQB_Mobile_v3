using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBK.SaaS.Migrations
{
    /// <inheritdoc />
    public partial class Update_SLDQB_v22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "JobApplicationId",
                schema: "BBKProfile",
                table: "AppTradingSessionAccounts",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppTradingSessionAccounts_JobApplicationId",
                schema: "BBKProfile",
                table: "AppTradingSessionAccounts",
                column: "JobApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTradingSessionAccounts_AppJobApplications_JobApplication~",
                schema: "BBKProfile",
                table: "AppTradingSessionAccounts",
                column: "JobApplicationId",
                principalSchema: "BBKProfile",
                principalTable: "AppJobApplications",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppTradingSessionAccounts_AppJobApplications_JobApplication~",
                schema: "BBKProfile",
                table: "AppTradingSessionAccounts");

            migrationBuilder.DropIndex(
                name: "IX_AppTradingSessionAccounts_JobApplicationId",
                schema: "BBKProfile",
                table: "AppTradingSessionAccounts");

            migrationBuilder.DropColumn(
                name: "JobApplicationId",
                schema: "BBKProfile",
                table: "AppTradingSessionAccounts");
        }
    }
}
