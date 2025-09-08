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
    public class CourseStatusController(AppDbContext context, IAdminAccessService adminAccessService)
        : BaseController(adminAccessService)
    {
        // GET: CourseStatus
        public async Task<IActionResult> Index()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            return View(await context.CourseStatuses.IgnoreQueryFilters().ToListAsync());
        }

        // GET: CourseStatus/Details/5
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

            var courseStatusEntity = await context.CourseStatuses
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseStatusEntity == null)
            {
                return NotFound();
            }

            return View(courseStatusEntity);
        }

        // GET: CourseStatus/Create
        public async Task<IActionResult> Create()
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            return View();
        }

        // POST: CourseStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseStatus,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,Deleted")] CourseStatusEntity courseStatusEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (ModelState.IsValid)
            {
                courseStatusEntity.Id = Guid.NewGuid();
                courseStatusEntity.CreatedAt = DateTime.UtcNow;
                courseStatusEntity.UpdatedAt = DateTime.UtcNow;
                context.Add(courseStatusEntity);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(courseStatusEntity);
        }

        // GET: CourseStatus/Edit/5
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

            var courseStatusEntity = await context.CourseStatuses
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (courseStatusEntity == null)
            {
                return NotFound();
            }
            return View(courseStatusEntity);
        }

        // POST: CourseStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CourseStatus,Id,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,Deleted")] CourseStatusEntity courseStatusEntity)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            if (id != courseStatusEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    courseStatusEntity.CreatedAt = DateTime.SpecifyKind(courseStatusEntity.CreatedAt, DateTimeKind.Utc);
                    courseStatusEntity.UpdatedAt = DateTime.UtcNow;
                    context.Update(courseStatusEntity);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseStatusEntityExists(courseStatusEntity.Id))
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
            return View(courseStatusEntity);
        }

        // GET: CourseStatus/Delete/5
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

            var courseStatusEntity = await context.CourseStatuses
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseStatusEntity == null)
            {
                return NotFound();
            }

            return View(courseStatusEntity);
        }

        // POST: CourseStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tokenValidity = await IsTokenValidAsync(HttpContext);
            if (!tokenValidity)
            {
                return Unauthorized("You cannot access admin panel without logging in!");
            }
            
            var courseStatusEntity = await context.CourseStatuses
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (courseStatusEntity != null)
            {
                context.CourseStatuses.Remove(courseStatusEntity);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseStatusEntityExists(Guid id)
        {
            return context.CourseStatuses.IgnoreQueryFilters().Any(e => e.Id == id);
        }
    }
}
