using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShop.BL.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nchar(50)",
                fixedLength: true,
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nchar(50)",
                oldFixedLength: true,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "Residence",
                type: "nchar(100)",
                fixedLength: true,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(35)",
                oldFixedLength: true,
                oldMaxLength: 35,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "Residence",
                type: "nchar(100)",
                fixedLength: true,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(35)",
                oldFixedLength: true,
                oldMaxLength: 35,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Residence",
                type: "nchar(100)",
                fixedLength: true,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(35)",
                oldFixedLength: true,
                oldMaxLength: 35,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nchar(50)",
                fixedLength: true,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(50)",
                oldFixedLength: true,
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "Residence",
                type: "nchar(35)",
                fixedLength: true,
                maxLength: 35,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(100)",
                oldFixedLength: true,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "Residence",
                type: "nchar(35)",
                fixedLength: true,
                maxLength: 35,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(100)",
                oldFixedLength: true,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Residence",
                type: "nchar(35)",
                fixedLength: true,
                maxLength: 35,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(100)",
                oldFixedLength: true,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }
    }
}
