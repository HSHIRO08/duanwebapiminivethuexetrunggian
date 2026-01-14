using System;
using System.Collections.Generic;

namespace duanminiveprogresql.Models;

public partial class Thanhtoan
{
    public int Id { get; set; }

    public int Datxeid { get; set; }

    public string Magiaodich { get; set; } = null!;

    public decimal Sotien { get; set; }

    public string Phuongthucthanhtoan { get; set; } = null!;

    public string Trangthai { get; set; } = null!;

    public DateTime Ngaythanhtoan { get; set; }

    public DateTime? Ngayxacnhan { get; set; }

    public string? Ghichu { get; set; }

    public virtual Datxe Datxe { get; set; } = null!;
}
