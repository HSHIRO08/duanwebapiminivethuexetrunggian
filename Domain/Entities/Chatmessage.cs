namespace Domain.Entities;

public partial class Chatmessage
{
    public int Id { get; set; }

    public int? Nguoidungid { get; set; }

    public string Sessionid { get; set; } = null!;

    public string Noidung { get; set; } = null!;

    public string Loaitinnhan { get; set; } = null!;

    public DateTime Thoigian { get; set; }

    public bool Dadoc { get; set; }

    public virtual Nguoidung? Nguoidung { get; set; }
}
