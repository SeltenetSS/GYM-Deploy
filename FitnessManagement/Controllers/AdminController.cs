﻿using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models;
using FitnessManagement.Dtos;
using FitnessManagement.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace FitnessManagement.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
      
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IUserService _userService;
        private readonly ITrainerService _trainerService;
        private readonly IUserDal _userDal;



        public AdminController(UserManager<ApplicationUser> userManager, IUserService userService, IUserDal userDal, ITrainerService trainerService)
        {
            _userManager = userManager;
            _userService = userService;
            _userDal = userDal;
            _trainerService=trainerService;
        }
        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser([FromForm] UserDto userDto)
        {
            if(userDto==null)
            {
                return BadRequest(new { Status = "Error", Message = "User data is required!" });
            }
            await _userService.AddUser(userDto);
            return Ok("User added successfully!");

        }
        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromForm] UserUpdateDto userUpdateDto)
        {
            try
            {
                await _userService.UpdateUser(userUpdateDto);
                return Ok(new { message = "User updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteUser(id);
                return Ok(new { message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound("Not Found");
            }
            return Ok(user);
        }
        [HttpGet("pending-users")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var pendingUsers = await _userService.GetPendingUsers();

            if (!pendingUsers.Any())
            {
                return NotFound(new { Status = "Error", Message = "No pending users found!" });
            }

            return Ok(pendingUsers.Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.UserName
            }));
        }

        [HttpGet("pending-trainers")]
        public async Task<IActionResult> GetPendingTrainers()
        {
            var pendingTrainers = await _trainerService.GetPendingTrainers();

            if (!pendingTrainers.Any())
            {
                return NotFound(new { Status = "Error", Message = "No pending trainers found!" });
            }

            return Ok(pendingTrainers.Select(t => new
            {
                t.Id,
                t.FullName,
                t.Email,
                t.UserName
            }));
        }


        [HttpPost("approve-user/{userId}")]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            try
            {
                await _userService.ApproveUser(userId);
                return Ok(new { Status = "Success", Message = "User approved successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = "Error", Message = ex.Message });
            }
        }


        [HttpPost("approve-trainer/{trainerId}")]
        public async Task<IActionResult> ApproveTrainer(string trainerId)
        {
            try
            {
                await _trainerService.ApproveTrainer(trainerId);
                return Ok(new { Status = "Success", Message = "Trainer approved successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = "Error", Message = ex.Message });
            }
        }

      
        [HttpPost("add-trainer")]
        public async Task<IActionResult> AddTrainer([FromForm] TrainerDto trainerDto)
        {
            if (trainerDto == null)
            {
                return BadRequest(new { Status = "Error", Message = "Trainer data is required!" });
            }
            await _trainerService.AddTrainer(trainerDto);
            return Ok("Trainer added successfully!");

        }

    }
}
