﻿// Controllers/StockController.cs
using Ajimen.Data;
using Ajimen.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ajimen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StockController(AppDbContext context)
        {
            _context = context;
        }

        // 在庫操作履歴を取得
        [HttpGet("logs")]
        public IActionResult GetStockLogs()
        {
            return Ok(_context.StockLogs.ToList());
        }       
    }
}