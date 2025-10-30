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
    public class AssignmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AssignmentsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssignmentDTO>>> GetAssignments()
        {
            var assignments = await _context.Assignments.ToListAsync();
            return Ok(_mapper.Map<List<AssignmentDTO>>(assignments));
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<AssignmentDTO>> GetAssignment(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);

            if (assignment == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AssignmentDTO>(assignment));
        }

        
        [HttpGet("{id}/submissions")]
        public async Task<ActionResult<IEnumerable<SubmissionDTO>>> GetAssignmentSubmissions(int id)
        {
            var submissions = await _context.AssignmentSubmissions
                .Include(s => s.Student)
                .Where(s => s.AssignmentId == id)
                .ToListAsync();

            return Ok(_mapper.Map<List<SubmissionDTO>>(submissions));
        }

        
        [HttpPost]
        public async Task<ActionResult<AssignmentDTO>> CreateAssignment(CreateAssignmentDTO dto)
        {
            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null)
            {
                return NotFound("Course not found");
            }

            var assignment = _mapper.Map<Assignment>(dto);
            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            var assignmentDto = _mapper.Map<AssignmentDTO>(assignment);
            return CreatedAtAction(nameof(GetAssignment), new { id = assignment.AssignmentId }, assignmentDto);
        }

        
        [HttpPost("submit")]
        public async Task<ActionResult<SubmissionDTO>> SubmitAssignment(CreateSubmissionDTO dto)
        {
            var student = await _context.Students.FindAsync(dto.StudentId);
            if (student == null)
            {
                return NotFound("Student not found");
            }

            var assignment = await _context.Assignments.FindAsync(dto.AssignmentId);
            if (assignment == null)
            {
                return NotFound("Assignment not found");
            }

            
            var exists = await _context.AssignmentSubmissions
                .AnyAsync(s => s.StudentId == dto.StudentId && s.AssignmentId == dto.AssignmentId);

            if (exists)
            {
                return BadRequest("Assignment already submitted");
            }

            var submission = _mapper.Map<AssignmentSubmission>(dto);
            _context.AssignmentSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            var submissionDto = await _context.AssignmentSubmissions
                .Include(s => s.Student)
                .FirstOrDefaultAsync(s => s.SubmissionId == submission.SubmissionId);

            return Ok(_mapper.Map<SubmissionDTO>(submissionDto));
        }

      
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssignment(int id, CreateAssignmentDTO dto)
        {
            var assignment = await _context.Assignments.FindAsync(id);

            if (assignment == null)
            {
                return NotFound();
            }

            assignment.Title = dto.Title;
            assignment.Description = dto.Description;
            assignment.DueDate = dto.DueDate;
            assignment.CourseId = dto.CourseId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

     
        [HttpPut("submissions/{id}/grade")]
        public async Task<IActionResult> GradeSubmission(int id, [FromBody] GradeSubmissionDTO dto)
        {
            var submission = await _context.AssignmentSubmissions.FindAsync(id);

            if (submission == null)
                return NotFound("Submission not found.");

            submission.Grade = dto.Grade;
            submission.Comments = dto.Comments;
            submission.Status = "Graded";
           

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Submission graded successfully",
               
                submission.Grade,
                submission.Comments,
                submission.Status,
               
            });
        }


        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class GradeSubmissionDTO
    {
        public int Grade { get; set; }
        public string? Comments { get; set; }
    }
}