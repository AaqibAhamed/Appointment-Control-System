using AppointmentControlSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentControlSystem.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        [BindProperty]
        public Appointment Appointment { get; set; }

        public AppointmentsController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Update and Create (Upsert) Actions joined to gether to avoid more code 
        //DRY - As per Don't Repeat Yourself
        public IActionResult Upsert(int? Id)
        {
            Appointment = new Appointment();

            if (Id == null)
            {
                //Create
                return View(Appointment);
            }
            //Update
            Appointment = dbContext.Appointment.FirstOrDefault(u => u.Id == Id);
            if (Appointment == null)
            {
                return NotFound();
            }

            return View(Appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (Appointment.Id == 0)
                {
                    //Create
                    dbContext.Appointment.Add(Appointment);
                }
                else
                {
                    //Update
                    dbContext.Appointment.Update(Appointment);
                }
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Appointment);
        }

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await dbContext.Appointment.ToListAsync() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var appointmentFromDb = await dbContext.Appointment.FirstOrDefaultAsync(u => u.Id == id);
            if (appointmentFromDb == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            dbContext.Appointment.Remove(appointmentFromDb);
            await dbContext.SaveChangesAsync();
            return Json(new { success = true, message = "Delete successful" });

        }
        #endregion

    }
}
