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
        try
        {
            var response = _userServices.GetById(UserId);
            return Ok(response);
        }
        catch(IdNotFoundException ex) 
        {
            return BadRequest(ex.Message);
        }
        
    }

    [HttpGet("getUserByUsername")]

    public async Task<IActionResult> GetByUsername(string UserName)
    {
        try
        {
            var response = _userServices.GetByUsername(UserName);
            return Ok(response);
        }
         catch (IdNotFoundException ex) 
        { 
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("deleteUser")]

    public async Task<IActionResult> DeleteUser(string UserName)
    {
        try
        {
            var deletedBy = User.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
            var response = await _userServices.DeleteUser(UserName,deletedBy);
            return Ok(response);
        }
        catch (IdNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }
 }