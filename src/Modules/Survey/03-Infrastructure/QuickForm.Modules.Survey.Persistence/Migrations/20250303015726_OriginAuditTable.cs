﻿//<auto-generated/>
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickForm.Modules.Survey.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OriginAuditTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginClass",
                schema: "Survey",
                table: "Audit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginClass",
                schema: "Survey",
                table: "Audit");
        }
    }
}
