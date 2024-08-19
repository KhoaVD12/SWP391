using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRoleToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "User",
                type: "varchar(max)",
                unicode: false,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldUnicode: false);

            migrationBuilder.Sql(@"
                UPDATE [User] SET [Role] = 'Admin' WHERE [Role] = '0';
                UPDATE [User] SET [Role] = 'Organizer' WHERE [Role] = '1';
                UPDATE [User] SET [Role] = 'Staff' WHERE [Role] = '2';
                UPDATE [User] SET [Role] = 'Sponsor' WHERE [Role] = '3';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE [User] SET [Role] = '0' WHERE [Role] = 'Admin';
                UPDATE [User] SET [Role] = '1' WHERE [Role] = 'Organizer';
                UPDATE [User] SET [Role] = '2' WHERE [Role] = 'Staff';
                UPDATE [User] SET [Role] = '3' WHERE [Role] = 'Sponsor';
            ");

            // Step 2: Revert the Role column from string back to int
            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "User",
                type: "int",
                unicode: false,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false);
        }
    }
}
