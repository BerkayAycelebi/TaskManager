using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User/Tasks
        public async Task<IActionResult> Index()
        {
            var email = User.Identity.Name;

            var userid = _context.Users.Where(x => x.Email == email).Select(y => y.Id).FirstOrDefault();
            var applicationDbContext = _context.Tasks.Where(y => y.UserId == userid).Where(z => z.IsDone == false).ToListAsync();
          
            return View(await applicationDbContext);
        }

        public async Task<IActionResult> Completed()
        {
            var email = User.Identity.Name;

            var userid = _context.Users.Where(x => x.Email == email).Select(y => y.Id).FirstOrDefault();
            var applicationDbContext = _context.Tasks.Where(y => y.UserId == userid).Where(z=>z.IsDone==true).ToListAsync();
         

            return View(await applicationDbContext);
        }


        // GET: User/Task/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: User/Task/Create
        public IActionResult Create()
        {
            var email = User.Identity.Name;

             ViewData["UserId"] = new SelectList(_context.ApplicationUsers.Where(x => x.Email == email), "Id", "Id");
            
            return View();
        }

        // POST: User/Task/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,IsDone,ExecutionDate,UserId")] Models.Task task)
        {
            if (ModelState.IsValid)
            {
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", task.UserId);
            return View(task);
        }

        // GET: User/Task/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
           

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", task.UserId);
            return View(task);
        }

        // POST: User/Task/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,IsDone,ExecutionDate,UserId")] Models.Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
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
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", task.UserId);
            return View(task);
        }

        // GET: User/Task/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: User/Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
