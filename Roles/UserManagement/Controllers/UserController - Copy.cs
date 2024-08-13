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
[Authorize(Roles = "Admin")]
[ApiController]
[Route("[controller]")]
public class AdminController : ControllerBase
{
    private readonly Interfaces.IUserServices _userServices;


    public AdminController(Interfaces.IUserServices userServices)
    {
        _userServices = userServices;

    }
    
    [HttpGet("getUsers")]
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
        var response =  _userServices.GetById(UserId);
        if (response == null)
        {
            return NotFound("No users found with the Id");
        }

        return Ok(response);
    }

    [HttpGet("getUserByUsername")]

    public async Task<IActionResult> GetByUsername(string UserName)
    {
        var response = _userServices.GetByUsername(UserName);
        if (response == null)
        {
            return NotFound("No users found with the Id");
        }

        return Ok(response);
    }

    [HttpDelete("deleteUser")]

    public async Task<IActionResult> DeleteUser(string UserName)
    {
        var response = await _userServices.DeleteUser(UserName);
        if (response == null)
        {
            return NotFound("No users found with the username");
        }

        return Ok(response);
    }
 }