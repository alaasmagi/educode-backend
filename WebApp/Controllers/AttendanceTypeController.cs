using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Contracts;

namespace WebApp.Controllers
{
    public class AttendanceTypeController(AppDbContext context, IAdminAccessService adminAccessService)
        : BaseController(adminAccessService)
    {
        // GET: AttendanceType
        public async Task<IActionResult> Index()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            return View(await context.AttendanceTypes.ToListAsync());
        }

        // GET: AttendanceType/Details/5
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

            var attendanceTypeEntity = await context.AttendanceTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attendanceTypeEntity == null)
            {
                return NotFound();
            }

            return View(attendanceTypeEntity);
        }

        // GET: AttendanceType/Create
        public async Task<IActionResult> Create()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            return View();
        }

        // POST: AttendanceType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AttendanceType,Id,CreatedBy,UpdatedBy")] AttendanceTypeEntity attendanceTypeEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (ModelState.IsValid)
            {
                attendanceTypeEntity.UpdatedAt = DateTime.Now.ToUniversalTime();
                attendanceTypeEntity.CreatedAt = DateTime.Now.ToUniversalTime();
                context.Add(attendanceTypeEntity);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(attendanceTypeEntity);
        }

        // GET: AttendanceType/Edit/5
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

            var attendanceTypeEntity = await context.AttendanceTypes.FindAsync(id);
            if (attendanceTypeEntity == null)
            {
                return NotFound();
            }
            return View(attendanceTypeEntity);
        }

        // POST: AttendanceType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AttendanceType,Id,CreatedBy,CreatedAt,UpdatedBy")] AttendanceTypeEntity attendanceTypeEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id != attendanceTypeEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    attendanceTypeEntity.UpdatedAt = DateTime.Now.ToUniversalTime();
                    context.Update(attendanceTypeEntity);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendanceTypeEntityExists(attendanceTypeEntity.Id))
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
            return View(attendanceTypeEntity);
        }

        // GET: AttendanceType/Delete/5
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

            var attendanceTypeEntity = await context.AttendanceTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attendanceTypeEntity == null)
            {
                return NotFound();
            }

            return View(attendanceTypeEntity);
        }

        // POST: AttendanceType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var attendanceTypeEntity = await context.AttendanceTypes.FindAsync(id);
            if (attendanceTypeEntity != null)
            {
                context.AttendanceTypes.Remove(attendanceTypeEntity);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AttendanceTypeEntityExists(int id)
        {
            return context.AttendanceTypes.Any(e => e.Id == id);
        }
    }
}
