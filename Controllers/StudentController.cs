using DataAccessLayer.Entities;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CipherStudioTest.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly FileUploadService _fileUploadService;

        public StudentController(ApplicationDbContext context, FileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student, IFormFile ImageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    student.ImagePath = await _fileUploadService.UploadFileAsync(ImageFile, "images");
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Student data has been saved successfully!";
                    return RedirectToAction(nameof(Create));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }
            return View(student);
        }

        public async Task<IActionResult> List(string searchString)
        {
            var students = from s in _context.Students select s;

            if (searchString != null)
            {
                students = students.Where(s => s.Name.Contains(searchString));
            }

            var groupedStudents = await students.OrderBy(s => s.Class).ToListAsync();
            return View(groupedStudents);
        }

        public async Task<IActionResult> SelectStudent()
        {
            var students = await _context.Students.ToListAsync();
            return View(students);
        }

        public async Task<IActionResult> StudentDetails(int studentId)
        {
            var student = await _context.Students.FindAsync(studentId);

            if (student == null)
            {
                return NotFound();
            }

            var subjects = await _context.Subjects
                .Where(s => s.Class == student.Class)
                .Include(s => s.TeacherSubjects)
                    .ThenInclude(ts => ts.Teachers)
                .Include(s => s.SubjectLanguages)
                .ToListAsync();

            var model = new StudentSubjectsViewModel
            {
                Student = student,
                Subjects = subjects
            };

            return View(model);
        }
    }
}
