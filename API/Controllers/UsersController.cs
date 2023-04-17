using System.Security.Claims;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using AutoMapper;
using API.DTOs;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper )
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();

            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
           var user = await _userRepository.GetMemberAsync(username);

            return user;


        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(MemberUpdateDto updateMemberDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var user = await  _userRepository.GetUserByUserNameAsync(username);

            if(user == null)
            {
                return NotFound();
            }

            _mapper.Map(updateMemberDto,user);

            if (await _userRepository.SaveAllAsync())
            {
                return NoContent();
            } 
            
            return BadRequest("Failed to update user...");
           
        }
    }
}