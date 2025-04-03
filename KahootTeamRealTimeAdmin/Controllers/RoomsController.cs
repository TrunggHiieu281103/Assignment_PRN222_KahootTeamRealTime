using KahootTeamRealTimeAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace KahootTeamRealTimeAdmin.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    // GET: Rooms
    public async Task<IActionResult> Index()
    {
        var rooms = await _roomService.GetAllRoomsAsync();
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
    }
}
