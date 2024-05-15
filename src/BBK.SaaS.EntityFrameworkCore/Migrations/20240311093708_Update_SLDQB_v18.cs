using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BBK.SaaS.Migrations
{
    /// <inheritdoc />
    public partial class Update_SLDQB_v18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppTradingSessions",
                schema: "BBKProfile",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    NameTrading = table.Column<string>(type: "text", nullable: true),
                    ProvinceId = table.Column<long>(type: "bigint", nullable: false),
                    DistrictId = table.Column<long>(type: "bigint", nullable: true),
                    VillageId = table.Column<long>(type: "bigint", nullable: true),
                    Address = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTradingSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTradingSessions_GeoUnits_DistrictId",
                        column: x => x.DistrictId,
                        principalSchema: "BBK",
                        principalTable: "GeoUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppTradingSessions_GeoUnits_ProvinceId",
                        column: x => x.ProvinceId,
                        principalSchema: "BBK",
                        principalTable: "GeoUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppTradingSessions_GeoUnits_VillageId",
                        column: x => x.VillageId,
                        principalSchema: "BBK",
                        principalTable: "GeoUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppTradingSessionAccounts",
                schema: "BBKProfile",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    UsertId = table.Column<long>(type: "bigint", nullable: false),
                    RecruiterId = table.Column<long>(type: "bigint", nullable: true),
                    CandidateId = table.Column<long>(type: "bigint", nullable: true),
                    TradingSessionId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTradingSessionAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTradingSessionAccounts_AppCandidates_CandidateId",
                        column: x => x.CandidateId,
                        principalSchema: "BBKProfile",
                        principalTable: "AppCandidates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppTradingSessionAccounts_AppRecruiters_RecruiterId",
                        column: x => x.RecruiterId,
                        principalSchema: "BBKProfile",
                        principalTable: "AppRecruiters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppTradingSessionAccounts_AppTradingSessions_TradingSession~",
                        column: x => x.TradingSessionId,
                        principalSchema: "BBKProfile",
                        principalTable: "AppTradingSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppTradingSessionAccounts_CandidateId",
                schema: "BBKProfile",
                table: "AppTradingSessionAccounts",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTradingSessionAccounts_RecruiterId",
                schema: "BBKProfile",
                table: "AppTradingSessionAccounts",
                column: "RecruiterId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTradingSessionAccounts_TenantId_TradingSessionId_Candida~",
                schema: "BBKProfile",
                table: "AppTradingSessionAccounts",
                columns: new[] { "TenantId", "TradingSessionId", "CandidateId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppTradingSessionAccounts_TenantId_TradingSessionId_Recruit~",
                schema: "BBKProfile",
                table: "AppTradingSessionAccounts",
                columns: new[] { "TenantId", "TradingSessionId", "RecruiterId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppTradingSessionAccounts_TradingSessionId",
                schema: "BBKProfile",
                table: "AppTradingSessionAccounts",
                column: "TradingSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTradingSessions_DistrictId",
                schema: "BBKProfile",
                table: "AppTradingSessions",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTradingSessions_ProvinceId",
                schema: "BBKProfile",
                table: "AppTradingSessions",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTradingSessions_TenantId_Id",
                schema: "BBKProfile",
                table: "AppTradingSessions",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_AppTradingSessions_VillageId",
                schema: "BBKProfile",
                table: "AppTradingSessions",
                column: "VillageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppTradingSessionAccounts",
                schema: "BBKProfile");

            migrationBuilder.DropTable(
                name: "AppTradingSessions",
                schema: "BBKProfile");
        }
    }
}
