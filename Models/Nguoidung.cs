namespace duanminiveprogresql.Models;

public partial class Nguoidung
{
    public int Id { get; set; }

    public string Hoten { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Matkhau { get; set; } = null!;

    public string? Sodienthoai { get; set; }

    public string? Diachi { get; set; }

    public string Vaitro { get; set; } = null!;

    public DateTime Ngaytao { get; set; }

    public bool Trangthai { get; set; }

    public virtual ICollection<Chatmessage> Chatmessages { get; set; } = new List<Chatmessage>();

    public virtual ICollection<Hotrokhachhang> Hotrokhachhangs { get; set; } = new List<Hotrokhachhang>();

    public virtual ICollection<Khachhang> Khachhangs { get; set; } = new List<Khachhang>();

    public virtual ICollection<Xe> Xes { get; set; } = new List<Xe>();
}
