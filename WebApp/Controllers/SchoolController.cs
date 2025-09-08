using App.DAL.EF;
using App.Domain;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers
{
    public class SchoolController(AppDbContext context, IAdminAccessService adminAccessService)
        : BaseController(adminAccessService)
    {
        // GET: CourseStatus
        public async Task<IActionResult> Index()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }

            return View(await context.Schools.IgnoreQueryFilters().ToListAsync());
        }

        // GET: CourseStatus/Details/5
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

            var schoolEntity = await context.Schools
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schoolEntity == null)
            {
                return NotFound();
            }

            return View(schoolEntity);
        }

        // GET: CourseStatus/Create
        public async Task<IActionResult> Create()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }

            return View();
        }

        // POST: CourseStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,ShortName,Domain,StudentCodePattern,Id,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,Deleted")]
            SchoolEntity schoolEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }

            if (ModelState.IsValid)
            {
                schoolEntity.Id = Guid.NewGuid();
                schoolEntity.CreatedAt = DateTime.UtcNow;
                schoolEntity.UpdatedAt = DateTime.UtcNow;
                context.Add(schoolEntity);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(schoolEntity);
        }

        // GET: CourseStatus/Edit/5
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

            var schoolEntity = await context.Schools.FindAsync(id);
            if (schoolEntity == null)
            {
                return NotFound();
            }

            return View(schoolEntity);
        }

        // POST: CourseStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id,
            [Bind("Name,ShortName,Domain,StudentCodePattern,Id,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,Deleted")]
            SchoolEntity schoolEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }

            if (id != schoolEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    schoolEntity.UpdatedAt = DateTime.UtcNow;
                    context.Update(schoolEntity);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException){}

                return RedirectToAction(nameof(Index));
            }

            return View(schoolEntity);
        }

        // GET: CourseStatus/Delete/5
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

            var schoolEntity = await context.Schools
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schoolEntity == null)
            {
                return NotFound();
            }

            return View(schoolEntity);
        }

        // POST: CourseStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }

            var schoolEntity = await context.Schools.FindAsync(id);
            if (schoolEntity != null)
            {
                context.Schools.Remove(schoolEntity);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}