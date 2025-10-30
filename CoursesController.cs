using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentAPI.Data;
using StudentEnrollmentAPI.DTOs;
using StudentEnrollmentAPI.Models;

namespace StudentEnrollmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CoursesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetCourses()
        {
            var courses = await _context.Courses.ToListAsync();
            return Ok(_mapper.Map<List<CourseDTO>>(courses));
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDTO>> GetCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CourseDTO>(course));
        }

        
        [HttpGet("{id}/students")]
        public async Task<ActionResult<CourseWithStudentsDTO>> GetCourseStudents(int id)
        {
            var course = await _context.Courses
                .Include(c => c.StudentCourses)
                .ThenInclude(sc => sc.Student)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CourseWithStudentsDTO>(course));
        }

        
        [HttpPost]
        public async Task<ActionResult<CourseDTO>> CreateCourse(CreateCourseDTO dto)
        {
            var course = _mapper.Map<Course>(dto);
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var courseDto = _mapper.Map<CourseDTO>(course);
            return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, courseDto);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, CreateCourseDTO dto)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            course.CourseName = dto.CourseName;
            course.CourseCode = dto.CourseCode;
            course.Credits = dto.Credits;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}