namespace Domain.Entities;

public partial class Hotrokhachhang
{
    public int Id { get; set; }

    public int Khachhangid { get; set; }

    public string Tieude { get; set; } = null!;

    public string Noidung { get; set; } = null!;

    public string Loaiyeucau { get; set; } = null!;

    public string Trangthai { get; set; } = null!;

    public string? Mucdouutien { get; set; }

    public DateTime Ngaytao { get; set; }

    public DateTime? Ngaycapnhat { get; set; }

    public DateTime? Ngaygiaiquyet { get; set; }

    public string? Traloi { get; set; }

    public int? Nhanvienxulyid { get; set; }

    public virtual Khachhang Khachhang { get; set; } = null!;

    public virtual Nguoidung? Nhanvienxuly { get; set; }
}
