namespace Domain.Entities;

public partial class Khachhang
{
    public int Id { get; set; }

    public int Nguoidungid { get; set; }

    public string? Cmnd { get; set; }

    public string? Banglai { get; set; }

    public DateOnly? Ngaysinh { get; set; }

    public string? Gioitinh { get; set; }

    public string? Diachichitiet { get; set; }

    public bool Daxacthuc { get; set; }

    public DateTime Ngaydangky { get; set; }

    public virtual ICollection<Datxe> Datxes { get; set; } = new List<Datxe>();

    public virtual ICollection<Hotrokhachhang> Hotrokhachhangs { get; set; } = new List<Hotrokhachhang>();

    public virtual ICollection<Lichsuthue> Lichsuthues { get; set; } = new List<Lichsuthue>();

    public virtual Nguoidung Nguoidung { get; set; } = null!;
}
