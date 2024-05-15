using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBK.SaaS.Migrations
{
    /// <inheritdoc />
    public partial class Update_SLDQB_v14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppCandidates_GeoUnits_DistrictId",
                schema: "BBKProfile",
                table: "AppCandidates");

            migrationBuilder.DropForeignKey(
                name: "FK_AppCandidates_GeoUnits_ProvinceId",
                schema: "BBKProfile",
                table: "AppCandidates");

            migrationBuilder.AlterColumn<long>(
                name: "ProvinceId",
                schema: "BBKProfile",
                table: "AppCandidates",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "DistrictId",
                schema: "BBKProfile",
                table: "AppCandidates",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_AppCandidates_GeoUnits_DistrictId",
                schema: "BBKProfile",
                table: "AppCandidates",
                column: "DistrictId",
                principalSchema: "BBK",
                principalTable: "GeoUnits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppCandidates_GeoUnits_ProvinceId",
                schema: "BBKProfile",
                table: "AppCandidates",
                column: "ProvinceId",
                principalSchema: "BBK",
                principalTable: "GeoUnits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppCandidates_GeoUnits_DistrictId",
                schema: "BBKProfile",
                table: "AppCandidates");

            migrationBuilder.DropForeignKey(
                name: "FK_AppCandidates_GeoUnits_ProvinceId",
                schema: "BBKProfile",
                table: "AppCandidates");

            migrationBuilder.AlterColumn<long>(
                name: "ProvinceId",
                schema: "BBKProfile",
                table: "AppCandidates",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "DistrictId",
                schema: "BBKProfile",
                table: "AppCandidates",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppCandidates_GeoUnits_DistrictId",
                schema: "BBKProfile",
                table: "AppCandidates",
                column: "DistrictId",
                principalSchema: "BBK",
                principalTable: "GeoUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppCandidates_GeoUnits_ProvinceId",
                schema: "BBKProfile",
                table: "AppCandidates",
                column: "ProvinceId",
                principalSchema: "BBK",
                principalTable: "GeoUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
