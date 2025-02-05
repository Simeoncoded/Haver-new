﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using haver.Data;
using haver.Models;
using haver.CustomControllers;
using haver.Utilities;
using System.Reflection.PortableExecutable;
using haver.ViewModels;
using System.Numerics;
using Microsoft.EntityFrameworkCore.Storage;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;

namespace haver.Controllers
{
    public class SalesOrderController : ElephantController
    {
        private readonly HaverContext _context;

        public SalesOrderController(HaverContext context)
        {
            _context = context;
        }

        // GET: SalesOrder
        public async Task<IActionResult> Index(int? page, int? pageSizeID, string? SearchString, string? StatusFilter, int? CustomerID,
            string? actionButton, string sortDirection = "asc",  string sortField="OrderNumber")
        {
            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Order Number", "Customer" };

            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
			//Then in each "test" for filtering, add to the count of Filters applied

			if (Enum.TryParse(StatusFilter, out Status selectedStatus))
			{

				ViewBag.StatusSelectList = Status.Draft.ToSelectList(selectedStatus);
			}
			else
			{
				ViewBag.StatusSelectList = Status.Draft.ToSelectList(null);
			}

			PopulateDropDownLists();

            var salesOrders = from s in _context.SalesOrders
                              .Include(s => s.Customer)
                              .Include(d => d.SalesOrderEngineers).ThenInclude(d => d.Engineer)
                              .AsNoTracking()
                              select s;

            //Add as many filters as needed
            if (CustomerID.HasValue)
            {
               salesOrders = salesOrders.Where(p => p.CustomerID == CustomerID);
                numberFilters++;
            }
            if (!String.IsNullOrEmpty(SearchString))
            {
                salesOrders = salesOrders.Where(p => p.OrderNumber.Contains(SearchString));
                numberFilters++;
            }
			if (!String.IsNullOrEmpty(StatusFilter))
			{
			    salesOrders = salesOrders.Where(p => p.Status == selectedStatus);
				numberFilters++;
			}


			//Give feedback about the state of the filters
			if (numberFilters != 0)
            {
                //Toggle the Open/Closed state of the collapse depending on if we are filtering
                ViewData["Filtering"] = " btn-danger";
                //Show how many filters have been applied
                ViewData["numberFilters"] = "(" + numberFilters.ToString()
                    + " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
            }

            //Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
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
            if (sortField == "Customer")
            {
                if (sortDirection == "asc")
                {
                    salesOrders = salesOrders
                        .OrderByDescending(p => p.Customer.CompanyName);
                }
                else
                {
                    salesOrders = salesOrders
                        .OrderBy(p => p.Customer.CompanyName);
                }
            }
            
            else //Sorting by Patient Name
            {
                if (sortDirection == "asc")
                {
                    salesOrders = salesOrders
                        .OrderBy(p => p.OrderNumber);
                }
                else
                {
                    salesOrders = salesOrders
                        .OrderByDescending(p => p.OrderNumber);
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;


            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<SalesOrder>.CreateAsync(salesOrders.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: SalesOrder/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrders
                .Include(s => s.Customer)
                .Include(s => s.PackageRelease)
                .Include(s => s.Machines)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            return View(salesOrder);
        }

        // GET: SalesOrder/Create
        public IActionResult Create()
        {
            SalesOrder salesOrder = new SalesOrder();
            PopulateAssignedSpecialtyData(salesOrder);
            PopulateDropDownLists();
            return View();
        }

        // POST: SalesOrder/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,OrderNumber,SoDate,Price,ShippingTerms,AppDwgRcd,DwgIsDt,PDate,CustomerID,Comments")] SalesOrder salesOrder
            ,string[] selectedOptions, string actionType)
        {
			try
			{
                // Skip custom validation when saving as a draft
                if (actionType == "save")
                {
                    // Clear ModelState to skip DataAnnotations validation
                    ModelState.Clear();
                    // Manually set a flag or custom validation status
                    salesOrder.Status = Status.Draft;  // Set status to Draft when saving as draft
                }

                UpdateSalesOrderEngineers(selectedOptions, salesOrder);

                if (ModelState.IsValid)
                {
                    if (actionType == "save")
                    {
                       // salesOrder.Status = Status.Draft; // Corrected: Assigning enum value
                        _context.Add(salesOrder);
                        await _context.SaveChangesAsync();
                        TempData["Message"] = "Sales Order saved as Draft. You can continue later.";
                        return RedirectToAction("Index"); // Redirect to index after saving
                    }
                    else if (actionType == "next")
                    {
                        salesOrder.Status = Status.InProgress; // Corrected: Assigning enum value
                        _context.Add(salesOrder);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index", "SalesOrderProcurement", new { SalesOrderID = salesOrder.ID });
                    }
                }
			}
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException dex)
			{
				if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: SalesOrders.OrderNumber"))
				{
					ModelState.AddModelError("OrderNumber", "Unable to save changes. Remember, you cannot have duplicate Order Number.");
				}
				else
				{
					ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
				}
			}
            PopulateAssignedSpecialtyData(salesOrder);
			PopulateDropDownLists(salesOrder);
			return View(salesOrder);
		}

        // GET: SalesOrder/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrders
                .Include(d => d.SalesOrderEngineers).ThenInclude(d => d.Engineer)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (salesOrder == null)
            {
                return NotFound();
            }
            PopulateAssignedSpecialtyData(salesOrder);
			PopulateDropDownLists(salesOrder);
			return View(salesOrder);
        }

        // POST: SalesOrder/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedOptions)
        {
			var salesOrderToUpdate = await _context.SalesOrders
                .Include(d => d.SalesOrderEngineers).ThenInclude(d => d.Engineer)
                .FirstOrDefaultAsync(p => p.ID == id);

			if (salesOrderToUpdate == null)
			{
				return NotFound();
			}

            UpdateSalesOrderEngineers(selectedOptions, salesOrderToUpdate);

            // Check if status is Draft and update it to InProgress before saving
            if (salesOrderToUpdate.Status == Status.Draft)
            {
                salesOrderToUpdate.Status = Status.InProgress;
            }

            if (await TryUpdateModelAsync<SalesOrder>(salesOrderToUpdate, "",
			   p => p.OrderNumber, p => p.SoDate, p => p.Price, p => p.ShippingTerms,
			   p => p.AppDwgRcd, p => p.DwgIsDt, p => p.PDate, p => p.CustomerID,p => p.Comments))
			{
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Sales Order updated and status set to 'In Progress'.";
                    return RedirectToAction("Index", "SalesOrderProcurement", new { SalesOrderID = salesOrderToUpdate.ID });
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }

                catch (DbUpdateException dex)
                {
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: SalesOrders.OrderNumber"))
                    {
                        ModelState.AddModelError("OrderNumber", "Unable to save changes. Remember, you cannot have duplicate Order Number.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }

			}
            PopulateAssignedSpecialtyData(salesOrderToUpdate);
            PopulateDropDownLists(salesOrderToUpdate);
            return View(salesOrderToUpdate);
        }

        // GET: SalesOrder/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrders
                .Include(s => s.Customer)
                .Include(d => d.SalesOrderEngineers).ThenInclude(d => d.Engineer)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            return View(salesOrder);
        }


        // POST: SalesOrder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
			var salesOrder = await _context.SalesOrders
			  .Include(s => s.Customer)
              .Include(d => d.SalesOrderEngineers).ThenInclude(d => d.Engineer)
			  .FirstOrDefaultAsync(m => m.ID == id);

            try
            {
				if (salesOrder != null)
				{
					_context.SalesOrders.Remove(salesOrder);
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
				//Note: there is really no reason a delete should fail if you can "talk" to the database.
				ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
            }
            return View(salesOrder);
        }


        // GET: SalesOrder/Archive/5
        public async Task<IActionResult> Archive(int id)
        {
            var salesOrder = await _context.SalesOrders
                .FirstOrDefaultAsync(m => m.ID == id);

            if (salesOrder == null)
            {
                return NotFound();
            }

            salesOrder.Status = Status.Archived;
            _context.Update(salesOrder);

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Sales order has been archived successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error archiving sales order. Please try again.";
            }

            return RedirectToAction(nameof(Index));  // Redirect back to the Index page after archiving.
        }

        // GET: SalesOrder/Complete/5
        public async Task<IActionResult> Complete(int id)
        {
            var salesOrder = await _context.SalesOrders
                .FirstOrDefaultAsync(m => m.ID == id);

            if (salesOrder == null)
            {
                return NotFound();
            }

            // Set the status to Completed
            salesOrder.Status = Status.Completed;
            _context.Update(salesOrder);

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Sales order has been marked as completed.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error marking sales order as completed. Please try again.";
            }

            // Redirect back to the Index page after marking as completed
            return RedirectToAction(nameof(Index));
		}

  

private void PopulateAssignedSpecialtyData(SalesOrder salesOrder)
        {
            //For this to work, you must have Included the child collection in the parent object
            var allOptions = _context.Engineers;
            var currentOptionsHS = new HashSet<int>(salesOrder.SalesOrderEngineers.Select(b => b.EngineerID));
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
        private void UpdateSalesOrderEngineers(string[] selectedOptions, SalesOrder salesOrderToUpdate)
        {
            if (selectedOptions == null)
            {
                salesOrderToUpdate.SalesOrderEngineers = new List<SalesOrderEngineer>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<int>(salesOrderToUpdate.SalesOrderEngineers.Select(b => b.EngineerID));
            foreach (var s in _context.Engineers)
            {
                if (selectedOptionsHS.Contains(s.ID.ToString()))//it is selected
                {
                    if (!currentOptionsHS.Contains(s.ID))//but not currently in the Doctor's collection - Add it!
                    {
                        salesOrderToUpdate.SalesOrderEngineers.Add(new SalesOrderEngineer
                        {
                            EngineerID = s.ID,
                            SalesOrderID = salesOrderToUpdate.ID
                        });
                    }
                }
                else //not selected
                {
                    if (currentOptionsHS.Contains(s.ID))//but is currently in the Doctor's collection - Remove it!
                    {
                        SalesOrderEngineer? specToRemove = salesOrderToUpdate.SalesOrderEngineers.FirstOrDefault(d => d.EngineerID == s.ID);
                        if (specToRemove != null)
                        {
                            _context.Remove(specToRemove);
                        }
                    }
                }
            }
        }

        //private SelectList EngineerSelectList(string skip)
        //{
        //    var EngineerQuery = _context.Engineers
        //        .AsNoTracking();

        //    if (!String.IsNullOrEmpty(skip))
        //    {
        //        //Convert the string to an array of integers
        //        //so we can make sure we leave them out of the data we download
        //        string[] avoidStrings = skip.Split('|');
        //        int[] skipKeys = Array.ConvertAll(avoidStrings, s => int.Parse(s));
        //        EngineerQuery = EngineerQuery
        //            .Where(s => !skipKeys.Contains(s.ID));
        //    }
        //    return new SelectList(EngineerQuery.OrderBy(d => d.EngineerInitials), "ID", "EngineerInitials");
        //}

        //[HttpGet]
        //public JsonResult GetEngineers(string skip)
        //{
        //    return Json(EngineerSelectList(skip));
        //}


        private SelectList CustomerList(int? selectedId)
        {
            return new SelectList(_context
                .Customers
                .OrderBy(c => c.CompanyName), "ID", "CompanyName", selectedId);
                
        }

        [HttpGet]
        public JsonResult GetCustomers(int? id)
        {
            return Json(CustomerList(id));
        }

        private bool SalesOrderExists(int id)
        {
            return _context.SalesOrders.Any(e => e.ID == id);
		}

		public IActionResult DownloadSchedules()
		{

			var schedules = _context.SalesOrders
		.Include(so => so.Customer)
        .Include(so=>so.PackageRelease)// Include the related Customer entity
		.Include(so => so.Procurements)              // Include Procurements from SalesOrder
			.ThenInclude(p => p.Vendor)              // Include Vendor from Procurement
		.Include(so => so.Machines)                  // Include Machines related to SalesOrder
		.OrderByDescending(so => so.SoDate)         // Order by SalesOrder Date
		.ToList()
		.SelectMany(so => so.Machines, // Correcting this to iterate through Machines
			(so, machine) => new {
				// SalesOrder Specific Data
				SalesOrderNumber = so.OrderNumber ?? "No Order Number",
				CustomerName = so.Customer?.CompanyName ?? "No Customer",
				MachineDescription = machine?.Description ?? "No Description",
				SerialNumber = machine?.SerialNumber ?? "No Serial Number",
				PackageRelease = so.PackageRelease?.Summary ?? "No Package Release",
				// Procurement Data (Vendor & PO)
				VendorName = so.Procurements?.FirstOrDefault()?.Vendor?.Name ?? "No Vendor",
				PONumber = so.Procurements?.FirstOrDefault()?.PONumber ?? "No PO Number",
				PODueDate = so.Procurements?.FirstOrDefault()?.ExpDueDate.ToString("yyyy-MM-dd") ?? "No Due Date",
				DeliveryDate = so.Procurements?.FirstOrDefault()?.DeliveryDate.ToString("yyyy-MM-dd") ?? "No Delivery Date",
				Media = machine?.Media == true ? "Yes" : "No",
				SpareParts = machine?.SpareParts == true ? "Yes" : "No",
				Base = machine?.Base == true ? "Yes" : "No",
				AirSeal = machine?.AirSeal == true ? "Yes" : "No",
				CoatingLining = machine?.CoatingLining == true ? "Yes" : "No",
				Disassembly = machine?.Disassembly == true ? "Yes" : "No",

				// Directly accessing properties from the first machine (no Note prefix)
				PreOrder = machine?.PreOrder ?? "No PreOrder",
				Scope = machine?.Scope ?? "No Scope",
				ActualHours = machine?.ActualAssemblyHours != null
					? $"Assembly: {machine.ActualAssemblyHours} hrs / Rework: {machine.ReworkHours} hrs"
					: "No Hours Data",
				BudgetHours = machine?.BudgetedHours != null
					? $"{machine.BudgetedHours} hrs per machine"
					: "No Budget Data",
				NamePlate = machine?.Nameplate != null
					? machine.Nameplate.ToString()
					: "No Nameplate"
			}).ToList();


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
				"Sales Order", "Customer Name", "Machine Description", "Serial Number", "Package Release",
				"Vendor Name", "PO Number", "PO Due Date","Engineer Pacakage Release", "Delivery Date", "Media", "Spare Parts", "Base", "Air Seal",
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


		private void PopulateDropDownLists(SalesOrder? salesOrder = null)
		{
			ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "CompanyName");
		}
	}
}
