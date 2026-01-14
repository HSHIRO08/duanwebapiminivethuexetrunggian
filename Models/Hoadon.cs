using System;
using System.Collections.Generic;

namespace duanminiveprogresql.Models;

public partial class Hoadon
{
    public int Id { get; set; }

    public int Datxeid { get; set; }

    public string Mahoadon { get; set; } = null!;

    public decimal Tongtienthue { get; set; }

    public decimal Phidichvu { get; set; }

    public decimal Giamgia { get; set; }

    public decimal Tongthanhtoan { get; set; }

    public DateTime Ngaytao { get; set; }

    public string Trangthai { get; set; } = null!;

    public string? Ghichu { get; set; }

    public virtual Datxe Datxe { get; set; } = null!;
}
