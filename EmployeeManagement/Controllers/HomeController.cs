using EmployeeManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using PagedList;
using System.Data;
using ClosedXML.Excel;
using System.IO;

namespace EmployeeManagement.Controllers
{

    public class HomeController : Controller
    {
        private readonly EmployeeDbContext _context = new EmployeeDbContext();
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            
            
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "Name";
            ViewBag.PositionSortParm = sortOrder == "Position" ? "position_desc" : "Position";
            ViewBag.SalarySortParm = sortOrder == "Salary" ? "salary_desc" : "Salary";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;

            }
            ViewBag.CurrentFilter = searchString;
            var emp = from e in _context.Employees select e;
            if (!String.IsNullOrEmpty(searchString))
            {
                emp = emp.Where(e => e.Name.Contains(searchString) || e.Position.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    emp = emp.OrderByDescending(e => e.Name);
                    break;
                case "Position":
                    emp = emp.OrderBy(e => e.Position);
                    break;
                case "position_desc":
                    emp = emp.OrderByDescending(e => e.Position);
                    break;
                case "Salary":
                    emp = emp.OrderBy(e => e.Salary);
                    break;
                case "salary_desc":
                    emp = emp.OrderByDescending(e => e.Salary);
                    break;
                default:
                    emp = emp.OrderBy(e => e.Name);
                    break;
            }
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                return View(emp.ToPagedList(pageNumber, pageSize));
          

          
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Employee employee)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    _context.Employees.Add(employee);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                ModelState.AddModelError("","Unable to save change . Please try again . ");
            }
            return View();
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee emp = _context.Employees.Find(id);
            if (emp == null)
            {
                return HttpNotFound();
            }
            return View(emp);
        }
        [HttpPost]
        public ActionResult Edit(Employee emp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Entry(emp).State = EntityState.Modified;
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                ModelState.AddModelError("","Unable to save change . Please try again ");
            }

            return View(emp);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            Employee emp = _context.Employees.Find(id);
            if (emp == null)
            {
                return HttpNotFound();
            }
            return View(emp);
        }
        [HttpPost]
        public ActionResult Delete(Employee emp)
        {
            try
            {

                Employee _emp = _context.Employees.Find(emp.Id);
                if (ModelState.IsValid)
                {
                    _context.Employees.Remove(_emp);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch 
            {
                ModelState.AddModelError("","Can't delete . Please try again");
            }
            return View(emp);
        }
        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee emp = _context.Employees.Find(id);
            if (emp == null)
            {
                return HttpNotFound();
            }
            return View(emp);
        }
        [HttpPost]
        public FileResult Export()
        {
           
            
            DataTable dt = new DataTable("Grid");
            //header
            dt.Columns.AddRange(new DataColumn[3] {
                new DataColumn("Name"),
                new DataColumn("Position"),
                new DataColumn("Salary")});
            //query data from database
            var emps = from emp in _context.Employees.ToList() select emp;
            //add data to datatable
            foreach (var emp in emps)
            {
                dt.Rows.Add(emp.Name, emp.Position, emp.Salary);
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                //add datatable in to excel
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    //save
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats.spreadsheetml.sheet", "Grid.xlsx");
                }
            }
           

            
        }

   
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}