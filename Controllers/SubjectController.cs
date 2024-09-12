using DataAccessLayer.Entities;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CipherStudioTest.Controllers
{
    public class SubjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            ViewBag.LanguageList = new List<string> { "English", "French", "Spanish", "German" };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subject subject, string[] selectedLanguages)
        {
            try
            {
                if (selectedLanguages.Length == 0)
                {
                    ModelState.AddModelError("Language", "Please select at least one language.");
                    ViewBag.LanguageList = new List<string> { "English", "French", "Spanish", "German" };
                    return View(subject);
                }

                if (ModelState.IsValid)
                {
                    subject.SubjectLanguages = selectedLanguages.Select(lang => new SubjectLanguage { Language = lang }).ToList();
                    _context.Add(subject);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Subject data has been saved successfully!";

                    return RedirectToAction("Create", "Subject");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }
            return View(subject);
        }

        public IActionResult AssignTeacher()
        {
            ViewBag.Teachers = new SelectList(_context.Teachers, "Id", "Name");
            ViewBag.Subjects = new SelectList(_context.Subjects.Include(s => s.SubjectLanguages), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTeacher(int teacherId, int subjectId, string className)
        {
            try
            {
                var exists = _context.TeacherSubjects
                        .Any(ts => ts.TeacherId == teacherId && ts.SubjectId == subjectId && ts.Class == className);

                if (!exists)
                {
                    var teacherSubject = new TeacherSubject
                    {
                        TeacherId = teacherId,
                        SubjectId = subjectId,
                        Class = className
                    };

                    _context.Add(teacherSubject);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Teacher has been assigned successfully!";

                    return RedirectToAction("AssignTeacher", "Subject");
                }
                ModelState.AddModelError("", "This teacher is already assigned to this subject for this class.");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }
            return View();
        }

    }
}
