﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using haver.Data;
using haver.Models;

namespace haver.Controllers
{
    public class NoteController : Controller
    {
        private readonly HaverContext _context;

        public NoteController(HaverContext context)
        {
            _context = context;
        }

        // GET: Note
        public async Task<IActionResult> Index()
        {
            var haverContext = _context.Notes.Include(n => n.MachineSchedule);
            return View(await haverContext.ToListAsync());
        }

        // GET: Note/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Notes
<<<<<<< HEAD
                .AsNoTracking()
=======
                .Include(n => n.MachineSchedule)
>>>>>>> 72ec3151358c738571b34bf22b29aa45f8631ede
                .FirstOrDefaultAsync(m => m.ID == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // GET: Note/Create
        public IActionResult Create()
        {
            ViewData["MachineScheduleID"] = new SelectList(_context.MachineSchedules, "ID", "ID");
            return View();
        }

        // POST: Note/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,PreOrder,Scope,AssemblyHours,ReworkHours,BudgetHours,NamePlate,MachineScheduleID")] Note note)
        {
            if (ModelState.IsValid)
            {
                _context.Add(note);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MachineScheduleID"] = new SelectList(_context.MachineSchedules, "ID", "ID", note.MachineScheduleID);
            return View(note);
        }

        // GET: Note/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }
            ViewData["MachineScheduleID"] = new SelectList(_context.MachineSchedules, "ID", "ID", note.MachineScheduleID);
            return View(note);
        }

        // POST: Note/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
<<<<<<< HEAD
        public async Task<IActionResult> Edit(int id)
=======
        public async Task<IActionResult> Edit(int id, [Bind("ID,PreOrder,Scope,AssemblyHours,ReworkHours,BudgetHours,NamePlate,MachineScheduleID")] Note note)
>>>>>>> 72ec3151358c738571b34bf22b29aa45f8631ede
        {
            var noteToUpdate = await _context.Notes.FirstOrDefaultAsync(n => n.ID == id);

            if (noteToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Note>(noteToUpdate, "", n => n.PreOrder,
                    n => n.Scope, n => n.AssemblyHours, n => n.ReworkHours, n => n.BudgetHours,
                    n => n.NamePlate, n => n.MachineScheduleID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoteExists(noteToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
<<<<<<< HEAD
                }      
            }     
            return View(noteToUpdate);
=======
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MachineScheduleID"] = new SelectList(_context.MachineSchedules, "ID", "ID", note.MachineScheduleID);
            return View(note);
>>>>>>> 72ec3151358c738571b34bf22b29aa45f8631ede
        }

        // GET: Note/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Notes
<<<<<<< HEAD
                .AsNoTracking()
=======
                .Include(n => n.MachineSchedule)
>>>>>>> 72ec3151358c738571b34bf22b29aa45f8631ede
                .FirstOrDefaultAsync(m => m.ID == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // POST: Note/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var note = await _context.Notes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (note != null)
            {
                _context.Notes.Remove(note);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NoteExists(int id)
        {
            return _context.Notes.Any(e => e.ID == id);
        }
    }
}
