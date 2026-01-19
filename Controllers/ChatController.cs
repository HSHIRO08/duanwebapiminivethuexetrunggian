using DataAccess.Context;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace duanminiveprogresql.Controllers
{
    public class ChatController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ChatController> _logger;

        public ChatController(AppDbContext context, ILogger<ChatController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Trang chat
        public IActionResult Index()
        {
            return View();
        }

        // API: Gửi tin nhắn và nhận phản hồi
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var sessionId = request.SessionId ?? Guid.NewGuid().ToString();

                // Lưu tin nhắn người dùng
                var userMessage = new Chatmessage
                {
                    Nguoidungid = userId,
                    Sessionid = sessionId,
                    Noidung = request.Message,
                    Loaitinnhan = "User",
                    Thoigian = DateTime.Now,
                    Dadoc = false
                };
                _context.Chatmessages.Add(userMessage);
                await _context.SaveChangesAsync();

                // Xử lý và tạo câu trả lời
                var botResponse = await ProcessMessage(request.Message, userId, sessionId);

                // Lưu phản hồi của bot
                var botMessage = new Chatmessage
                {
                    Nguoidungid = null, // Bot không có userId
                    Sessionid = sessionId,
                    Noidung = botResponse,
                    Loaitinnhan = "Bot",
                    Thoigian = DateTime.Now,
                    Dadoc = false
                };
                _context.Chatmessages.Add(botMessage);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    response = botResponse,
                    sessionId = sessionId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return Json(new
                {
                    success = false,
                    response = "Xin lỗi, có lỗi xảy ra. Vui lòng thử lại sau."
                });
            }
        }

        // Logic xử lý tin nhắn thông minh
        private async Task<string> ProcessMessage(string message, int? userId, string sessionId)
        {
            message = message.ToLower().Trim();

            // 1. CHÀO HỎI
            if (IsGreeting(message))
            {
                return "Xin chào! Tôi là trợ lý ảo của Nhoang Cho Thuê Xe Ô Tô.\n\n" +
                       "Tôi có thể giúp bạn:\n" +
                       "• Tìm xe phù hợp\n" +
                       "• Xem bảng giá\n" +
                       "• Hướng dẫn thuê xe\n" +
                       "• Liên hệ hỗ trợ\n" +
                       "• Trả lời câu hỏi\n\n" +
                       "Bạn cần tôi hỗ trợ điều gì?";
            }

            // 2. TÌM XE / XEM DANH SÁCH XE
            if (message.Contains("xe") || message.Contains("danh sách") || message.Contains("xem xe"))
            {
                if (message.Contains("suv") || message.Contains("7 chỗ"))
                {
                    return await GetCarsByType("SUV");
                }
                else if (message.Contains("sedan") || message.Contains("4 chỗ") || message.Contains("5 chỗ"))
                {
                    return await GetCarsByType("Sedan");
                }
                else if (message.Contains("toyota"))
                {
                    return await GetCarsByBrand("Toyota");
                }
                else if (message.Contains("honda"))
                {
                    return await GetCarsByBrand("Honda");
                }
                else
                {
                    var cars = await _context.Xes
                        .Where(x => x.Trangthai == "Available")
                        .Take(5)
                        .ToListAsync();

                    if (!cars.Any())
                        return "Hiện tại chưa có xe khả dụng.";

                    var response = "**Xe đang có sẵn:**\n\n";
                    foreach (var car in cars)
                    {
                        response += $"? **{car.Tenxe}**\n" +
                                  $"   • Loại: {car.Loaixe} - {car.Sochongoi} chỗ\n" +
                                  $"   • Giá: {car.Giathuetheongay:N0}d/ngày\n" +
                                  $"   • Xem chi tiết: [Nhấn vào đây](/Xe/Details/{car.Id})\n\n";
                    }
                    response += "Bạn muốn tìm xe theo loại nào? (SUV, Sedan, Toyota, Honda...)";
                    return response;
                }
            }

            // 3. GIÁ CẢ
            if (message.Contains("giá") || message.Contains("bao nhiêu") || message.Contains("chi phí"))
            {
                if (message.Contains("rẻ") || message.Contains("thấp"))
                {
                    var cheapCars = await _context.Xes
                        .Where(x => x.Trangthai == "Available")
                        .OrderBy(x => x.Giathuetheongay)
                        .Take(3)
                        .ToListAsync();

                    var response = "**Xe giá tốt nhất:**\n\n";
                    foreach (var car in cheapCars)
                    {
                        response += $" {car.Tenxe} - **{car.Giathuetheongay:N0}d/ngày**\n";
                    }
                    response += "\n Liên hệ: 0981231205 để được tư vấn thêm!";
                    return response;
                }

                return "💰 **Bảng giá thuê xe:**\n\n" +
                       "• Sedan 4-5 chỗ: 500,000đ - 800,000đ/ngày\n" +
                       "• SUV 7 chỗ: 800,000đ - 1,500,000đ/ngày\n" +
                       "• MPV 7 chỗ: 700,000đ - 1,200,000đ/ngày\n\n" +
                       "🎁 Thuê 7 ngày trở lên: Giảm 10%\n" +
                       "🎁 Thuê 30 ngày: Giảm 20%\n\n" +
                       "Bạn muốn xem xe nào cụ thể?";
            }

            // 4. HƯỚNG DẪN THUÊ XE
            if (message.Contains("thuê") || message.Contains("đặt") || message.Contains("cách") || message.Contains("hướng dẫn"))
            {
                return "📋 **Quy trình thuê xe:**\n\n" +
                       "**Bước 1:** Đăng ký tài khoản (nếu chưa có)\n" +
                       "**Bước 2:** Tìm và chọn xe phù hợp\n" +
                       "**Bước 3:** Chọn thời gian thuê\n" +
                       "**Bước 4:** Đặt cọc 30% giá trị xe\n" +
                       "**Bước 5:** Nhận xe và thanh toán phần còn lại\n\n" +
                       "📄 **Giấy tờ cần thiết:**\n" +
                       "• CMND/CCCD (Bản gốc)\n" +
                       "• Bằng lái xe hợp lệ\n" +
                       "• Hộ khẩu hoặc sổ tạm trú\n\n" +
                       "👉 Bạn muốn [đăng ký ngay](/Auth/Register) hay [xem xe](/Xe)?";
            }

            // 5. LIÊN HỆ / HỖ TRỢ
            if (message.Contains("liên hệ") || message.Contains("hotline") || message.Contains("điện thoại") || message.Contains("hỗ trợ"))
            {
                return "📞 **Thông tin liên hệ:**\n\n" +
                       "• **Hotline:** 0981231205 (24/7)\n" +
                       "• **Email:** mainhathoangevil@gmail.com\n" +
                       "• **Địa chỉ:** Hà Nội, Việt Nam\n\n" +
                       "🕒 **Giờ làm việc:**\n" +
                       "• Thứ 2 - Thứ 7: 8:00 - 20:00\n" +
                       "• Chủ nhật: 9:00 - 18:00\n\n" +
                       "Bạn muốn [gửi yêu cầu hỗ trợ](/Account/Support)?";
            }

            // 6. ĐIỀU KIỆN THUÊ
            if (message.Contains("điều kiện") || message.Contains("yêu cầu") || message.Contains("tuổi"))
            {
                return "📝 **Điều kiện thuê xe:**\n\n" +
                       "✓ Từ độ 21 tuổi trở lên\n" +
                       "✓ Có bằng lái xe hợp lệ (B1 trở lên)\n" +
                       "✓ Có CMND/CCCD bản gốc\n" +
                       "✓ Đặt cọc 30% giá trị hợp đồng\n\n" +
                       "✗ **Không cho thuê nếu:**\n" +
                       "• Chưa đủ 21 tuổi\n" +
                       "• Không có giấy tờ hợp lệ\n" +
                       "• Vi phạm giao thông nghiêm trọng\n\n" +
                       "Bạn có câu hỏi nào khác?";
            }

            // 7. BẢO HIỂM
            if (message.Contains("bảo hiểm") || message.Contains("tai nạn"))
            {
                return "🛡️ **Chính sách bảo hiểm:**\n\n" +
                       "✓ **Bảo hiểm xe bao gồm:**\n" +
                       "• Bảo hiểm trách nhiệm dân sự\n" +
                       "• Bảo hiểm vật chất xe\n" +
                       "• Bảo hiểm người ngồi trên xe\n\n" +
                       "⚠️ **Lưu ý:**\n" +
                       "• Khấu hao tự nhiên: Không đền bù\n" +
                       "• Tai nạn do rượu bia: Không bảo hiểm\n" +
                       "• Mất cắp: Báo công an ngay\n\n" +
                       "Cần tư vấn thêm? Gọi: 0981231205";
            }

            // 8. THANH TOÁN
            if (message.Contains("thanh toán") || message.Contains("trả tiền"))
            {
                return "💳 **Phương thức thanh toán:**\n\n" +
                       "✓ Tiền mặt\n" +
                       "✓ Chuyển khoản ngân hàng\n" +
                       "✓ Quét mã QR\n" +
                       "✓ Ví điện tử (Momo, ZaloPay)\n\n" +
                       "📅 **Lịch thanh toán:**\n" +
                       "• Đặt cọc: 30% khi đặt xe\n" +
                       "• Thanh toán: 70% khi nhận xe\n\n" +
                       "🎁 Thanh toán full ngay: Giảm thêm 5%!";
            }

            // 9. H?Y ÐON
            if (message.Contains("h?y") || message.Contains("hoàn ti?n"))
            {
                return "🚫 **Chính sách hủy đơn:**\n\n" +
                       "• Hủy trước 7 ngày: Hoàn 100%\n" +
                       "• Hủy trước 3 ngày: Hoàn 70%\n" +
                       "• Hủy trước 1 ngày: Hoàn 50%\n" +
                       "• Hủy trong ngày: Không hoàn\n\n" +
                       "📌 Chỉ áp dụng cho tiền đặt cọc.\n\n" +
                       "Bạn cần hỗ trợ hủy đơn? [Liên hệ ngay](/Account/Support)";
            }

            // 10. ĐƠN ĐẶT CỦA TÔI
            if (message.Contains("đơn của tôi") || message.Contains("đơn đặt") || message.Contains("lịch sử"))
            {
                if (userId == null)
                {
                    return "🔒 Bạn cần [đăng nhập](/Auth/Login) để xem lịch sử đơn đặt xe.";
                }

                var khachhang = await _context.Khachhangs
                    .FirstOrDefaultAsync(k => k.Nguoidungid == userId);

                if (khachhang == null)
                {
                    return "❌ Không tìm thấy thông tin khách hàng.";
                }

                var bookings = await _context.Datxes
                    .Where(d => d.Khachhangid == khachhang.Id)
                    .OrderByDescending(d => d.Ngaydat)
                    .Take(5)
                    .ToListAsync();

                if (!bookings.Any())
                {
                    return "📝 Bạn chưa có đơn đặt xe nào.\n\n" +
                           "🚗 [Xem danh sách xe](/Xe) để thuê ngay!";
                }

                var response = "📝 **Đơn đặt xe của bạn:**\n\n";
                foreach (var booking in bookings)
                {
                    var statusIcon = booking.Trangthai switch
                    {
                        "Pending" => "⏳",
                        "Confirmed" => "✅",
                        "Completed" => "✅✅",
                        "Cancelled" => "❌",
                        _ => "📝"
                    };
                    response += $"{statusIcon} **Đơn #{booking.Id}** - {booking.Trangthai}\n" +
                              $"   • Ngày đặt: {booking.Ngaydat:dd/MM/yyyy}\n" +
                              $"   • Tổng tiền: {booking.Tongtien:N0}đ\n\n";
                }
                response += "📝 [Xem chi tiết](/Account/BookingHistory)";
                return response;
            }

            // 11. AI KHÔNG HIỂU
            return "🤔 Xin lỗi, tôi chưa hiểu câu hỏi của bạn.\n\n" +
                   "💡 **Bạn có thể hỏi:**\n" +
                   "• Có xe nào đang có sẵn?\n" +
                   "• Giá thuê xe là bao nhiêu?\n" +
                   "• Cách thuê xe như thế nào?\n" +
                   "• Liên hệ hotline\n" +
                   "• Điều kiện thuê xe\n" +
                   "• Đơn đặt của tôi\n\n" +
                   "Hoặc gọi: **0981231205** để được hỗ trợ trực tiếp! 📞";
        }

        // Helper methods
        private bool IsGreeting(string message)
        {
            string[] greetings = { "xin chào", "chào", "hello", "hi", "hey", "chào bạn", "alo" };
            return greetings.Any(g => message.Contains(g));
        }

        private async Task<string> GetCarsByType(string type)
        {
            var cars = await _context.Xes
                .Where(x => x.Loaixe == type && x.Trangthai == "Available")
                .Take(5)
                .ToListAsync();

            if (!cars.Any())
                return $"Hiện tại không có xe {type} nào khả dụng.";

            var response = $"🚗 **Xe {type} có sẵn:**\n\n";
            foreach (var car in cars)
            {
                response += $"🚘 **{car.Tenxe}**\n" +
                          $"   • {car.Sochongoi} chỗ - Màu {car.Mauxe}\n" +
                          $"   • Giá: **{car.Giathuetheongay:N0}đ/ngày**\n" +
                          $"   • [Xem chi tiết](/Xe/Details/{car.Id})\n\n";
            }
            return response;
        }

        private async Task<string> GetCarsByBrand(string brand)
        {
            var cars = await _context.Xes
                .Where(x => x.Hangxe == brand && x.Trangthai == "Available")
                .Take(5)
                .ToListAsync();

            if (!cars.Any())
                return $"Hiện tại không có xe {brand} nào khả dụng.";

            var response = $"🚗 **Xe {brand} có sẵn:**\n\n";
            foreach (var car in cars)
            {
                response += $"🚘 **{car.Tenxe}**\n" +
                          $"   • {car.Loaixe} - {car.Sochongoi} chỗ\n" +
                          $"   • Giá: **{car.Giathuetheongay:N0}đ/ngày**\n" +
                          $"   • [Xem chi tiết](/Xe/Details/{car.Id})\n\n";
            }
            return response;
        }

        // Lấy lịch sử chat
        [HttpGet]
        public async Task<IActionResult> GetHistory(string sessionId)
        {
            var messages = await _context.Chatmessages
                .Where(m => m.Sessionid == sessionId)
                .OrderBy(m => m.Thoigian)
                .Select(m => new
                {
                    m.Id,
                    m.Noidung,
                    m.Loaitinnhan,
                    m.Thoigian
                })
                .ToListAsync();

            return Json(messages);
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = null!;
        public string? SessionId { get; set; }
    }
}
