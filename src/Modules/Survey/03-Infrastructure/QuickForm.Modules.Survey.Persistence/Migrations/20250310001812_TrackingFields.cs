﻿//<auto-generated/>
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickForm.Modules.Survey.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TrackingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OriginClass",
                schema: "Survey",
                table: "Audit",
                newName: "ClassOrigin");

            migrationBuilder.AddColumn<string>(
                name: "ClassOrigin",
                schema: "Survey",
                table: "outbox_messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TransactionId",
                schema: "Survey",
                table: "outbox_messages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Survey",
                table: "Forms",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Survey",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                schema: "Survey",
                table: "Customers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Survey",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                schema: "Survey",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                schema: "Survey",
                table: "Customers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassOrigin",
                schema: "Survey",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                schema: "Survey",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Survey",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "Survey",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Survey",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "Survey",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                schema: "Survey",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "ClassOrigin",
                schema: "Survey",
                table: "Audit",
                newName: "OriginClass");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Survey",
                table: "Forms",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }
    }
}
