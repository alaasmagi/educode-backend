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
    public class UserTypeController : BaseController
    {
        private readonly AppDbContext _context;

        public UserTypeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: UserType
        public async Task<IActionResult> Index()
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            return View(await _context.UserTypes.ToListAsync());
        }

        // GET: UserType/Details/5
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

            var userTypeEntity = await _context.UserTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userTypeEntity == null)
            {
                return NotFound();
            }

            return View(userTypeEntity);
        }

        // GET: UserType/Create
        public IActionResult Create()
        {
            if (!IsTokenValid(HttpContext))
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
        public async Task<IActionResult> Create([Bind("UserType,Id,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt")] UserTypeEntity userTypeEntity)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(userTypeEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userTypeEntity);
        }

        // GET: UserType/Edit/5
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

            var userTypeEntity = await _context.UserTypes.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("UserType,Id,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt")] UserTypeEntity userTypeEntity)
        {
            if (!IsTokenValid(HttpContext))
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
                    _context.Update(userTypeEntity);
                    await _context.SaveChangesAsync();
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

            var userTypeEntity = await _context.UserTypes
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var userTypeEntity = await _context.UserTypes.FindAsync(id);
            if (userTypeEntity != null)
            {
                _context.UserTypes.Remove(userTypeEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserTypeEntityExists(int id)
        {
            return _context.UserTypes.Any(e => e.Id == id);
        }
    }
}
