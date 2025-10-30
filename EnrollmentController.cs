using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentAPI.Data;
using StudentEnrollmentAPI.DTOs;
using StudentEnrollmentAPI.Models;

namespace StudentEnrollmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpPost]
        public async Task<ActionResult<EnrollmentResponseDTO>> EnrollStudent(EnrollmentDTO dto)
        {
            
            var student = await _context.Students.FindAsync(dto.StudentId);
            if (student == null)
            {
                return NotFound("Student not found");
            }

            
            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null)
            {
                return NotFound("Course not found");
            }

           
            var exists = await _context.StudentCourses
                .AnyAsync(sc => sc.StudentId == dto.StudentId && sc.CourseId == dto.CourseId);

            if (exists)
            {
                return BadRequest("Student is already enrolled in this course");
            }

            
            var enrollment = new StudentCourse
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                EnrolledDate = DateTime.Now
            };

            _context.StudentCourses.Add(enrollment);
            await _context.SaveChangesAsync();

            var response = new EnrollmentResponseDTO
            {
                StudentId = student.StudentId,
                StudentName = student.Name,
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                EnrolledDate = enrollment.EnrolledDate
            };

            return Ok(response);
        }

        
        [HttpDelete]
        public async Task<IActionResult> UnenrollStudent(EnrollmentDTO dto)
        {
            var enrollment = await _context.StudentCourses
                .FirstOrDefaultAsync(sc => sc.StudentId == dto.StudentId && sc.CourseId == dto.CourseId);

            if (enrollment == null)
            {
                return NotFound("Enrollment not found");
            }

            _context.StudentCourses.Remove(enrollment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}