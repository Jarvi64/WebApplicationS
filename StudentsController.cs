using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationS;
using WebApplicationS.Models;
namespace WebApplicationS.Controllers
{
    public class StudentsController : Controller
    {
        private readonly StudentDbContext _context;
        public StudentsController(StudentDbContext context)
        {
            _context = context;
        }

        private void PopulateDeptDropDownList(object? selectedDepts = null)
        {
            var deptsQuery = from s in _context.Department
                             orderby s.DeptName
                             select new { DeptId = s.ID, s.DeptName };
            ViewBag.DeptId = new SelectList(deptsQuery.AsNoTracking(), "Id", "DeptName", selectedDepts);
        }
        // GET: Students	
        public ActionResult Index(string sortOrder, string searchString, string departmentFilter)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DepartmentFilter = departmentFilter;
            var dbContext_Demo = _context.Student.Include(s => s.department);
            var students = from s in dbContext_Demo
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.Name.Contains(searchString)
                                       || s.department.DeptName.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(departmentFilter))
            {
                students = students.Where(s => s.department.DeptName == departmentFilter);
            }

            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.Name);
                    break;

                default:
                    students = students.OrderBy(s => s.Name);
                    break;
            }
            ViewBag.Departments = _context.Department.Select(d => d.DeptName).Distinct().ToList();
            return View(students.ToList());
        }
       
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Student == null)
            {
                return NotFound();
            }
            var student = await _context.Student
                .Include(s => s.department)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }
        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["DeptID"] = new SelectList(_context.Department, "ID", "DeptName");
            return View();
        }
        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Class,HasPRN,Gender,DeptID")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DeptID"] = new SelectList(_context.Department, "ID", "DeptName", student.DeptID);
            return View(student);
        }
        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Student == null)
            {
                return NotFound();
            }
            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["DeptID"] = new SelectList(_context.Department, "ID", "DeptName", student.DeptID);
            return View(student);
        }
        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Class,HasPRN,Gender,DeptID")] Student student)
        {
            if (id != student.ID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.ID))
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
            ViewData["DeptID"] = new SelectList(_context.Department, "ID", "DeptName", student.DeptID);
            return View(student);
        }
        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Student == null)
            {
                return NotFound();
            }
            var student = await _context.Student
                .Include(s => s.department)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }
        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Student == null)
            {
                return Problem("Entity set 'StudentDbContext.Student'  is null.");
            }
            var student = await _context.Student.FindAsync(id);
            if (student != null)
            {
                _context.Student.Remove(student);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool StudentExists(int id)
        {
            return (_context.Student?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}