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
using Microsoft.EntityFrameworkCore;
using UserManagement.Exceptions;
using OrderManagement.Services;

namespace UserManagement.Controllers;
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly Interfaces.IUserServices _userServices;


    public UserController(Interfaces.IUserServices userServices)
    {
        _userServices = userServices;

    }

    /*[HttpGet("getUsers")]

    public async Task<IActionResult> GetUsers()
    {
        var response = await _userServices.GetUsers();
        if (response.Count == 0)
        {
            return NotFound("No users found");
        }

        return Ok(response);
    }

    [HttpGet("getUserById")]

    public async Task<IActionResult> GetById(int UserId)
    {
        var response = _userServices.GetById(UserId);
        if (response == null)
        {
            return NotFound("No users found with the Id");
        }

        return Ok(response);
    }
    [AllowAnonymous]
    [HttpGet("getNameHindi")]

    public async Task<IActionResult> GetNameHindi(string UserName)
    {
        var response =  _userServices.GetNameHindi(UserName);
        if (response == null)
        {
            return NotFound("No users found with the username");
        }

        return Ok(response);
    }
    [AllowAnonymous]
    [HttpDelete("deleteUser")]

    public async Task<IActionResult> DeleteUser(string UserName)
    {
        var response =await  _userServices.DeleteUser(UserName);
        if (response == null)
        {
            return NotFound("No users found with the username");
        }

        return Ok(response);
    }*/
    [AllowAnonymous]
    [HttpGet("getNameHindi")]

    public async Task<IActionResult> GetNameHindi(string UserName)
    {
        var response = _userServices.GetNameHindi(UserName);
        if (response == null)
        {
            return NotFound("No users found with the username");
        }

        return Ok(response);
    }
    [AllowAnonymous]
    [HttpPost("register")]

    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        try
        {
            var response = await _userServices.Register(request);

            return Ok(response);

        }
        catch (ArgumentsException ex)
        {
            return BadRequest(ex.Message);
        }
    }
   
    [AllowAnonymous]
    [HttpPost("login")]

    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        try
        {
            var user = await _userServices.Login(request);

            var token = _userServices.GenerateWebToken(user);
            //HttpContext.Session.SetString("UserId", user.Id.ToString());



            //string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(token);

        }
        catch (ArgumentsException ex)
        {
            return Unauthorized(ex.Message);
        }

    }

    [AllowAnonymous]
    [HttpPut("reset-password")]

    public async Task<IActionResult> ResetPassword(ResetRequestDto request)
    {
        try
        {
            var response = await _userServices.Reset(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

}