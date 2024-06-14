using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Models;
using OrderManagement.DTOs;
using OrderManagement.Interface;

namespace OrderManagement.Controllers;
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IAuthenticationServices _authenticationService;

    public UserController(IAuthenticationServices authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpGet("getUsers")]
   
    public async Task<IActionResult> GetUsers()
    {
        var response = await _authenticationService.GetUsers();
        if(response.Count == 0) 
        { 
            return NotFound("No users found");
        }

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("register")]
   
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        var response = await _authenticationService.Register(request);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        var response = await _authenticationService.Login(request);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPut("reset-password")]
    
    public async Task<IActionResult> ResetPassword(ResetRequestDto request)
    {
        var response = await _authenticationService.Reset(request);
        return Ok(response);
    }
}