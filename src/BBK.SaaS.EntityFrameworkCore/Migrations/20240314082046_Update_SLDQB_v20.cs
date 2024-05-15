using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBK.SaaS.Migrations
{
    /// <inheritdoc />
    public partial class Update_SLDQB_v20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AppRecruitments_FormOfWork",
                schema: "BBKProfile",
                table: "AppRecruitments",
                column: "FormOfWork");

            migrationBuilder.AddForeignKey(
                name: "FK_AppRecruitments_CatUnits_FormOfWork",
                schema: "BBKProfile",
                table: "AppRecruitments",
                column: "FormOfWork",
                principalSchema: "BBK",
                principalTable: "CatUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppRecruitments_CatUnits_FormOfWork",
                schema: "BBKProfile",
                table: "AppRecruitments");

            migrationBuilder.DropIndex(
                name: "IX_AppRecruitments_FormOfWork",
                schema: "BBKProfile",
                table: "AppRecruitments");
        }
    }
}
