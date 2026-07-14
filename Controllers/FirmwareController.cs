using IOTAssetTracking.Data;
using IOTAssetTracking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IoTDeviceManager.Controllers
{
    /// CRUD for firmware builds: list, create, edit, delete.
    public class FirmwareController : Controller
    {
        private readonly AppDBContext _context;

        public FirmwareController(AppDBContext context)
        {
            _context = context;
        }

        // List all firmware, sorted by name then version
        public async Task<IActionResult> Index()
        {
            var firmware = await _context.firmwares
                .OrderBy(f => f.FirmwareName)
                .ThenBy(f => f.Version)
                .ToListAsync();
            return View(firmware);
        }

        // GET: empty create form
        public IActionResult Create() => View();

        // POST: save a new firmware build (only bind allowed fields)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirmwareName,Version,ReleaseDate")] Firmware firmware)
        {
            if (ModelState.IsValid)
            {
                _context.Add(firmware);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Validation failed,  redisplay form with entered values
            return View(firmware);
        }

        // GET: load firmware for editing
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var firmware = await _context.firmwares.FindAsync(id);
            if (firmware == null) return NotFound();
            return View(firmware);
        }

        // POST: save edits, route id must match the form's FirmwareID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FirmwareID,FirmwareName,Version,ReleaseDate")] Firmware firmware)
        {
            if (id != firmware.FirmwareID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(firmware);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Someone else deleted it, or the row changed under us
                    if (!await _context.firmwares.AnyAsync(f => f.FirmwareID == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(firmware);
        }

        // GET: confirm delete page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var firmware = await _context.firmwares.FirstOrDefaultAsync(f => f.FirmwareID == id);
            if (firmware == null) return NotFound();
            return View(firmware);
        }

        // POST: remove the firmware after confirmation
        // delete may fail if devices still reference this firmware
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var firmware = await _context.firmwares.FindAsync(id);
            if (firmware != null)
            {
                _context.firmwares.Remove(firmware);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}