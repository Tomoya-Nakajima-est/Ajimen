using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Ajimen.Data;
using Ajimen.Models;
using Ajimen.DTOs;

[ApiController]
[Route("api/[controller]")]
public class ShiftController : ControllerBase
{
    private readonly AppDbContext _context;

    public ShiftController(AppDbContext context)
    {
        _context = context;
    }

    // ✅ カレンダー用：月別シフト取得
    [HttpGet("get")]
    public IActionResult GetShifts([FromQuery] int year, [FromQuery] int month)
    {
        var shifts = _context.Shifts
            .Where(s => s.ShiftDay.Year == year && s.ShiftDay.Month == month)
            .ToList();

        // 日付ごとに A, B いずれかの申請があるかどうかをまとめる
        var result = shifts
            .GroupBy(s => s.ShiftDay.Date)
            .Select(g => new {
                day = g.Key.Day,
                hasA = g.Any(s => s.ShiftSelect == "A"),
                hasB = g.Any(s => s.ShiftSelect == "B")
            })
            .ToList();

        return Ok(result);
    }

    // ✅ 登録処理：申請内容を受け取る
    [HttpPost("apply")]
    public IActionResult ApplyShift([FromBody] ShiftApplyRequestDto dto)
    {
        var shiftId = $"{dto.Day:yyyyMMdd}-{dto.ShiftSelect}";
        var shift = _context.Shifts.FirstOrDefault(s => s.ShiftId == shiftId);

        if (shift == null)
        {
            // 新規作成
            shift = new Shift
            {
                ShiftId = shiftId,
                ShiftDay = dto.Day,
                ShiftSelect = dto.ShiftSelect,
                ShiftMembers = new List<string> { dto.MemberId },
                UseShiftLog = dto.MemberId
            };
            _context.Shifts.Add(shift);
        }
        else
        {
            // すでにある場合、追加（重複防止）
            if (!shift.ShiftMembers.Contains(dto.MemberId))
            {
                shift.ShiftMembers.Add(dto.MemberId);
                shift.UseShiftLog = dto.MemberId;
            }
        }

        _context.SaveChanges();
        return Ok(new { success = true });
    }

    // ✅ 対象日に申請済みかどうか確認（任意で使用）
    [HttpGet("check")]
    public IActionResult CheckShift([FromQuery] DateTime day, [FromQuery] string memberId)
    {
        var result = _context.Shifts
            .Where(s => s.ShiftDay == day && s.ShiftMembers.Contains(memberId))
            .Select(s => new { s.ShiftSelect })
            .ToList();

        return Ok(result);
    }
    [HttpGet("all")]
    public IActionResult GetAllShifts()
    {
        var shifts = _context.Shifts.ToList();
        return Ok(shifts);
    }
}