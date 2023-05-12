using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Extensions;
using API.Entities;
using API.Helpers;
namespace API.Controllers
{

    public class MessagesController : BaseApiController
    {
       
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        
        private readonly IMapper _mapper;

        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage (CreateMessageDto createMessageDto) 
        {
            var username = User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("you cannot send messages to yourself...");

            var sender = await _userRepository.GetUserByUserNameAsync(username);
            var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername); 
            
            if (recipient == null) return NotFound();

            var message = new Message{
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = createMessageDto.Content
            };


            _messageRepository.AddMessage(message);

            if(await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("failed to send message");
             

        }


        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessageForUser([FromQuery] MessageParams messageParams) 
        {
            messageParams.Username = User.GetUsername();

            var messages = await _messageRepository.GetMessageForUser(messageParams);

            Response.AddPaginationHeader(
                new PaginationHeader(
                    messages.CurrentPage,
                    messages.PageSize,
                    messages.TotalCount,
                    messages.TotalPages
                )
            );

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();
            
           return Ok( await _messageRepository.GetMessageThread(currentUsername, username)); 

        }
    }
}