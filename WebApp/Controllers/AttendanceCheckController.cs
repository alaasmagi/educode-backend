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
            return View(await appDbContext.IgnoreQueryFilters().ToListAsync());
        }

        // GET: AttendanceCheck/Details/ID
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
        public async Task<IActionResult> Create([Bind("StudentId,FullName,AttendanceIdentifier,WorkplaceIdentifier,Id,CreatedBy,UpdatedBy,Deleted")] AttendanceCheckEntity attendanceCheckEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (ModelState.IsValid)
            {
                attendanceCheckEntity.UpdatedAt = DateTime.UtcNow;
                attendanceCheckEntity.CreatedAt = DateTime.UtcNow;
                context.Add(attendanceCheckEntity);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WorkplaceId"] = new SelectList(context.Workplaces, "Id", "ClassRoom", attendanceCheckEntity.WorkplaceIdentifier);
            return View(attendanceCheckEntity);
        }

        // GET: AttendanceCheck/Edit/5
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

            var attendanceCheckEntity = await context.AttendanceChecks.FindAsync(id);
            if (attendanceCheckEntity == null)
            {
                return NotFound();
            }
            ViewData["WorkplaceIdentifier"] = new SelectList(context.Workplaces, "Id", "ClassRoom", attendanceCheckEntity.WorkplaceIdentifier);
            ViewData["AttendanceIdentifier"] = new SelectList(context.CourseAttendances, "Id", "Id", attendanceCheckEntity.AttendanceIdentifier);
            return View(attendanceCheckEntity);
        }

        // POST: AttendanceCheck/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("StudentId,FullName,AttendanceIdentifier,WorkplaceIdentifier,Id,CreatedBy,CreatedAt,UpdatedBy,Deleted")] AttendanceCheckEntity attendanceCheckEntity)
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
                    attendanceCheckEntity.UpdatedAt = DateTime.UtcNow;
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
            ViewData["WorkplaceId"] = new SelectList(context.Workplaces, "Id", "ClassRoom", attendanceCheckEntity.WorkplaceIdentifier);
            return View(attendanceCheckEntity);
        }

        // GET: AttendanceCheck/Delete/5
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
        public async Task<IActionResult> DeleteConfirmed(Guid id)
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

        private bool AttendanceCheckEntityExists(Guid id)
        {
            return context.AttendanceChecks.Any(e => e.Id == id);
        }
    }
}
