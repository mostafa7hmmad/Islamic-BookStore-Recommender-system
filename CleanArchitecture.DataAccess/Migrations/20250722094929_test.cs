using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CleanArchitecture.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3ce09ae9-171e-4a6c-84e3-36fe6b71ed82");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "71ec0374-12dc-46f8-b3d1-23c9844f905c");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "BookCategories",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "07a5482a-3ba7-4219-9fe9-f5195d6b0bf3", "1", "Admin", "Admin" },
                    { "fef766f3-53c0-4157-a0be-74957eb61e45", "2", "User", "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "07a5482a-3ba7-4219-9fe9-f5195d6b0bf3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fef766f3-53c0-4157-a0be-74957eb61e45");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "BookCategories",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3ce09ae9-171e-4a6c-84e3-36fe6b71ed82", "1", "Admin", "Admin" },
                    { "71ec0374-12dc-46f8-b3d1-23c9844f905c", "2", "User", "User" }
                });
        }
    }
}
