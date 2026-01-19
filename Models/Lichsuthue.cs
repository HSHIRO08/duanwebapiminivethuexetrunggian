namespace duanminiveprogresql.Models;

public partial class Lichsuthue
{
    public int Id { get; set; }

    public int Xeid { get; set; }

    public int Khachhangid { get; set; }

    public int Datxeid { get; set; }

    public DateTime Ngaynhanxe { get; set; }

    public DateTime? Ngaytraxe { get; set; }

    public int Kmbatdau { get; set; }

    public int? Kmketthuc { get; set; }

    public decimal? Phiphatsinh { get; set; }

    public string? Ghichunhanxe { get; set; }

    public string? Ghichutraxe { get; set; }

    public string? Trangthaixe { get; set; }

    public int? Danhgia { get; set; }

    public string? Nhanxet { get; set; }

    public virtual Datxe Datxe { get; set; } = null!;

    public virtual Khachhang Khachhang { get; set; } = null!;

    public virtual Xe Xe { get; set; } = null!;
}
