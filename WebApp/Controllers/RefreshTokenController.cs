using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Contracts;

namespace WebApp.Controllers
{
    public class RefreshTokenController(AppDbContext context, IAdminAccessService adminAccessService)
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
            
            var appDbContext = context.RefreshTokens.Include(r => r.User);
            return View(await appDbContext.IgnoreQueryFilters().ToListAsync());
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
                .Include(r => r.User)
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
            
            ViewData["UserId"] = new SelectList(context.Users, "Id", "UniId");
            return View();
        }

        // POST: RefreshToken/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Token,ExpirationTime,IsUsed,IsRevoked,ReplacedByTokenId,RevokedAt,RevokedByIp,CreatedByIp,Id,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,Deleted")] RefreshTokenEntity refreshTokenEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (ModelState.IsValid)
            {
                refreshTokenEntity.Id = Guid.NewGuid();
                refreshTokenEntity.ExpirationTime = DateTime.SpecifyKind(
                    refreshTokenEntity.ExpirationTime,
                    DateTimeKind.Local
                ).ToUniversalTime();
                refreshTokenEntity.CreatedAt = DateTime.Now.ToUniversalTime();
                refreshTokenEntity.UpdatedAt = DateTime.Now.ToUniversalTime();
                context.Add(refreshTokenEntity);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(context.Users, "Id", "UniId", refreshTokenEntity.UserId);
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

            var refreshTokenEntity = await context.RefreshTokens.FindAsync(id);
            if (refreshTokenEntity == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(context.Users, "Id", "UniId", refreshTokenEntity.UserId);
            return View(refreshTokenEntity);
        }

        // POST: RefreshToken/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserId,Token,ExpirationTime,IsUsed,IsRevoked,ReplacedByTokenId,RevokedAt,RevokedByIp,CreatedByIp,Id,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,Deleted")] RefreshTokenEntity refreshTokenEntity)
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
                    refreshTokenEntity.ExpirationTime = DateTime.SpecifyKind(
                        refreshTokenEntity.ExpirationTime,
                        DateTimeKind.Local
                    ).ToUniversalTime();
                    refreshTokenEntity.UpdatedAt = DateTime.Now.ToUniversalTime();
                    context.Update(refreshTokenEntity);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RefreshTokenEntityExists(refreshTokenEntity.Id))
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
            ViewData["UserId"] = new SelectList(context.Users, "Id", "UniId", refreshTokenEntity.UserId);
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
                .Include(r => r.User)
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
            
            var refreshTokenEntity = await context.RefreshTokens.FindAsync(id);
            if (refreshTokenEntity != null)
            {
                context.RefreshTokens.Remove(refreshTokenEntity);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RefreshTokenEntityExists(Guid id)
        {
            return context.RefreshTokens.Any(e => e.Id == id);
        }
    }
}
