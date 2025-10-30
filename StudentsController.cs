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
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public StudentsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudents()
        {
            var students = await _context.Students.ToListAsync();
            return Ok(_mapper.Map<List<StudentDTO>>(students));
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDTO>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<StudentDTO>(student));
        }

        
        [HttpGet("{id}/courses")]
        public async Task<ActionResult<StudentWithCoursesDTO>> GetStudentCourses(int id)
        {
            var student = await _context.Students
                .Include(s => s.StudentCourses)
                .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<StudentWithCoursesDTO>(student));
        }

       
        [HttpPost]
        public async Task<ActionResult<StudentDTO>> CreateStudent(CreateStudentDTO dto)
        {
            var student = _mapper.Map<Student>(dto);
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            var studentDto = _mapper.Map<StudentDTO>(student);
            return CreatedAtAction(nameof(GetStudent), new { id = student.StudentId }, studentDto);
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, CreateStudentDTO dto)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            student.Name = dto.Name;
            student.Email = dto.Email;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}