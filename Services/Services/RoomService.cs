using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Infrastructures;
using Repositories.Models;
using Services.Interfaces;

namespace Services.Services
{
    public class RoomService : IRoomService
    {
        private readonly UnitOfWork _unitOfWork;

        public RoomService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Room> CreateRoomAsync(string name)
        {
            var room = new Room
            {
                Name = name
            };

            var createdRoom = _unitOfWork.RoomRepository.AddEntity(room);
            await _unitOfWork.CompleteAsync();
            return createdRoom;
        }

        public async Task<Room> GetRoomByCodeAsync(int roomCode)
        {
            return  _unitOfWork.RoomRepository.GetRoomByCode(roomCode);
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return _unitOfWork.RoomRepository.GetAll();
        }

        public async Task<Room> GetRoomByIdAsync(Guid id)
        {
            return _unitOfWork.RoomRepository.GetEntityByGuid(id);
        }

        public async Task<bool> ToggleRoomActiveStatusAsync(Guid roomId)
        {
            var room = _unitOfWork.RoomRepository.GetEntityByGuid(roomId);

            if (room == null)
                return false;


            room.IsActive = !room.IsActive;

            _unitOfWork.RoomRepository.UpdateEntity(room);


            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
