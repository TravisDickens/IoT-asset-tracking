using IOTAssetTracking.Data;
using IOTAssetTracking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IoTDeviceManager.Controllers
{
    //CRUD for devices: list, create, edit, delete, plus firmware/group dropdowns.
    public class DevicesController : Controller
    {
        private readonly AppDBContext _context;

        public DevicesController(AppDBContext context)
        {
            _context = context;
        }

        // List all devices with firmware and group loaded for the table
        public async Task<IActionResult> Index()
        {
            var devices = await _context.devices
                .Include(d => d.Firmware)
                .Include(d => d.Group)
                .OrderBy(d => d.DeviceName)
                .ToListAsync();
            return View(devices);
        }

        // GET: empty create form with dropdown options
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        // POST: save a new device (only bind allowed fields)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SerialNumber,DeviceName,FirmwareID,GroupID")] Device device)
        {
            if (ModelState.IsValid)
            {
                _context.Add(device);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Validation failed — redisplay form with dropdowns and entered values
            await PopulateDropdowns(device);
            return View(device);
        }

        // GET: load device for editing
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var device = await _context.devices.FindAsync(id);
            if (device == null) return NotFound();
            await PopulateDropdowns(device);
            return View(device);
        }

        // POST: save edits, route id must match the form's DeviceID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DeviceID,SerialNumber,DeviceName,FirmwareID,GroupID")] Device device)
        {
            if (id != device.DeviceID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(device);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Someone else deleted it, or the row changed under us
                    if (!await _context.devices.AnyAsync(d => d.DeviceID == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdowns(device);
            return View(device);
        }

        // GET: confirm delete page (show related firmware/group names)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var device = await _context.devices
                .Include(d => d.Firmware)
                .Include(d => d.Group)
                .FirstOrDefaultAsync(d => d.DeviceID == id);
            if (device == null) return NotFound();
            return View(device);
        }

        // POST: remove the device after confirmation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var device = await _context.devices.FindAsync(id);
            if (device != null)
            {
                _context.devices.Remove(device);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Fill ViewBag firmware/group lists for Create and Edit forms
        private async Task PopulateDropdowns(Device? device = null)
        {
            ViewBag.FirmwareID = new SelectList(
                await _context.firmwares.OrderBy(f => f.FirmwareName).ToListAsync(),
                "FirmwareID", "FirmwareName", device?.FirmwareID);

            ViewBag.GroupID = new SelectList(
                await _context.groups.OrderBy(g => g.GroupName).ToListAsync(),
                "GroupID", "GroupName", device?.GroupID);
        }
    }
}