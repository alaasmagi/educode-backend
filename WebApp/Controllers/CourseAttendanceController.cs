using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Contracts;

namespace WebApp.Controllers
{
    public class CourseAttendanceController(AppDbContext context, IAdminAccessService adminAccessService)
        : BaseController(adminAccessService)
    {

        // GET: CourseAttendance
        public async Task<IActionResult> Index()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var appDbContext = context.CourseAttendances.Include(c => c.AttendanceType).Include(c => c.Course);
            return View(await appDbContext.IgnoreQueryFilters().ToListAsync());
        }

        // GET: CourseAttendance/Details/5
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

            var courseAttendanceEntity = await context.CourseAttendances
                .Include(c => c.AttendanceType)
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseAttendanceEntity == null)
            {
                return NotFound();
            }

            return View(courseAttendanceEntity);
        }

        // GET: CourseAttendance/Create
        public async Task<IActionResult> Create()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            ViewData["AttendanceTypeId"] = new SelectList(context.AttendanceTypes, "Id", "AttendanceType");
            ViewData["CourseId"] = new SelectList(context.Courses, "Id", "CourseCode");
            return View();
        }

        // POST: CourseAttendance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,AttendanceTypeId,StartTime,EndTime,Id,CreatedBy,UpdatedBy,Delete")] CourseAttendanceEntity courseAttendanceEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (ModelState.IsValid)
            {
                courseAttendanceEntity.UpdatedAt = DateTime.UtcNow;
                courseAttendanceEntity.CreatedAt = DateTime.UtcNow;
                context.Add(courseAttendanceEntity);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AttendanceTypeId"] = new SelectList(context.AttendanceTypes, "Id", "AttendanceType", courseAttendanceEntity.AttendanceTypeId);
            ViewData["CourseId"] = new SelectList(context.Courses, "Id", "CourseCode", courseAttendanceEntity.CourseId);
            return View(courseAttendanceEntity);
        }

        // GET: CourseAttendance/Edit/5
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

            var courseAttendanceEntity = await context.CourseAttendances.FindAsync(id);
            if (courseAttendanceEntity == null)
            {
                return NotFound();
            }
            ViewData["AttendanceTypeId"] = new SelectList(context.AttendanceTypes, "Id", "AttendanceType", courseAttendanceEntity.AttendanceTypeId);
            ViewData["CourseId"] = new SelectList(context.Courses, "Id", "CourseCode", courseAttendanceEntity.CourseId);
            return View(courseAttendanceEntity);
        }

        // POST: CourseAttendance/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CourseId,AttendanceTypeId,StartTime,EndTime,Id,CreatedBy,CreatedAt,UpdatedBy,Deleted")] CourseAttendanceEntity courseAttendanceEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id != courseAttendanceEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    courseAttendanceEntity.UpdatedAt = DateTime.UtcNow;
                    context.Update(courseAttendanceEntity);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseAttendanceEntityExists(courseAttendanceEntity.Id))
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
            ViewData["AttendanceTypeId"] = new SelectList(context.AttendanceTypes, "Id", "AttendanceType", courseAttendanceEntity.AttendanceTypeId);
            ViewData["CourseId"] = new SelectList(context.Courses, "Id", "CourseCode", courseAttendanceEntity.CourseId);
            return View(courseAttendanceEntity);
        }

        // GET: CourseAttendance/Delete/5
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

            var courseAttendanceEntity = await context.CourseAttendances
                .Include(c => c.AttendanceType)
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseAttendanceEntity == null)
            {
                return NotFound();
            }

            return View(courseAttendanceEntity);
        }

        // POST: CourseAttendance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var courseAttendanceEntity = await context.CourseAttendances.FindAsync(id);
            if (courseAttendanceEntity != null)
            {
                context.CourseAttendances.Remove(courseAttendanceEntity);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseAttendanceEntityExists(Guid id)
        {
            return context.CourseAttendances.Any(e => e.Id == id);
        }
    }
}
