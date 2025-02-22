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
    public class AttendanceTypeController : BaseController
    {
        private readonly AppDbContext _context;

        public AttendanceTypeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AttendanceType
        public async Task<IActionResult> Index()
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            return View(await _context.AttendanceTypes.ToListAsync());
        }

        // GET: AttendanceType/Details/5
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

            var attendanceTypeEntity = await _context.AttendanceTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attendanceTypeEntity == null)
            {
                return NotFound();
            }

            return View(attendanceTypeEntity);
        }

        // GET: AttendanceType/Create
        public IActionResult Create()
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            return View();
        }

        // POST: AttendanceType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AttendanceType,Id,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt")] AttendanceTypeEntity attendanceTypeEntity)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(attendanceTypeEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(attendanceTypeEntity);
        }

        // GET: AttendanceType/Edit/5
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

            var attendanceTypeEntity = await _context.AttendanceTypes.FindAsync(id);
            if (attendanceTypeEntity == null)
            {
                return NotFound();
            }
            return View(attendanceTypeEntity);
        }

        // POST: AttendanceType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AttendanceType,Id,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt")] AttendanceTypeEntity attendanceTypeEntity)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id != attendanceTypeEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attendanceTypeEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendanceTypeEntityExists(attendanceTypeEntity.Id))
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
            return View(attendanceTypeEntity);
        }

        // GET: AttendanceType/Delete/5
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

            var attendanceTypeEntity = await _context.AttendanceTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attendanceTypeEntity == null)
            {
                return NotFound();
            }

            return View(attendanceTypeEntity);
        }

        // POST: AttendanceType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var attendanceTypeEntity = await _context.AttendanceTypes.FindAsync(id);
            if (attendanceTypeEntity != null)
            {
                _context.AttendanceTypes.Remove(attendanceTypeEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AttendanceTypeEntityExists(int id)
        {
            return _context.AttendanceTypes.Any(e => e.Id == id);
        }
    }
}
