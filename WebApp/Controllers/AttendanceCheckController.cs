using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebApp.Controllers
{
    public class AttendanceCheckController : BaseController
    {
        private readonly AppDbContext _context;

        public AttendanceCheckController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AttendanceCheck
        public async Task<IActionResult> Index()
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            var appDbContext = _context.AttendanceChecks.Include(a => a.Workplace);
            return View(await appDbContext.ToListAsync());
        }

        // GET: AttendanceCheck/Details/5
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

            var attendanceCheckEntity = await _context.AttendanceChecks
                .Include(a => a.Workplace)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attendanceCheckEntity == null)
            {
                return NotFound();
            }

            return View(attendanceCheckEntity);
        }

        // GET: AttendanceCheck/Create
        public IActionResult Create()
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            ViewData["WorkplaceId"] = new SelectList(_context.Workplaces, "Id", "ClassRoom");
            return View();
        }

        // POST: AttendanceCheck/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,CourseAttendanceId,WorkplaceId,Id,CreatedBy,UpdatedBy")] AttendanceCheckEntity attendanceCheckEntity)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (ModelState.IsValid)
            {
                attendanceCheckEntity.UpdatedAt = DateTime.Now;
                attendanceCheckEntity.CreatedAt = DateTime.Now;
                _context.Add(attendanceCheckEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WorkplaceId"] = new SelectList(_context.Workplaces, "Id", "ClassRoom", attendanceCheckEntity.WorkplaceId);
            return View(attendanceCheckEntity);
        }

        // GET: AttendanceCheck/Edit/5
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

            var attendanceCheckEntity = await _context.AttendanceChecks.FindAsync(id);
            if (attendanceCheckEntity == null)
            {
                return NotFound();
            }
            ViewData["WorkplaceId"] = new SelectList(_context.Workplaces, "Id", "ClassRoom", attendanceCheckEntity.WorkplaceId);
            return View(attendanceCheckEntity);
        }

        // POST: AttendanceCheck/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,CourseAttendanceId,WorkplaceId,Id,CreatedBy,CreatedAt,UpdatedBy")] AttendanceCheckEntity attendanceCheckEntity)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id != attendanceCheckEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    attendanceCheckEntity.UpdatedAt = DateTime.Now;
                    _context.Update(attendanceCheckEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendanceCheckEntityExists(attendanceCheckEntity.Id))
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
            ViewData["WorkplaceId"] = new SelectList(_context.Workplaces, "Id", "ClassRoom", attendanceCheckEntity.WorkplaceId);
            return View(attendanceCheckEntity);
        }

        // GET: AttendanceCheck/Delete/5
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

            var attendanceCheckEntity = await _context.AttendanceChecks
                .Include(a => a.Workplace)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attendanceCheckEntity == null)
            {
                return NotFound();
            }

            return View(attendanceCheckEntity);
        }

        // POST: AttendanceCheck/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var attendanceCheckEntity = await _context.AttendanceChecks.FindAsync(id);
            if (attendanceCheckEntity != null)
            {
                _context.AttendanceChecks.Remove(attendanceCheckEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AttendanceCheckEntityExists(int id)
        {
            return _context.AttendanceChecks.Any(e => e.Id == id);
        }
    }
}
