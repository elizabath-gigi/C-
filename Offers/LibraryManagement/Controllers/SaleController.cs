using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using static System.Reflection.Metadata.BlobBuilder;
using static Azure.Core.HttpHeader;
using LibraryManagement.Exceptions;
using LibraryManagement.DTOs;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class SaleController:ControllerBase
    { 


        readonly ISalesServices _salesServices;
        public SaleController(Interfaces.ISalesServices saleServices)
        {
            _salesServices = saleServices;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("getAllSales")]
        public async Task<ActionResult> GetAllSales()
        {
            var sale = await  _salesServices.GetAllSales();
            if (sale.Count == 0)
            {
                return NotFound("No books found.");
            }

            return Ok(sale);

        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("addSales")]
        public async Task<ActionResult> AddSales(SaleDto request)
        {
            try
            {
                var sales = await _salesServices.AddSales(request);
                return Ok(sales );
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }



        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("updateSales")]
        public async Task<ActionResult> UpdateSales(SaleDto request)
        {
            try
            {
                var sales = await _salesServices.UpdateSales(request);
                return Ok(sales);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }
         }
        /*[Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("deleteSales")]
        public async Task<ActionResult> DeleteSales(int id)
        {
            try
            {
                var sale = await _salesServices.DeleteSales(id);
                return Ok(sale);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }
        }*/
    }
}
