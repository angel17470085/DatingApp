using System;
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

            if (await _userRepository.SaveAllAsync()) 
            {
                return CreatedAtAction("GetUser",
                 new{username = user.UserName}, _mapper.Map<PhotoDto>(photo));
            }
          
            return BadRequest("Problem adding photo"); 
        }

        [HttpPut("set-main-photo/{photoId}")]
               public async Task<ActionResult> SetMainPhoto (int photoId) 
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null)
            {
                return NotFound();
            }

            if (photo.IsMain)
            {
                return BadRequest("this is already your main photo"); 
            }

                var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

                if (currentMain != null)
                {
                    currentMain.IsMain = false;
                }

                photo.IsMain = true;

                if (await _userRepository.SaveAllAsync())
                {
                    return  NoContent();
                }

                return BadRequest("Problem setting the main photo");
        }

        [HttpDelete("delte-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto (int photoId) 
        { 
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain)
            {
                return BadRequest("you can not delete your main photo");
            }

            if (photo.PublicId != null) 
            {
                //delete the photos from cloudinary
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result != null) return BadRequest(result.Error.Message);
            }
            //PublicId is equal to null, it means that the photo is in the Db.
            user.Photos.Remove(photo);

            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest ("Problem deleting photo");
        }

    }

 
}