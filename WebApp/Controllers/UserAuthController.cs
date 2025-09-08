using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Contracts;

namespace WebApp.Controllers
{
    public class UserAuthController(AppDbContext context, IAdminAccessService adminAccessService)
        : BaseController(adminAccessService)
    {
        // GET: UserAuth
        public async Task<IActionResult> Index()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var appDbContext = context.UserAuthData.Include(u => u.User);
            return View(await appDbContext.IgnoreQueryFilters().ToListAsync());
        }

        // GET: UserAuth/Details/5
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

            var userAuthEntity = await context.UserAuthData
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAuthEntity == null)
            {
                return NotFound();
            }

            return View(userAuthEntity);
        }

        // GET: UserAuth/Create
        public async Task<IActionResult> Create()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            ViewData["UserId"] = new SelectList(context.Users, "Id", "Email");
            return View();
        }

        // POST: UserAuth/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,PasswordHash,Id,CreatedBy,UpdatedBy,Deleted")] UserAuthEntity userAuthEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (ModelState.IsValid)
            {
                userAuthEntity.UpdatedAt = DateTime.UtcNow;
                userAuthEntity.CreatedAt = DateTime.UtcNow;
                context.Add(userAuthEntity);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(context.Users, "Id", "Email", userAuthEntity.UserId);
            return View(userAuthEntity);
        }

        // GET: UserAuth/Edit/5
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

            var userAuthEntity = await context.UserAuthData.FindAsync(id);
            if (userAuthEntity == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(context.Users, "Id", "Email");
            return View(userAuthEntity);
        }

        // POST: UserAuth/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserId,PasswordHash,Id,CreatedBy,CreatedAt,UpdatedBy,Deleted")] UserAuthEntity userAuthEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id != userAuthEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    userAuthEntity.UpdatedAt = DateTime.UtcNow;
                    context.Update(userAuthEntity);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAuthEntityExists(userAuthEntity.Id))
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
            ViewData["UserId"] = new SelectList(context.Users, "Id", "Email", userAuthEntity.UserId);
            return View(userAuthEntity);
        }

        // GET: UserAuth/Delete/5
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

            var userAuthEntity = await context.UserAuthData
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAuthEntity == null)
            {
                return NotFound();
            }

            return View(userAuthEntity);
        }

        // POST: UserAuth/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var userAuthEntity = await context.UserAuthData.FindAsync(id);
            if (userAuthEntity != null)
            {
                context.UserAuthData.Remove(userAuthEntity);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserAuthEntityExists(Guid id)
        {
            return context.UserAuthData.Any(e => e.Id == id);
        }
    }
}
