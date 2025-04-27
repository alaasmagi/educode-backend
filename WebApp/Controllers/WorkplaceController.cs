using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Contracts;

namespace WebApp.Controllers
{
    public class WorkplaceController(AppDbContext context, IAdminAccessService adminAccessService)
        : BaseController(adminAccessService)
    {
        // GET: Workplace
        public async Task<IActionResult> Index()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            return View(await context.Workplaces.ToListAsync());
        }

        // GET: Workplace/Details/5
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

            var workplaceEntity = await context.Workplaces
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workplaceEntity == null)
            {
                return NotFound();
            }

            return View(workplaceEntity);
        }

        // GET: Workplace/Create
        public async Task<IActionResult> Create()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            return View();
        }

        // POST: Workplace/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClassRoom,ComputerCode,Id,CreatedBy,UpdatedBy,Deleted")] WorkplaceEntity workplaceEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (ModelState.IsValid)
            {
                workplaceEntity.UpdatedAt = DateTime.Now.ToUniversalTime();
                workplaceEntity.CreatedAt = DateTime.Now.ToUniversalTime();
                context.Add(workplaceEntity);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workplaceEntity);
        }

        // GET: Workplace/Edit/5
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

            var workplaceEntity = await context.Workplaces.FindAsync(id);
            if (workplaceEntity == null)
            {
                return NotFound();
            }
            return View(workplaceEntity);
        }

        // POST: Workplace/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClassRoom,ComputerCode,Id,CreatedBy,CreatedAt,UpdatedBy,Deleted")] WorkplaceEntity workplaceEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id != workplaceEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    workplaceEntity.UpdatedAt = DateTime.Now.ToUniversalTime();
                    context.Update(workplaceEntity);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkplaceEntityExists(workplaceEntity.Id))
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
            return View(workplaceEntity);
        }

        // GET: Workplace/Delete/5
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

            var workplaceEntity = await context.Workplaces
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workplaceEntity == null)
            {
                return NotFound();
            }

            return View(workplaceEntity);
        }

        // POST: Workplace/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var workplaceEntity = await context.Workplaces.FindAsync(id);
            if (workplaceEntity != null)
            {
                context.Workplaces.Remove(workplaceEntity);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkplaceEntityExists(int id)
        {
            return context.Workplaces.Any(e => e.Id == id);
        }
    }
}
