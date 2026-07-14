using IOTAssetTracking.Data;
using IOTAssetTracking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IoTDeviceManager.Controllers
{
    // CRUD for groups, including parent/child hierarchy display.
    public class GroupsController : Controller
    {
        private readonly AppDBContext _context;

        public GroupsController(AppDBContext context)
        {
            _context = context;
        }

        // List groups in tree order (parent, then children) with Depth for indentation
        public async Task<IActionResult> Index()
        {
            var allGroups = await _context.groups
                .Include(g => g.ParentGroup)
                .OrderBy(g => g.GroupName)
                .ToListAsync();

            // Lookup children by ParentGroupID
            var byParent = allGroups.ToLookup(g => g.ParentGroupID);
            var ordered = new List<Group>();

            void AddChildren(int? parentId, int depth)
            {
                foreach (var group in byParent[parentId])
                {
                    group.Depth = depth;
                    ordered.Add(group);
                    AddChildren(group.GroupID, depth + 1);
                }
            }

            AddChildren(null, 0);
            return View(ordered);
        }

        // GET: empty create form with parent dropdown
        public async Task<IActionResult> Create()
        {
            ViewBag.ParentGroupID = new SelectList(
                await _context.groups.OrderBy(g => g.GroupName).ToListAsync(),
                "GroupID", "GroupName");
            return View();
        }

        // POST: save a new group (only bind allowed fields)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GroupName,ParentGroupID")] Group group)
        {
            if (ModelState.IsValid)
            {
                _context.Add(group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Validation failed, redisplay form with dropdown and entered values
            ViewBag.ParentGroupID = new SelectList(
                await _context.groups.OrderBy(g => g.GroupName).ToListAsync(),
                "GroupID", "GroupName", group.ParentGroupID);
            return View(group);
        }

        // GET: load group for editing (exclude itself from parent options)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var group = await _context.groups.FindAsync(id);
            if (group == null) return NotFound();

            var possibleParents = await _context.groups
                .Where(g => g.GroupID != id)
                .OrderBy(g => g.GroupName)
                .ToListAsync();
            ViewBag.ParentGroupID = new SelectList(possibleParents, "GroupID", "GroupName", group.ParentGroupID);
            return View(group);
        }

        // POST: save edits, block setting a group as its own parent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GroupID,GroupName,ParentGroupID")] Group group)
        {
            if (id != group.GroupID) return NotFound();

            if (group.ParentGroupID == id)
            {
                ModelState.AddModelError(nameof(group.ParentGroupID), "A group cannot be its own parent.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(group);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Someone else deleted it, or the row changed under us
                    if (!await _context.groups.AnyAsync(g => g.GroupID == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var possibleParents = await _context.groups
                .Where(g => g.GroupID != id)
                .OrderBy(g => g.GroupName)
                .ToListAsync();
            ViewBag.ParentGroupID = new SelectList(possibleParents, "GroupID", "GroupName", group.ParentGroupID);
            return View(group);
        }

        // GET: confirm delete page (show parent name if any)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var group = await _context.groups
                .Include(g => g.ParentGroup)
                .FirstOrDefaultAsync(g => g.GroupID == id);
            if (group == null) return NotFound();

            return View(group);
        }

        // POST: remove the group after confirmation
        // delete may fail if child groups still reference it
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var group = await _context.groups.FindAsync(id);
            if (group != null)
            {
                _context.groups.Remove(group);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}