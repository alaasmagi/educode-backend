using App.DAL.EF;
using App.Domain;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers
{
    public class RefreshTokenController(AppDbContext context, RedisRepository redis, IAdminAccessService adminAccessService)
        : BaseController(adminAccessService)
    {
        // GET: RefreshToken
        public async Task<IActionResult> Index()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }

            return View(await context.RefreshTokens.IgnoreQueryFilters().ToListAsync());
        }

        // GET: RefreshToken/Details/5
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

            var refreshTokenEntity = await context.RefreshTokens
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (refreshTokenEntity == null)
            {
                return NotFound();
            }

            return View(refreshTokenEntity);
        }

        // GET: RefreshToken/Create
        public async Task<IActionResult> Create()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }

            return View();
        }

        // POST: RefreshToken/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("UserId,Token,CreatedByIp,ExpirationTime,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,Deleted")]
            RefreshTokenEntity refreshTokenEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }

            if (ModelState.IsValid)
            {
                refreshTokenEntity.Id = Guid.NewGuid();
                refreshTokenEntity.CreatedAt = DateTime.UtcNow;
                refreshTokenEntity.UpdatedAt = DateTime.UtcNow;
                context.Add(refreshTokenEntity);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(refreshTokenEntity);
        }

        // GET: RefreshToken/Edit/5
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

            var refreshTokenEntity = await context.RefreshTokens
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(r => r.Id == id);
            if (refreshTokenEntity == null)
            {
                return NotFound();
            }

            return View(refreshTokenEntity);
        }

        // POST: RefreshToken/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id,
            [Bind("UserId,Token,CreatedByIp,ExpirationTime,Id,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,Deleted")]
            RefreshTokenEntity refreshTokenEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }

            if (id != refreshTokenEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    refreshTokenEntity.CreatedAt = DateTime.SpecifyKind(refreshTokenEntity.CreatedAt, DateTimeKind.Utc);
                    refreshTokenEntity.UpdatedAt = DateTime.UtcNow;
                    await redis.DeleteKeysByPatternAsync(refreshTokenEntity.Id.ToString());
                    await redis.DeleteKeysByPatternAsync(refreshTokenEntity.Token);
                    context.Update(refreshTokenEntity);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) {}

                return RedirectToAction(nameof(Index));
            }

            return View(refreshTokenEntity);
        }

        // GET: RefreshToken/Delete/5
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

            var refreshTokenEntity = await context.RefreshTokens
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (refreshTokenEntity == null)
            {
                return NotFound();
            }

            return View(refreshTokenEntity);
        }

        // POST: RefreshToken/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }

            var refreshTokenEntity = await context.RefreshTokens
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(r => r.Id == id);
            if (refreshTokenEntity != null)
            {
                await redis.DeleteKeysByPatternAsync(refreshTokenEntity.Id.ToString());
                await redis.DeleteKeysByPatternAsync(refreshTokenEntity.Token);
                context.RefreshTokens.Remove(refreshTokenEntity);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}