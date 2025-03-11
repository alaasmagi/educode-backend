using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;

namespace WebApp.Controllers
{
    public class CourseTeacherController : BaseController
    {
        private readonly AppDbContext _context;

        public CourseTeacherController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CourseTeacher
        public async Task<IActionResult> Index()
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var appDbContext = _context.CourseTeachers.Include(c => c.Course).Include(c => c.Teacher);
            return View(await appDbContext.ToListAsync());
        }

        // GET: CourseTeacher/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id == null)
            {
                return NotFound();
            }

            var courseTeacherEntity = await _context.CourseTeachers
                .Include(c => c.Course)
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseTeacherEntity == null)
            {
                return NotFound();
            }

            return View(courseTeacherEntity);
        }

        // GET: CourseTeacher/Create
        public IActionResult Create()
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "CourseCode");
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "UniId");
            return View();
        }

        // POST: CourseTeacher/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,TeacherId,Id,CreatedBy,UpdatedBy")] CourseTeacherEntity courseTeacherEntity)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (ModelState.IsValid)
            {
                courseTeacherEntity.UpdatedAt = DateTime.Now;
                courseTeacherEntity.CreatedAt = DateTime.Now;
                _context.Add(courseTeacherEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "CourseCode", courseTeacherEntity.CourseId);
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "UniId", courseTeacherEntity.TeacherId);
            return View(courseTeacherEntity);
        }

        // GET: CourseTeacher/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id == null)
            {
                return NotFound();
            }

            var courseTeacherEntity = await _context.CourseTeachers.FindAsync(id);
            if (courseTeacherEntity == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "CourseCode", courseTeacherEntity.CourseId);
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "UniId", courseTeacherEntity.TeacherId);
            return View(courseTeacherEntity);
        }

        // POST: CourseTeacher/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,TeacherId,Id,CreatedBy,CreatedAt,UpdatedBy")] CourseTeacherEntity courseTeacherEntity)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id != courseTeacherEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    courseTeacherEntity.UpdatedAt = DateTime.Now;
                    _context.Update(courseTeacherEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseTeacherEntityExists(courseTeacherEntity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "CourseCode", courseTeacherEntity.CourseId);
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "UniId", courseTeacherEntity.TeacherId);
            return View(courseTeacherEntity);
        }

        // GET: CourseTeacher/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id == null)
            {
                return NotFound();
            }

            var courseTeacherEntity = await _context.CourseTeachers
                .Include(c => c.Course)
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseTeacherEntity == null)
            {
                return NotFound();
            }

            return View(courseTeacherEntity);
        }

        // POST: CourseTeacher/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var courseTeacherEntity = await _context.CourseTeachers.FindAsync(id);
            if (courseTeacherEntity != null)
            {
                _context.CourseTeachers.Remove(courseTeacherEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseTeacherEntityExists(int id)
        {
            return _context.CourseTeachers.Any(e => e.Id == id);
        }
    }
}
