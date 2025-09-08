using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Contracts;

namespace WebApp.Controllers
{
    public class UserTypeController(AppDbContext context, IAdminAccessService adminAccessService)
        : BaseController(adminAccessService)
    {

        // GET: UserType
        public async Task<IActionResult> Index()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            return View(await context.UserTypes.IgnoreQueryFilters().ToListAsync());
        }

        // GET: UserType/Details/5
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

            var userTypeEntity = await context.UserTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userTypeEntity == null)
            {
                return NotFound();
            }

            return View(userTypeEntity);
        }

        // GET: UserType/Create
        public async Task<IActionResult> Create()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            return View();
        }

        // POST: UserType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserType,CreatedBy,UpdatedBy,Deleted")] UserTypeEntity userTypeEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (ModelState.IsValid)
            {
                userTypeEntity.UpdatedAt = DateTime.UtcNow;
                userTypeEntity.CreatedAt = DateTime.UtcNow;
                context.Add(userTypeEntity);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userTypeEntity);
        }

        // GET: UserType/Edit/5
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

            var userTypeEntity = await context.UserTypes.FindAsync(id);
            if (userTypeEntity == null)
            {
                return NotFound();
            }
            return View(userTypeEntity);
        }

        // POST: UserType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserType,Id,CreatedBy,CreatedAt,UpdatedBy,Deleted")] UserTypeEntity userTypeEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id != userTypeEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    userTypeEntity.CreatedAt = DateTime.SpecifyKind(userTypeEntity.CreatedAt, DateTimeKind.Utc);
                    userTypeEntity.UpdatedAt = DateTime.UtcNow;
                    context.Update(userTypeEntity);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserTypeEntityExists(userTypeEntity.Id))
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
            return View(userTypeEntity);
        }

        // GET: UserType/Delete/5
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

            var userTypeEntity = await context.UserTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userTypeEntity == null)
            {
                return NotFound();
            }

            return View(userTypeEntity);
        }

        // POST: UserType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var userTypeEntity = await context.UserTypes.FindAsync(id);
            if (userTypeEntity != null)
            {
                context.UserTypes.Remove(userTypeEntity);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserTypeEntityExists(Guid id)
        {
            return context.UserTypes.Any(e => e.Id == id);
        }
    }
}
