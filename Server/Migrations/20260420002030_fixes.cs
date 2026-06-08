using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class fixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSchedules_Courses_CourseId",
                table: "CourseSchedules");

            migrationBuilder.CreateTable(
                name: "RoomDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomDto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolClassDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolClassDto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeacherDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherDto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    SchoolClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SchoolClassDtoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentDto_SchoolClassDto_SchoolClassDtoId",
                        column: x => x.SchoolClassDtoId,
                        principalTable: "SchoolClassDto",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubjectDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeacherDtoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectDto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectDto_TeacherDto_TeacherDtoId",
                        column: x => x.TeacherDtoId,
                        principalTable: "TeacherDto",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CourseDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseDto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseDto_SchoolClassDto_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "SchoolClassDto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseDto_SubjectDto_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "SubjectDto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseDto_TeacherDto_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "TeacherDto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GradeDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StudentDtoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradeDto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradeDto_StudentDto_StudentDtoId",
                        column: x => x.StudentDtoId,
                        principalTable: "StudentDto",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GradeDto_SubjectDto_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "SubjectDto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseScheduleDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    LessonNumber = table.Column<int>(type: "int", nullable: false),
                    RoomId1 = table.Column<int>(type: "int", nullable: false),
                    CourseId1 = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: true),
                    RoomId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseScheduleDto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseScheduleDto_CourseDto_CourseId1",
                        column: x => x.CourseId1,
                        principalTable: "CourseDto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseScheduleDto_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourseScheduleDto_RoomDto_RoomId1",
                        column: x => x.RoomId1,
                        principalTable: "RoomDto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseScheduleDto_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseDto_SchoolClassId",
                table: "CourseDto",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseDto_SubjectId",
                table: "CourseDto",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseDto_TeacherId",
                table: "CourseDto",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseScheduleDto_CourseId",
                table: "CourseScheduleDto",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseScheduleDto_CourseId1",
                table: "CourseScheduleDto",
                column: "CourseId1");

            migrationBuilder.CreateIndex(
                name: "IX_CourseScheduleDto_RoomId",
                table: "CourseScheduleDto",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseScheduleDto_RoomId1",
                table: "CourseScheduleDto",
                column: "RoomId1");

            migrationBuilder.CreateIndex(
                name: "IX_GradeDto_StudentDtoId",
                table: "GradeDto",
                column: "StudentDtoId");

            migrationBuilder.CreateIndex(
                name: "IX_GradeDto_SubjectId",
                table: "GradeDto",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDto_SchoolClassDtoId",
                table: "StudentDto",
                column: "SchoolClassDtoId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectDto_TeacherDtoId",
                table: "SubjectDto",
                column: "TeacherDtoId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSchedules_CourseDto_CourseId",
                table: "CourseSchedules",
                column: "CourseId",
                principalTable: "CourseDto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSchedules_CourseDto_CourseId",
                table: "CourseSchedules");

            migrationBuilder.DropTable(
                name: "CourseScheduleDto");

            migrationBuilder.DropTable(
                name: "GradeDto");

            migrationBuilder.DropTable(
                name: "CourseDto");

            migrationBuilder.DropTable(
                name: "RoomDto");

            migrationBuilder.DropTable(
                name: "StudentDto");

            migrationBuilder.DropTable(
                name: "SubjectDto");

            migrationBuilder.DropTable(
                name: "SchoolClassDto");

            migrationBuilder.DropTable(
                name: "TeacherDto");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSchedules_Courses_CourseId",
                table: "CourseSchedules",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
