namespace Domain.Entities;

public partial class Datxe
{
    public int Id { get; set; }

    public int Khachhangid { get; set; }

    public int Xeid { get; set; }

    public DateTime Ngaybatdau { get; set; }

    public DateTime Ngayketthuc { get; set; }

    public int Songaythue { get; set; }

    public decimal Giatheongay { get; set; }

    public decimal Tongtien { get; set; }

    public string? Diadiemnhan { get; set; }

    public string? Diadiemtra { get; set; }

    public string? Ghichu { get; set; }

    public string Trangthai { get; set; } = null!;

    public DateTime Ngaydat { get; set; }

    public DateTime? Ngayxacnhan { get; set; }

    public DateTime? Ngayhoanthanh { get; set; }

    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();

    public virtual Khachhang Khachhang { get; set; } = null!;

    public virtual ICollection<Lichsuthue> Lichsuthues { get; set; } = new List<Lichsuthue>();

    public virtual ICollection<Thanhtoan> Thanhtoans { get; set; } = new List<Thanhtoan>();

    public virtual Xe Xe { get; set; } = null!;
}
