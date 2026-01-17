namespace Domain.Entities;

public partial class Xe
{
    public int Id { get; set; }

    public int Nhacungcapid { get; set; }

    public string Tenxe { get; set; } = null!;

    public string? Biensoxe { get; set; }

    public string? Hangxe { get; set; }

    public string? Mauxe { get; set; }

    public int Namsanxuat { get; set; }

    public int Sochongoi { get; set; }

    public string? Loaixe { get; set; }

    public decimal Giathuetheongay { get; set; }

    public string? Mota { get; set; }

    public string? Hinhanh { get; set; }

    public string Trangthai { get; set; } = null!;

    public DateTime Ngaytao { get; set; }

    public DateTime? Ngaycapnhat { get; set; }

    public virtual ICollection<Datxe> Datxes { get; set; } = new List<Datxe>();

    public virtual ICollection<Lichsuthue> Lichsuthues { get; set; } = new List<Lichsuthue>();

    public virtual Nguoidung Nhacungcap { get; set; } = null!;
}
