﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taxi_App.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserEntityBusyPropAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Busy",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Busy",
                table: "AspNetUsers");
        }
    }
}
