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
    public class CourseAttendanceController : BaseController
    {
        private readonly AppDbContext _context;

        public CourseAttendanceController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CourseAttendance
        public async Task<IActionResult> Index()
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var appDbContext = _context.CourseAttendances.Include(c => c.AttendanceType).Include(c => c.Course);
            return View(await appDbContext.ToListAsync());
        }

        // GET: CourseAttendance/Details/5
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

            var courseAttendanceEntity = await _context.CourseAttendances
                .Include(c => c.AttendanceType)
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseAttendanceEntity == null)
            {
                return NotFound();
            }

            return View(courseAttendanceEntity);
        }

        // GET: CourseAttendance/Create
        public IActionResult Create()
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            ViewData["AttendanceTypeId"] = new SelectList(_context.AttendanceTypes, "Id", "AttendanceType");
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "CourseCode");
            return View();
        }

        // POST: CourseAttendance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,AttendanceTypeId,StartTime,EndTime,OnlineRegistration,Id,CreatedBy,UpdatedBy")] CourseAttendanceEntity courseAttendanceEntity)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (ModelState.IsValid)
            {
                courseAttendanceEntity.UpdatedAt = DateTime.Now.ToUniversalTime();
                courseAttendanceEntity.CreatedAt = DateTime.Now.ToUniversalTime();
                _context.Add(courseAttendanceEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AttendanceTypeId"] = new SelectList(_context.AttendanceTypes, "Id", "AttendanceType", courseAttendanceEntity.AttendanceTypeId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "CourseCode", courseAttendanceEntity.CourseId);
            return View(courseAttendanceEntity);
        }

        // GET: CourseAttendance/Edit/5
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

            var courseAttendanceEntity = await _context.CourseAttendances.FindAsync(id);
            if (courseAttendanceEntity == null)
            {
                return NotFound();
            }
            ViewData["AttendanceTypeId"] = new SelectList(_context.AttendanceTypes, "Id", "AttendanceType", courseAttendanceEntity.AttendanceTypeId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "CourseCode", courseAttendanceEntity.CourseId);
            return View(courseAttendanceEntity);
        }

        // POST: CourseAttendance/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,AttendanceTypeId,StartTime,EndTime,OnlineRegistration,Id,CreatedBy,CreatedAt,UpdatedBy")] CourseAttendanceEntity courseAttendanceEntity)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id != courseAttendanceEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    courseAttendanceEntity.UpdatedAt = DateTime.Now;
                    _context.Update(courseAttendanceEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseAttendanceEntityExists(courseAttendanceEntity.Id))
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
            ViewData["AttendanceTypeId"] = new SelectList(_context.AttendanceTypes, "Id", "AttendanceType", courseAttendanceEntity.AttendanceTypeId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "CourseCode", courseAttendanceEntity.CourseId);
            return View(courseAttendanceEntity);
        }

        // GET: CourseAttendance/Delete/5
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

            var courseAttendanceEntity = await _context.CourseAttendances
                .Include(c => c.AttendanceType)
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseAttendanceEntity == null)
            {
                return NotFound();
            }

            return View(courseAttendanceEntity);
        }

        // POST: CourseAttendance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsTokenValid(HttpContext))
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var courseAttendanceEntity = await _context.CourseAttendances.FindAsync(id);
            if (courseAttendanceEntity != null)
            {
                _context.CourseAttendances.Remove(courseAttendanceEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseAttendanceEntityExists(int id)
        {
            return _context.CourseAttendances.Any(e => e.Id == id);
        }
    }
}
