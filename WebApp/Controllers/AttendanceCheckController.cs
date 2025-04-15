using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Contracts;

namespace WebApp.Controllers
{
    public class AttendanceCheckController(AppDbContext context, IAdminAccessService adminAccessService)
        : BaseController(adminAccessService)
    {
        // GET: AttendanceCheck
        public async Task<IActionResult> Index()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var appDbContext = context.AttendanceChecks.Include(a => a.Workplace);
            return View(await appDbContext.ToListAsync());
        }

        // GET: AttendanceCheck/Details/5
        public async Task<IActionResult> Details(int? id)
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

            var attendanceCheckEntity = await context.AttendanceChecks
                .Include(a => a.Workplace)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attendanceCheckEntity == null)
            {
                return NotFound();
            }

            return View(attendanceCheckEntity);
        }

        // GET: AttendanceCheck/Create
        public async Task<IActionResult> Create()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            ViewData["WorkplaceId"] = new SelectList(context.Workplaces, "Id", "ClassRoom");
            ViewData["CourseAttendanceId"] = new SelectList(context.CourseAttendances, "Id", "Id");
            return View();
        }

        // POST: AttendanceCheck/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,FullName,CourseAttendanceId,WorkplaceId,Id,CreatedBy,UpdatedBy")] AttendanceCheckEntity attendanceCheckEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (ModelState.IsValid)
            {
                attendanceCheckEntity.UpdatedAt = DateTime.Now.ToUniversalTime();
                attendanceCheckEntity.CreatedAt = DateTime.Now.ToUniversalTime();
                context.Add(attendanceCheckEntity);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WorkplaceId"] = new SelectList(context.Workplaces, "Id", "ClassRoom", attendanceCheckEntity.WorkplaceId);
            return View(attendanceCheckEntity);
        }

        // GET: AttendanceCheck/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            var attendanceCheckEntity = await context.AttendanceChecks.FindAsync(id);
            if (attendanceCheckEntity == null)
            {
                return NotFound();
            }
            ViewData["WorkplaceId"] = new SelectList(context.Workplaces, "Id", "ClassRoom", attendanceCheckEntity.WorkplaceId);
            ViewData["CourseAttendanceId"] = new SelectList(context.CourseAttendances, "Id", "Id", attendanceCheckEntity.CourseAttendanceId);
            return View(attendanceCheckEntity);
        }

        // POST: AttendanceCheck/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,FullName,CourseAttendanceId,WorkplaceId,Id,CreatedBy,CreatedAt,UpdatedBy")] AttendanceCheckEntity attendanceCheckEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id != attendanceCheckEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    attendanceCheckEntity.UpdatedAt = DateTime.Now.ToUniversalTime();
                    context.Update(attendanceCheckEntity);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendanceCheckEntityExists(attendanceCheckEntity.Id))
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
            ViewData["WorkplaceId"] = new SelectList(context.Workplaces, "Id", "ClassRoom", attendanceCheckEntity.WorkplaceId);
            return View(attendanceCheckEntity);
        }

        // GET: AttendanceCheck/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            var attendanceCheckEntity = await context.AttendanceChecks
                .Include(a => a.Workplace)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attendanceCheckEntity == null)
            {
                return NotFound();
            }

            return View(attendanceCheckEntity);
        }

        // POST: AttendanceCheck/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var attendanceCheckEntity = await context.AttendanceChecks.FindAsync(id);
            if (attendanceCheckEntity != null)
            {
                context.AttendanceChecks.Remove(attendanceCheckEntity);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AttendanceCheckEntityExists(int id)
        {
            return context.AttendanceChecks.Any(e => e.Id == id);
        }
    }
}
