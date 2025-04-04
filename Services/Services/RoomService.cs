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

        public async Task<List<Question>> GetRoomQuestion(Guid roomId)
        {
            return  _unitOfWork.RoomRepository.GetRoomQuestion(roomId);
        }

        public Task<bool> RemoveUserFromRoom(Guid userId, Guid roomId)
        {

            return _unitOfWork.RoomRepository.RemoveUserFromRoom(userId, roomId);
        }

        public async Task<bool> IsUserInRoom(Guid userId, Guid roomId)
        {
            return await _unitOfWork.RoomRepository.IsUserInRoom(userId, roomId);
        }

        public async Task<bool> AddUserToRoom(Guid userId, Guid roomId)
        {
            return await _unitOfWork.RoomRepository.AddUserToRoom(userId, roomId);
        }
        public async Task<List<(string Name, int Points)>> GetUserScoresByRoomCode(int roomCode)
        {
            return await _unitOfWork.RoomRepository.GetUserScoresByRoomCode(roomCode);
        }

        public async Task<List<string>> GetUsernamesByRoomCode(int roomCode)
        {
            return await _unitOfWork.RoomRepository.GetUsernamesByRoomCode(roomCode);
        }
    }
}
