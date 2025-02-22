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
    public class UserAuthController : BaseController
    {
        private readonly AppDbContext _context;

        public UserAuthController(AppDbContext context)
        {
            _context = context;
        }

        // GET: UserAuth
        public async Task<IActionResult> Index()
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var appDbContext = _context.UserAuthData.Include(u => u.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: UserAuth/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id == null)
            {
                return NotFound();
            }

            var userAuthEntity = await _context.UserAuthData
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAuthEntity == null)
            {
                return NotFound();
            }

            return View(userAuthEntity);
        }

        // GET: UserAuth/Create
        public IActionResult Create()
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            ViewData["UserId"] = new SelectList(_context.Users, "User", "User");
            return View();
        }

        // POST: UserAuth/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,PasswordHash,Id,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt")] UserAuthEntity userAuthEntity)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(userAuthEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "User", userAuthEntity.UserId);
            return View(userAuthEntity);
        }

        // GET: UserAuth/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id == null)
            {
                return NotFound();
            }

            var userAuthEntity = await _context.UserAuthData.FindAsync(id);
            if (userAuthEntity == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CreatedBy", userAuthEntity.UserId);
            return View(userAuthEntity);
        }

        // POST: UserAuth/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,PasswordHash,Id,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt")] UserAuthEntity userAuthEntity)
        {
            if (!IsTokenValid(HttpContext))
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
                    _context.Update(userAuthEntity);
                    await _context.SaveChangesAsync();
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CreatedBy", userAuthEntity.UserId);
            return View(userAuthEntity);
        }

        // GET: UserAuth/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id == null)
            {
                return NotFound();
            }

            var userAuthEntity = await _context.UserAuthData
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var userAuthEntity = await _context.UserAuthData.FindAsync(id);
            if (userAuthEntity != null)
            {
                _context.UserAuthData.Remove(userAuthEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserAuthEntityExists(int id)
        {
            return _context.UserAuthData.Any(e => e.Id == id);
        }
    }
}
