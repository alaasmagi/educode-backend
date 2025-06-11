using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DAL.EF.Migrations
{
    /// <inheritdoc />
    public partial class DbMigration_v8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "educode");

            migrationBuilder.CreateTable(
                name: "AttendanceTypes",
                schema: "educode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttendanceType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseStatuses",
                schema: "educode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseStatus = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTypes",
                schema: "educode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workplaces",
                schema: "educode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Identifier = table.Column<int>(type: "integer", nullable: false),
                    ClassRoom = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ComputerCode = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workplaces", x => x.Id);
                    table.UniqueConstraint("AK_Workplaces_Identifier", x => x.Identifier);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                schema: "educode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseCode = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CourseName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CourseStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_CourseStatuses_CourseStatusId",
                        column: x => x.CourseStatusId,
                        principalSchema: "educode",
                        principalTable: "CourseStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "educode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    UniId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    StudentCode = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    FullName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_UserTypes_UserTypeId",
                        column: x => x.UserTypeId,
                        principalSchema: "educode",
                        principalTable: "UserTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseAttendances",
                schema: "educode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Identifier = table.Column<int>(type: "integer", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttendanceTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseAttendances", x => x.Id);
                    table.UniqueConstraint("AK_CourseAttendances_Identifier", x => x.Identifier);
                    table.ForeignKey(
                        name: "FK_CourseAttendances_AttendanceTypes_AttendanceTypeId",
                        column: x => x.AttendanceTypeId,
                        principalSchema: "educode",
                        principalTable: "AttendanceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseAttendances_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "educode",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseTeachers",
                schema: "educode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTeachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseTeachers_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "educode",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseTeachers_Users_TeacherId",
                        column: x => x.TeacherId,
                        principalSchema: "educode",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "educode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    ReplacedByTokenId = table.Column<Guid>(type: "uuid", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RevokedByIp = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreatedByIp = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "educode",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAuthData",
                schema: "educode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAuthData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAuthData_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "educode",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceChecks",
                schema: "educode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentCode = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AttendanceIdentifier = table.Column<int>(type: "integer", nullable: false),
                    WorkplaceIdentifier = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceChecks_CourseAttendances_AttendanceIdentifier",
                        column: x => x.AttendanceIdentifier,
                        principalSchema: "educode",
                        principalTable: "CourseAttendances",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceChecks_Workplaces_WorkplaceIdentifier",
                        column: x => x.WorkplaceIdentifier,
                        principalSchema: "educode",
                        principalTable: "Workplaces",
                        principalColumn: "Identifier");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceChecks_AttendanceIdentifier",
                schema: "educode",
                table: "AttendanceChecks",
                column: "AttendanceIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceChecks_StudentCode_AttendanceIdentifier",
                schema: "educode",
                table: "AttendanceChecks",
                columns: new[] { "StudentCode", "AttendanceIdentifier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceChecks_WorkplaceIdentifier",
                schema: "educode",
                table: "AttendanceChecks",
                column: "WorkplaceIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceTypes_AttendanceType",
                schema: "educode",
                table: "AttendanceTypes",
                column: "AttendanceType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseAttendances_AttendanceTypeId",
                schema: "educode",
                table: "CourseAttendances",
                column: "AttendanceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAttendances_CourseId",
                schema: "educode",
                table: "CourseAttendances",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAttendances_Identifier",
                schema: "educode",
                table: "CourseAttendances",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CourseCode",
                schema: "educode",
                table: "Courses",
                column: "CourseCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CourseStatusId",
                schema: "educode",
                table: "Courses",
                column: "CourseStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseStatuses_CourseStatus",
                schema: "educode",
                table: "CourseStatuses",
                column: "CourseStatus",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeachers_CourseId",
                schema: "educode",
                table: "CourseTeachers",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeachers_TeacherId",
                schema: "educode",
                table: "CourseTeachers",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                schema: "educode",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "educode",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAuthData_UserId",
                schema: "educode",
                table: "UserAuthData",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_FullName",
                schema: "educode",
                table: "Users",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StudentCode",
                schema: "educode",
                table: "Users",
                column: "StudentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UniId",
                schema: "educode",
                table: "Users",
                column: "UniId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserTypeId",
                schema: "educode",
                table: "Users",
                column: "UserTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTypes_UserType",
                schema: "educode",
                table: "UserTypes",
                column: "UserType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workplaces_Identifier",
                schema: "educode",
                table: "Workplaces",
                column: "Identifier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceChecks",
                schema: "educode");

            migrationBuilder.DropTable(
                name: "CourseTeachers",
                schema: "educode");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "educode");

            migrationBuilder.DropTable(
                name: "UserAuthData",
                schema: "educode");

            migrationBuilder.DropTable(
                name: "CourseAttendances",
                schema: "educode");

            migrationBuilder.DropTable(
                name: "Workplaces",
                schema: "educode");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "educode");

            migrationBuilder.DropTable(
                name: "AttendanceTypes",
                schema: "educode");

            migrationBuilder.DropTable(
                name: "Courses",
                schema: "educode");

            migrationBuilder.DropTable(
                name: "UserTypes",
                schema: "educode");

            migrationBuilder.DropTable(
                name: "CourseStatuses",
                schema: "educode");
        }
    }
}
