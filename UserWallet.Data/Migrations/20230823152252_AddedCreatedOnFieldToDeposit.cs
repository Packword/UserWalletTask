﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserWallet.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedCreatedOnFieldToDeposit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_on",
                table: "deposits",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_on",
                table: "deposits");
        }
    }
}
