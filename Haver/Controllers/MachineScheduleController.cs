﻿    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using haver.Data;
using haver.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System.Numerics;
using Microsoft.VisualBasic;
using System.Reflection.PortableExecutable;
using System.Reflection;
using haver.Utilities;
using haver.CustomControllers;
using Machine = haver.Models.Machine;
using haver.ViewModels;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Text.RegularExpressions;

namespace haver.Controllers
{
    public class MachineScheduleController : ElephantController
    {
        private readonly HaverContext _context;

        public MachineScheduleController(HaverContext context)
        {
            _context = context;
        }

        // GET: MachineSchedule
        public async Task<IActionResult> Index(int? page, int? pageSizeID, int? MachineID, 
            DateTime? DueDate,DateTime? RelDate, DateTime? PODate, DateTime? DelDate,
            string? actionButton, string sortDirection = "asc", string sortField = "Machine")
        {
            //List of sort options.
            string[] sortOptions = new[] { "Machine", "StartDate", "DueDate", "EndDate","PackageReleaseDate","PODueDate","DeliveryDate" };

            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            PopulateDropDownLists();

            var machineSchedules = from m in _context.MachineSchedules
                .Include(m => m.Machine)
                .Include(n => n.Note)
                .Include(s => s.SalesOrders)
                 .Include(p => p.PackageRelease)
                .Include(e => e.MachineScheduleEngineers).ThenInclude(e => e.Engineer)
                .AsNoTracking()
                select m;

            //Add as many filters as needed
            if (MachineID.HasValue)
            {
                machineSchedules = machineSchedules.Where(p => p.MachineID == MachineID);
                numberFilters++;
            }


            if (DueDate.HasValue)
            {
                DateTime SearchDValue = DueDate.Value.Date;
                machineSchedules = machineSchedules.Where(o => o.DueDate.Year == SearchDValue.Year &&
                                                                     o.DueDate.Month == SearchDValue.Month &&
                                                                      o.DueDate.Day == SearchDValue.Day);
                numberFilters++;
            }

            if (RelDate.HasValue)
            {
                DateTime SearchDValue = RelDate.Value.Date;
               machineSchedules = machineSchedules.Where(o => o.PackageRDate.Year == SearchDValue.Year &&
                                                                    o.PackageRDate.Month == SearchDValue.Month &&
                                                                     o.PackageRDate.Day == SearchDValue.Day);
                numberFilters++;
            }
            if (PODate.HasValue)
            {
                DateTime SearchDValue = PODate.Value.Date;
                machineSchedules = machineSchedules.Where(o => o.PODueDate.Year == SearchDValue.Year &&
                                                                    o.PODueDate.Month == SearchDValue.Month &&
                                                                     o.PODueDate.Day == SearchDValue.Day);
                numberFilters++;
            }
            if (DelDate.HasValue)
            {
                DateTime SearchDValue = DelDate.Value.Date;
                machineSchedules = machineSchedules.Where(o => o.DeliveryDate.Year == SearchDValue.Year &&
                                                                    o.DeliveryDate.Month == SearchDValue.Month &&
                                                                     o.DeliveryDate.Day == SearchDValue.Day);
                numberFilters++;
            }


            if (numberFilters != 0)
            {
                ViewData["Filtering"] = " btn-danger";
                //Show how many filters have been applied
                ViewData["numberFilters"] = "(" + numberFilters.ToString()
                    + " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
                //Keep the Bootstrap collapse open
                @ViewData["ShowFilter"] = " show";
            }

            //Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;
                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
            }

            //Now we know which field and direction to sort by
            if (sortField == "StartDate")
            {
                if (sortDirection == "asc")
                {
                    machineSchedules = machineSchedules
                        .OrderByDescending(p => p.StartDate);
                }
                else
                {
                   machineSchedules = machineSchedules
                        .OrderBy(p => p.StartDate);
                }
            }
            else if (sortField == "DueDate")
            {
                if (sortDirection == "asc")
                {
                    machineSchedules = machineSchedules
                        .OrderByDescending(p => p.DueDate);
                }
                else
                {
                    machineSchedules = machineSchedules
                         .OrderBy(p => p.DueDate);
                }
            }
            else if (sortField == "EndDate")
            {
                if (sortDirection == "asc")
                {
                    machineSchedules = machineSchedules
                        .OrderByDescending(p => p.EndDate);
                }
                else
                {
                    machineSchedules = machineSchedules
                         .OrderBy(p => p.EndDate);
                }
            }
            else if (sortField == "PackageReleaseDate")
            {
                if (sortDirection == "asc")
                {
                    machineSchedules = machineSchedules
                        .OrderByDescending(p => p.PackageRDate);
                }
                else
                {
                    machineSchedules = machineSchedules
                         .OrderBy(p => p.PackageRDate);
                }
            }
            else if (sortField == "PODueDate")
            {
                if (sortDirection == "asc")
                {
                    machineSchedules = machineSchedules
                        .OrderByDescending(p => p.PODueDate);
                }
                else
                {
                    machineSchedules = machineSchedules
                         .OrderBy(p => p.PODueDate);
                }
            }
            else if (sortField == "DeliveryDate")
            {
                if (sortDirection == "asc")
                {
                    machineSchedules = machineSchedules
                        .OrderByDescending(p => p.DeliveryDate);
                }
                else
                {
                    machineSchedules = machineSchedules
                         .OrderBy(p => p.DeliveryDate);
                }
            }
            else 
            {
                if (sortDirection == "asc")
                {
                    machineSchedules = machineSchedules
                         .OrderBy(p => p.Machine.Class);
                }
                else
                {
                    machineSchedules = machineSchedules
                        .OrderByDescending(p => p.Machine.Class);
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<MachineSchedule>.CreateAsync(machineSchedules.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        public IActionResult DownloadMachineSchedules()
        {
            var schedules = _context.MachineSchedules
                .Include(ms => ms.Machine)
                .Include(ms => ms.PackageRelease)
                .Include(ms => ms.Note)  // Include the related Note entity
                .Include(ms => ms.SalesOrders)
                    .ThenInclude(so => so.Customer)
                .Include(ms => ms.SalesOrders)
                    .ThenInclude(so => so.Vendor)
                .OrderByDescending(ms => ms.StartDate)
                .ToList() // Forces execution and materializes the data
                .SelectMany(ms => ms.SalesOrders.DefaultIfEmpty(),
                    (ms, so) => new
                    {
                        SalesOrder = so?.OrderNumber ?? "",
                        CustomerName = so?.Customer?.CompanyName ?? "",
                        MachineDescription = ms.Machine?.Description ?? "",
                        SerialNumber = ms.Machine?.SerialNumber ?? "",
                        PackageReleaseDate = ms.PackageRelease != null && ms.PackageRelease.PReleaseDateP.HasValue
                            ? ms.PackageRelease.PReleaseDateP.Value.ToString("yyyy-MM-dd")
                            : "",
                        VendorName = so?.Vendor?.Name ?? "",
                        PONumber = so?.PoNumber ?? "",
                        PODueDate = ms.PODueDate.ToString("yyyy-MM-dd"), // Non-nullable DateTime
                        DeliveryDate = ms.DeliveryDate.ToString("yyyy-MM-dd"), // Non-nullable DateTime
                        Media = ms.Media ? "Yes" : "No",
                        SpareParts = ms.SpareParts ? "Yes" : "No",
                        Base = ms.Base ? "Yes" : "No",
                        AirSeal = ms.AirSeal ? "Yes" : "No",
                        CoatingLining = ms.CoatingLining ? "Yes" : "No",
                        Disassembly = ms.Dissembly ? "Yes" : "No",

                        // Fixed Note Information
                        PreOrder = StripHtml(ms.Note?.PreOrder ?? ""),
                        Scope = StripHtml(ms.Note?.Scope ?? ""),
                        ActualHours = ms.Note != null
                            ? $"Assembly: {ms.Note.AssemblyHours} hrs / Rework: {ms.Note.ReworkHours} hrs"
                            : "",
                        BudgetHours = ms.Note?.BudgetHours != null
                            ? $"{ms.Note.BudgetHours} hrs per machine"
                            : "",
                        NamePlate = ms.Note?.NamePlate != null ? ms.Note.NamePlate.ToString() : "" // Enum conversion outside query
                    });

            int numRows = schedules.Count();

            if (numRows > 0)
            {
                using (ExcelPackage excel = new ExcelPackage())
                {
                    var workSheet = excel.Workbook.Worksheets.Add("Machine Schedules");

                    // Add the main header
                    workSheet.Cells[1, 1].Value = "Machine Schedule Report";
                    using (ExcelRange title = workSheet.Cells[1, 1, 1, 16])
                    {
                        title.Merge = true;
                        title.Style.Font.Bold = true;
                        title.Style.Font.Size = 18;
                        title.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    // Add the timestamp
                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, localTimeZone);
                    using (ExcelRange timestamp = workSheet.Cells[2, 20])
                    {
                        timestamp.Value = "Created: " + localDate.ToString("yyyy-MM-dd HH:mm");
                        timestamp.Style.Font.Bold = true;
                        timestamp.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    // Add the column headers for the main data
                    string[] mainHeaders = {
                "Sales Order", "Customer Name", "Machine Description", "Serial Number", "Package Release Date",
                "Vendor Name", "PO Number", "PO Due Date", "Delivery Date", "Media", "Spare Parts", "Base", "Air Seal",
                "Coating Lining", "Disassembly"
            };
                    for (int i = 0; i < mainHeaders.Length; i++)
                    {
                        workSheet.Cells[3, i + 1].Value = mainHeaders[i];
                    }

                    // Add the "Notes/Comments" header spanning the last five columns
                    using (ExcelRange notesHeader = workSheet.Cells[3, 16, 3, 20])
                    {
                        notesHeader.Value = "Notes/Comments";
                        notesHeader.Merge = true;
                        notesHeader.Style.Font.Bold = true;
                        notesHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        notesHeader.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                        notesHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    // Add individual headers for the note-related columns starting from row 4
                    string[] noteHeaders = { "PreOrder", "Scope", "Actual Hours", "Budget Hours", "NamePlate" };
                    for (int i = 0; i < noteHeaders.Length; i++)
                    {
                        workSheet.Cells[4, 16 + i].Value = noteHeaders[i];
                    }

                    // Load the data starting from row 5
                    workSheet.Cells[5, 1].LoadFromCollection(schedules, false);

                    // Format date columns
                    workSheet.Column(7).Style.Numberformat.Format = "yyyy-MM-dd";
                    workSheet.Column(8).Style.Numberformat.Format = "yyyy-MM-dd";
                    workSheet.Column(9).Style.Numberformat.Format = "yyyy-MM-dd";

                    // Style the main headers
                    using (ExcelRange headings = workSheet.Cells[3, 1, 3, 16])
                    {
                        headings.Style.Font.Bold = true;
                        headings.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        headings.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    }

                    // Style the note headers
                    using (ExcelRange noteHeadings = workSheet.Cells[4, 16, 4, 20])
                    {
                        noteHeadings.Style.Font.Bold = true;
                        noteHeadings.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        noteHeadings.Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                    }

                    workSheet.Cells.AutoFitColumns();

                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "MachineSchedules.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch (Exception)
                    {
                        return BadRequest("Could not build and download the file.");
                    }
                }
            }
            return NotFound("No data available to export.");
        }

        // GET: MachineSchedule/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineSchedule = await _context.MachineSchedules
                .Include(m => m.Machine)
                .Include(n => n.Note)
                 .Include(s => s.SalesOrders)
                .Include(p => p.PackageRelease)
                .Include(e => e.MachineScheduleEngineers).ThenInclude(e => e.Engineer)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (machineSchedule == null)
            {
                return NotFound();
            }

            return View(machineSchedule);
        }

        // GET: MachineSchedule/Create
        public IActionResult Create()
        {
            MachineSchedule schedule= new MachineSchedule();
            PopulateAssignedSpecialtyData(schedule);
            PopulateDropDownLists();
            return View();
        }

        // POST: MachineSchedule/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,StartDate,DueDate,EndDate,PackageRDate,PODueDate,DeliveryDate,Media,SpareParts,SparePMedia,Base,AirSeal,CoatingLining,Dissembly,MachineID")] MachineSchedule machineSchedule,
            string[] selectedOptions)
        {
            try
            {
                UpdateDoctorSpecialties(selectedOptions, machineSchedule);
                if (ModelState.IsValid)
                {
                    _context.Add(machineSchedule);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { machineSchedule.ID });
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            PopulateAssignedSpecialtyData(machineSchedule);  
            PopulateDropDownLists(machineSchedule);
                return View(machineSchedule);
            
        }

        // GET: MachineSchedule/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineSchedule = await _context.MachineSchedules
                 .Include(e => e.MachineScheduleEngineers).ThenInclude(e => e.Engineer)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (machineSchedule == null)
            {
                return NotFound();
            }

            if (machineSchedule.IsCompleted)
            {
                TempData["ErrorMessage"] = "This schedule is marked as completed and cannot be edited.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["MachineID"] = new SelectList(_context.Machines, "ID", "Class", machineSchedule.MachineID);
            PopulateAssignedSpecialtyData(machineSchedule);
            return View(machineSchedule);
        }

        // POST: MachineSchedule/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedOptions)
        {
            var scheduleToUpdate = await _context.MachineSchedules
                .Include(e => e.MachineScheduleEngineers).ThenInclude(e => e.Engineer)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (scheduleToUpdate == null)
            {
                return NotFound();
            }

            UpdateDoctorSpecialties(selectedOptions, scheduleToUpdate);

            //Put the original RowVersion value in the OriginalValues collection for the entity
            //_context.Entry(scheduleToUpdate).Property("RowVersion").OriginalValue = RowVersion;


            if (await TryUpdateModelAsync<MachineSchedule>(scheduleToUpdate, "",
                s => s.StartDate, s => s.DueDate, s => s.EndDate, s => s.PackageRDate,
                s => s.PODueDate, s => s.DeliveryDate, s => s.Media, s => s.SpareParts,
                s => s.SparePMedia, s => s.Base, s => s.AirSeal, s => s.CoatingLining, s => s.Dissembly, s => s.MachineID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { scheduleToUpdate.ID });
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MachineScheduleExists(scheduleToUpdate.ID))
                    {
                        return NotFound();
                    }
                }
                catch (DbUpdateException)
                {

                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");

                }
                return RedirectToAction(nameof(Index));
            }
            PopulateDropDownLists(scheduleToUpdate);
            PopulateAssignedSpecialtyData(scheduleToUpdate);
            return View(scheduleToUpdate);
        }

        // GET: MachineSchedule/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineSchedule = await _context.MachineSchedules
                .Include(m => m.Machine)
                .Include(n => n.Note)
                .Include(p => p.PackageRelease)
                .Include(e => e.MachineScheduleEngineers).ThenInclude(e => e.Engineer)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (machineSchedule == null)
            {
                return NotFound();
            }

            return View(machineSchedule);
        }

        // POST: MachineSchedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var machineSchedule = await _context.MachineSchedules.FindAsync(id);
            try
            {
                if (machineSchedule != null)
                {
                    _context.MachineSchedules.Remove(machineSchedule);
                }

                await _context.SaveChangesAsync();
                var returnUrl = ViewData["returnURL"]?.ToString();
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction(nameof(Index));
                }
                return Redirect(returnUrl);
            }
            catch (DbUpdateException)
            {       
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");  
            }
            return View(machineSchedule);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            var machineSchedule = await _context.MachineSchedules.FindAsync(id);
            if (machineSchedule == null)
            {
                return NotFound();
            }

            machineSchedule.IsCompleted = true;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "The schedule has been marked as completed.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the schedule.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public IActionResult Unfinalize(int id)
        {
            var machineSchedule = _context.MachineSchedules.Find(id);
            if (machineSchedule == null)
            {
                return NotFound();
            }

            machineSchedule.IsCompleted = false;
            _context.Update(machineSchedule);
            _context.SaveChanges();

            return RedirectToAction(nameof(Details), new { id });
        }


        private void PopulateDropDownLists(MachineSchedule? machineSchedule = null)
        {
            ViewData["MachineID"] = new SelectList(_context.Machines, "ID", "Class", machineSchedule?.MachineID);
        }


        private void PopulateAssignedSpecialtyData(MachineSchedule machineSchedule)
        {
            //For this to work, you must have Included the child collection in the parent object
            var allOptions = _context.Engineers;
            var currentOptionsHS = new HashSet<int>(machineSchedule.MachineScheduleEngineers.Select(b => b.EngineerID));
            //Instead of one list with a boolean, we will make two lists
            var selected = new List<ListOptionVM>();
            var available = new List<ListOptionVM>();
            foreach (var s in allOptions)
            {
                if (currentOptionsHS.Contains(s.ID))
                {
                    selected.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.EngineerInitials
                    });
                }
                else
                {
                    available.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.EngineerInitials
                    });
                }
            }

            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
            ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        }

        private void UpdateDoctorSpecialties(string[] selectedOptions, MachineSchedule scheduleToUpdate)
        {
            if (selectedOptions == null)
            {
                scheduleToUpdate.MachineScheduleEngineers = new List<MachineScheduleEngineer>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<int>(scheduleToUpdate.MachineScheduleEngineers.Select(b => b.EngineerID));
            foreach (var s in _context.Engineers)
            {
                if (selectedOptionsHS.Contains(s.ID.ToString()))//it is selected
                {
                    if (!currentOptionsHS.Contains(s.ID))
                    {
                        scheduleToUpdate.MachineScheduleEngineers.Add(new MachineScheduleEngineer
                        {
                            EngineerID = s.ID,
                            MachineScheduleID = scheduleToUpdate.ID
                        });
                    }
                }
                else //not selected
                {
                    if (currentOptionsHS.Contains(s.ID))
                    {
                        MachineScheduleEngineer? specToRemove = scheduleToUpdate.MachineScheduleEngineers.FirstOrDefault(d => d.EngineerID == s.ID);
                        if (specToRemove != null)
                        {
                            _context.Remove(specToRemove);
                        }
                    }
                }
            }
        }

        private bool MachineScheduleExists(int id)
        {
            return _context.MachineSchedules.Any(e => e.ID == id);
        }

public string StripHtml(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input; // Return the input as is if it's null or empty
        }

        return Regex.Replace(input, "<.*?>", string.Empty); // Remove HTML tags
    }

}
}
