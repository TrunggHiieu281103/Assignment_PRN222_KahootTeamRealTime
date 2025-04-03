using KahootTeamRealTimeAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace KahootTeamRealTimeAdmin.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // GET: Rooms
        public async Task<IActionResult> Index(string filter)
        {
            var rooms = await _roomService.GetAllRoomsAsync();

            // Apply filtering if specified
            if (!string.IsNullOrEmpty(filter))
            {
                rooms = filter.ToLower() switch
                {
                    "active" => rooms.Where(r => r.IsActive),
                    "inactive" => rooms.Where(r => !r.IsActive),
                    _ => rooms
                };
            }

            return View(rooms);
        }

        // GET: Rooms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _roomService.CreateRoomAsync(model.Name);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            var success = await _roomService.ToggleRoomActiveStatusAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
