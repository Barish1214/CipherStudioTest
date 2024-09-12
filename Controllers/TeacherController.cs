using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CipherStudioTest.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly FileUploadService _fileUploadService;

        public TeacherController(ApplicationDbContext context,FileUploadService fileUploadService)
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
        public async Task<IActionResult> Create(Teacher teacher, IFormFile ImageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    teacher.ImagePath = await _fileUploadService.UploadFileAsync(ImageFile, "images");
                    _context.Add(teacher);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Teacher data has been saved successfully!";

                    return RedirectToAction(nameof(Create));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }
            return View(teacher);
        }
    }
}
