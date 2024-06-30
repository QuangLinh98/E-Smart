using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Smart.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomerTB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Customer_name",
                table: "Customers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Customer_name",
                table: "Customers");
        }
    }
}
