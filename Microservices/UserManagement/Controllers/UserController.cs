using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models;
using UserManagement.DTOs;
using UserManagement.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using static UserManagement.Interfaces.IUserServices;
using UserManagement.DTOs;

namespace UserManagement.Controllers;
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly Interfaces.IUserServices _userSevices;


    public UserController(Interfaces.IUserServices userServices)
    {
        _userSevices = userServices;

    }

    [HttpGet("getUsers")]

    public async Task<IActionResult> GetUsers()
    {
        var response = await _userSevices.GetUsers();
        if (response.Count == 0)
        {
            return NotFound("No users found");
        }

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("register")]

    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        var response = await _userSevices.Register(request);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("login")]

    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        var user = await _userSevices.Login(request);

        var token = _userSevices.GenerateWebToken(user);
        //HttpContext.Session.SetString("UserId", user.Id.ToString());



        //string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(token);

    }

    [AllowAnonymous]
    [HttpPut("reset-password")]

    public async Task<IActionResult> ResetPassword(ResetRequestDto request)
    {
        var response = await _userSevices.Reset(request);
        return Ok(response);
    }



}