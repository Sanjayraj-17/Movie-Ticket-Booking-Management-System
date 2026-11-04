using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTicketBooking.Migrations
{
    public partial class AddBookingUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add nullable UserId column
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Bookings",
                type: "int",
                nullable: true);

            // 2. Match existing bookings with registered users
            migrationBuilder.Sql(@"
                UPDATE Bookings
                SET UserId = Users.UserId
                FROM Bookings
                INNER JOIN Users
                    ON LOWER(LTRIM(RTRIM(Bookings.CustomerEmail))) =
                       LOWER(LTRIM(RTRIM(Users.Email)));
            ");

            // 3. Create index
            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            // 4. Create foreign key
            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bookings");
        }
    }
}