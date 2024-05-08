using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Taxi_App;

[Authorize(Roles = "Admin")]
public class AdminController : BaseApiController
{
    private readonly IUserRepository _userRepo;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public AdminController(IUserRepository userRepo, IEmailService emailService, IMapper mapper)
    {
        _userRepo = userRepo;
        _emailService = emailService;
        _mapper = mapper;
    }

    [HttpPatch("accept-verification/{id}")]
    public async Task<ActionResult> AcceptVerification(int id)
    {
        var user = await _userRepo.GetUserByIdAsync(id);

        if (user == null) return NotFound("User doen't exist!");

        if (user.VerificationStatus != EVerificationStatus.IN_PROGRESS) return BadRequest("Can't change verification anymore!");

        var userVerified = await _userRepo.AcceptVerification(user.Id);
        
        await _emailService.SendEmail(user.Email, userVerified.VerificationStatus.ToString());

        var userToReturn = _mapper.Map<VerificationDto>(user);

        return Ok(userToReturn);
    }

    [HttpPatch("deny-verification/{id}")]
    public async Task<ActionResult> DenyVerification(int id)
    {
        var user = await _userRepo.GetUserByIdAsync(id);

        if (user == null) return NotFound("User doen't exist!");

        if (user.VerificationStatus != EVerificationStatus.IN_PROGRESS) return BadRequest("Can't change verification anymore!");

        var userVerified = await _userRepo.DenyVerification(user.Id);
        
        await _emailService.SendEmail(user.Email, userVerified.VerificationStatus.ToString());

        var userToReturn = _mapper.Map<VerificationDto>(user);

        return Ok(userToReturn);
    }
}
