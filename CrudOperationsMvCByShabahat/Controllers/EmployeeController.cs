using CrudOperationsMvCByShabahat.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrudOperationsMvCByShabahat.Controllers
{
    public class EmployeeController : Controller
    {
        EmployeeManagementDbEntities db = new EmployeeManagementDbEntities();

        // GET: Employee
        public ActionResult Index()
        {

            List<Department> list = db.Departments.ToList();
            ViewBag.DepartmentList = new SelectList(list, "DepartmentId", "DepartmentName");

            List<EmployeeViewModel> listEmp = db.Employees.Where(x => x.IsDeleted == false).Select(x => new EmployeeViewModel
            {
                EmployeeId = x.EmployeeId,
                Name = x.Name,
                Address = x.Address,
                DepartmentName = x.Department.DepartmentName
            }).ToList();
            ViewBag.EmployeeList = listEmp;

            return View();
        }


        [HttpPost]
        public ActionResult Index(EmployeeViewModel model)
        {
            try
            {

                List<Department> list = db.Departments.ToList();
                ViewBag.DepartmentList = new SelectList(list, "DepartmentId", "DepartmentName");

                Employee emp = new Employee();
                emp.Address = model.Address;
                emp.Name = model.Name;
                emp.DepartmentId = model.DepartmentId;
                emp.IsDeleted = false;

                db.Employees.Add(emp);
                db.SaveChanges();

                int latestEmpId = emp.EmployeeId;


                Site site = new Site();
                site.SiteName = model.SiteName;
                site.EmployeeId = latestEmpId;

                db.Sites.Add(site);
                db.SaveChanges();
                return View ();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public JsonResult Get(int EmployeeId)
        {
            Employee model = db.Employees.Where(x => x.EmployeeId == EmployeeId).SingleOrDefault();

            string value = string.Empty;
            value = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(employee, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteEmployee(int EmployeeId)
        {
            bool result = false;
            Employee emp = db.Employees.SingleOrDefault(x => x.IsDeleted == false && x.EmployeeId == EmployeeId);
            if (emp != null)
            {
                emp.IsDeleted = true;
                db.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}