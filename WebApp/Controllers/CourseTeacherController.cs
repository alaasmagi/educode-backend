using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Contracts;

namespace WebApp.Controllers
{
    public class CourseTeacherController(AppDbContext context, IAdminAccessService adminAccessService)
        : BaseController(adminAccessService)
    {
        // GET: CourseTeacher
        public async Task<IActionResult> Index()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var appDbContext = context.CourseTeachers.Include(c => c.Course).Include(c => c.Teacher);
            return View(await appDbContext.IgnoreQueryFilters().ToListAsync());
        }

        // GET: CourseTeacher/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id == null)
            {
                return NotFound();
            }

            var courseTeacherEntity = await context.CourseTeachers
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
        public async Task<IActionResult> Create()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            ViewData["CourseId"] = new SelectList(context.Courses, "Id", "CourseCode");
            ViewData["TeacherId"] = new SelectList(context.Users, "Id", "UniId");
            return View();
        }

        // POST: CourseTeacher/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,TeacherId,Id,CreatedBy,UpdatedBy,Deleted")] CourseTeacherEntity courseTeacherEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (ModelState.IsValid)
            {
                courseTeacherEntity.UpdatedAt = DateTime.UtcNow;
                courseTeacherEntity.CreatedAt = DateTime.UtcNow;
                context.Add(courseTeacherEntity);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(context.Courses, "Id", "CourseCode", courseTeacherEntity.CourseId);
            ViewData["TeacherId"] = new SelectList(context.Users, "Id", "UniId", courseTeacherEntity.TeacherId);
            return View(courseTeacherEntity);
        }

        // GET: CourseTeacher/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id == null)
            {
                return NotFound();
            }

            var courseTeacherEntity = await context.CourseTeachers.FindAsync(id);
            if (courseTeacherEntity == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(context.Courses, "Id", "CourseCode", courseTeacherEntity.CourseId);
            ViewData["TeacherId"] = new SelectList(context.Users, "Id", "UniId", courseTeacherEntity.TeacherId);
            return View(courseTeacherEntity);
        }

        // POST: CourseTeacher/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CourseId,TeacherId,Id,CreatedBy,CreatedAt,UpdatedBy,Deleted")] CourseTeacherEntity courseTeacherEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
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
                    courseTeacherEntity.UpdatedAt = DateTime.UtcNow;
                    context.Update(courseTeacherEntity);
                    await context.SaveChangesAsync();
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
            ViewData["CourseId"] = new SelectList(context.Courses, "Id", "CourseCode", courseTeacherEntity.CourseId);
            ViewData["TeacherId"] = new SelectList(context.Users, "Id", "UniId", courseTeacherEntity.TeacherId);
            return View(courseTeacherEntity);
        }

        // GET: CourseTeacher/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id == null)
            {
                return NotFound();
            }

            var courseTeacherEntity = await context.CourseTeachers
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
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var courseTeacherEntity = await context.CourseTeachers.FindAsync(id);
            if (courseTeacherEntity != null)
            {
                context.CourseTeachers.Remove(courseTeacherEntity);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseTeacherEntityExists(Guid id)
        {
            return context.CourseTeachers.Any(e => e.Id == id);
        }
    }
}
