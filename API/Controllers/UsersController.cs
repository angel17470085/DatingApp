using System.Reflection.Metadata;
using System.Security.Claims;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using AutoMapper;
using API.DTOs;
using API.Extensions;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper , IPhotoService photoService )
        {
            _photoService = photoService;
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

            return  Ok(user); 


        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto updateMemberDto)
        {
            var username = User.GetUsername();
            
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

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto ( IFormFile file) 
        { 
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());

            if (user == null) 
            {
                return NotFound();
            }

            var result = await _photoService.AddPhotoAync(file);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            var photo = new Photo{
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            
            if(user.Photos.Count == 0) photo.IsMain = true ;
            
            user.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync()) return _mapper.Map<PhotoDto>(photo);
          
            return BadRequest("Problem adding photo"); 
        }

      

    }
}