using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Contracts;

namespace WebApp.Controllers
{
    public class UserController(AppDbContext context, IAdminAccessService adminAccessService)
        : BaseController(adminAccessService)
    {
        // GET: User
        public async Task<IActionResult> Index()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var users = context.Users.Include(u => u.UserType);
            return View(await users.ToListAsync());
        }

        // GET: User/Details/5
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

            var userEntity = await context.Users
                .Include(u => u.UserType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userEntity == null)
            {
                return NotFound();
            }

            return View(userEntity);
        }

        // GET: User/Create
        public async Task<IActionResult> Create()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            ViewData["UserType"] = new SelectList(context.UserTypes, "Id", "UserType");
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserTypeId,UniId,StudentCode,FullName,Id,CreatedBy,UpdatedBy")] UserEntity userEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (ModelState.IsValid)
            {
                userEntity.UpdatedAt = DateTime.Now.ToUniversalTime();
                userEntity.CreatedAt = DateTime.Now.ToUniversalTime();
                context.Add(userEntity);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["UserType"] = new SelectList(context.UserTypes, "Id", "UserType", userEntity.UserType);
            return View(userEntity);
        }

        // GET: User/Edit/5
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

            var userEntity = await context.Users.FindAsync(id);
            if (userEntity == null)
            {
                return NotFound();
            }
            ViewData["UserType"] = new SelectList(context.UserTypes, "Id", "UserType");
            return View(userEntity);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserTypeId,UniId,StudentCode,FullName,Id,CreatedBy,CreatedAt,UpdatedBy")] UserEntity userEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id != userEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    userEntity.UpdatedAt = DateTime.Now.ToUniversalTime();
                    context.Update(userEntity);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserEntityExists(userEntity.Id))
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
            ViewData["UserType"] = new SelectList(context.UserTypes, "Id", "UserType", userEntity.UserType);
            return View(userEntity);
        }

        // GET: User/Delete/5
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

            var userEntity = await context.Users
                .Include(u => u.UserType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userEntity == null)
            {
                return NotFound();
            }

            return View(userEntity);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var userEntity = await context.Users.FindAsync(id);
            if (userEntity != null)
            {
                context.Users.Remove(userEntity);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserEntityExists(int id)
        {
            return context.Users.Any(e => e.Id == id);
        }
    }
}
