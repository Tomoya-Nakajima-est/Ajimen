using Ajimen.Data;
using Ajimen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ajimen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderLogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderLogController(AppDbContext context)
        {
            _context = context;
        }

        // 発注確定
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmOrder([FromBody] OrderLog order)
        {
            if (order == null || order.ItemId == 0 || order.OrderNum <= 0)
            {
                return BadRequest("不正な発注情報です。");
            }

            // 発注日とログ記録時間の設定
            order.OrderDay = DateTime.Now;
            order.useOrderLog = DateTime.Now;

            _context.OrderLogs.Add(order);

            // 在庫を更新
            var stock = await _context.StockLogs.FirstOrDefaultAsync(s => s.ItemId == order.ItemId);

            if (stock != null)
            {
                stock.StockNum += order.OrderNum; // OrderNum は発注数として使用
                stock.StockDay = DateTime.Now;
                _context.StockLogs.Update(stock);
            }
            else
            {
                // 在庫ログがない場合は新規作成
                var newStock = new StockLog
                {
                    StaffId = order.StaffId,
                    StaffName = order.StaffName,
                    StockDay = DateTime.Now,
                    StockNum = order.OrderNum,
                    StockMinNum = 0, // 必要に応じて変更
                    ItemId = order.ItemId,
                    ItemName = order.ItemName,
                    ItemKinds = order.ItemKinds
                };
                _context.StockLogs.Add(newStock);
            }

            await _context.SaveChangesAsync();
            return Ok("発注と在庫更新が完了しました。");
        }
    }
}