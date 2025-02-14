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
    public class WorkplaceController : Controller
    {
        private readonly AppDbContext _context;

        public WorkplaceController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Workplace
        public async Task<IActionResult> Index()
        {
            return View(await _context.Workplaces.ToListAsync());
        }

        // GET: Workplace/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workplaceEntity = await _context.Workplaces
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workplaceEntity == null)
            {
                return NotFound();
            }

            return View(workplaceEntity);
        }

        // GET: Workplace/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Workplace/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClassRoom,ComputerCode,Id,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt")] WorkplaceEntity workplaceEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workplaceEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workplaceEntity);
        }

        // GET: Workplace/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workplaceEntity = await _context.Workplaces.FindAsync(id);
            if (workplaceEntity == null)
            {
                return NotFound();
            }
            return View(workplaceEntity);
        }

        // POST: Workplace/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClassRoom,ComputerCode,Id,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt")] WorkplaceEntity workplaceEntity)
        {
            if (id != workplaceEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workplaceEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkplaceEntityExists(workplaceEntity.Id))
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
            return View(workplaceEntity);
        }

        // GET: Workplace/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workplaceEntity = await _context.Workplaces
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workplaceEntity == null)
            {
                return NotFound();
            }

            return View(workplaceEntity);
        }

        // POST: Workplace/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workplaceEntity = await _context.Workplaces.FindAsync(id);
            if (workplaceEntity != null)
            {
                _context.Workplaces.Remove(workplaceEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkplaceEntityExists(int id)
        {
            return _context.Workplaces.Any(e => e.Id == id);
        }
    }
}
