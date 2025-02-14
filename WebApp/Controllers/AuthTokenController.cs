using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;

namespace WebApp.Controllers
{
    public class AuthTokenController : Controller
    {
        private readonly AppDbContext _context;

        public AuthTokenController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AuthToken
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.UserAuthTokens.Include(u => u.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: AuthToken/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAuthTokenEntity = await _context.UserAuthTokens
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAuthTokenEntity == null)
            {
                return NotFound();
            }

            return View(userAuthTokenEntity);
        }

        // GET: AuthToken/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CreatedBy");
            return View();
        }

        // POST: AuthToken/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Token,ExpireTime,Id,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt")] UserAuthTokenEntity userAuthTokenEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userAuthTokenEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CreatedBy", userAuthTokenEntity.UserId);
            return View(userAuthTokenEntity);
        }

        // GET: AuthToken/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAuthTokenEntity = await _context.UserAuthTokens.FindAsync(id);
            if (userAuthTokenEntity == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CreatedBy", userAuthTokenEntity.UserId);
            return View(userAuthTokenEntity);
        }

        // POST: AuthToken/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Token,ExpireTime,Id,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt")] UserAuthTokenEntity userAuthTokenEntity)
        {
            if (id != userAuthTokenEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userAuthTokenEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAuthTokenEntityExists(userAuthTokenEntity.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CreatedBy", userAuthTokenEntity.UserId);
            return View(userAuthTokenEntity);
        }

        // GET: AuthToken/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAuthTokenEntity = await _context.UserAuthTokens
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAuthTokenEntity == null)
            {
                return NotFound();
            }

            return View(userAuthTokenEntity);
        }

        // POST: AuthToken/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userAuthTokenEntity = await _context.UserAuthTokens.FindAsync(id);
            if (userAuthTokenEntity != null)
            {
                _context.UserAuthTokens.Remove(userAuthTokenEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserAuthTokenEntityExists(int id)
        {
            return _context.UserAuthTokens.Any(e => e.Id == id);
        }
    }
}
