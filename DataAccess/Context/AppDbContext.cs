using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace DataAccess.Context
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Chatmessage> Chatmessages { get; set; }
        public virtual DbSet<Datxe> Datxes { get; set; }
        public virtual DbSet<Hoadon> Hoadons { get; set; }
        public virtual DbSet<Hotrokhachhang> Hotrokhachhangs { get; set; }
        public virtual DbSet<Khachhang> Khachhangs { get; set; }
        public virtual DbSet<Lichsuthue> Lichsuthues { get; set; }
        public virtual DbSet<Nguoidung> Nguoidungs { get; set; }
        public virtual DbSet<Thanhtoan> Thanhtoans { get; set; }
        public virtual DbSet<Xe> Xes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chatmessage>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("chatmessage_pkey");

                entity.ToTable("chatmessage");

                entity.Property(e => e.Id)
                    .UseIdentityAlwaysColumn()
                    .HasColumnName("id");
                entity.Property(e => e.Dadoc)
                    .HasDefaultValue(false)
                    .HasColumnName("dadoc");
                entity.Property(e => e.Loaitinnhan)
                    .HasMaxLength(20)
                    .HasColumnName("loaitinnhan");
                entity.Property(e => e.Nguoidungid).HasColumnName("nguoidungid");
                entity.Property(e => e.Noidung).HasColumnName("noidung");
                entity.Property(e => e.Sessionid)
                    .HasMaxLength(50)
                    .HasColumnName("sessionid");
                entity.Property(e => e.Thoigian)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("thoigian");

                entity.HasOne(d => d.Nguoidung).WithMany(p => p.Chatmessages)
                    .HasForeignKey(d => d.Nguoidungid)
                    .HasConstraintName("fk_chatmessage_nguoidung");
            });

            modelBuilder.Entity<Datxe>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("datxe_pkey");

                entity.ToTable("datxe");

                entity.HasIndex(e => e.Khachhangid, "ix_datxe_khachhangid");
                entity.HasIndex(e => e.Trangthai, "ix_datxe_trangthai");
                entity.HasIndex(e => e.Xeid, "ix_datxe_xeid");

                entity.Property(e => e.Id)
                    .UseIdentityAlwaysColumn()
                    .HasColumnName("id");
                entity.Property(e => e.Diadiemnhan)
                    .HasMaxLength(500)
                    .HasColumnName("diadiemnhan");
                entity.Property(e => e.Diadiemtra)
                    .HasMaxLength(500)
                    .HasColumnName("diadiemtra");
                entity.Property(e => e.Ghichu).HasColumnName("ghichu");
                entity.Property(e => e.Giatheongay)
                    .HasPrecision(18, 2)
                    .HasColumnName("giatheongay");
                entity.Property(e => e.Khachhangid).HasColumnName("khachhangid");
                entity.Property(e => e.Ngaybatdau)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("ngaybatdau");
                entity.Property(e => e.Ngaydat)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("ngaydat");
                entity.Property(e => e.Ngayhoanthanh)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("ngayhoanthanh");
                entity.Property(e => e.Ngayketthuc)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("ngayketthuc");
                entity.Property(e => e.Ngayxacnhan)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("ngayxacnhan");
                entity.Property(e => e.Songaythue).HasColumnName("songaythue");
                entity.Property(e => e.Tongtien)
                    .HasPrecision(18, 2)
                    .HasColumnName("tongtien");
                entity.Property(e => e.Trangthai)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'Pending'::character varying")
                    .HasColumnName("trangthai");
                entity.Property(e => e.Xeid).HasColumnName("xeid");

                entity.HasOne(d => d.Khachhang).WithMany(p => p.Datxes)
                    .HasForeignKey(d => d.Khachhangid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_datxe_khachhang");

                entity.HasOne(d => d.Xe).WithMany(p => p.Datxes)
                    .HasForeignKey(d => d.Xeid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_datxe_xe");
            });

            modelBuilder.Entity<Hoadon>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("hoadon_pkey");

                entity.ToTable("hoadon");

                entity.HasIndex(e => e.Mahoadon, "hoadon_mahoadon_key").IsUnique();

                entity.Property(e => e.Id)
                    .UseIdentityAlwaysColumn()
                    .HasColumnName("id");
                entity.Property(e => e.Datxeid).HasColumnName("datxeid");
                entity.Property(e => e.Ghichu).HasColumnName("ghichu");
                entity.Property(e => e.Giamgia)
                    .HasPrecision(18, 2)
                    .HasColumnName("giamgia");
                entity.Property(e => e.Mahoadon)
                    .HasMaxLength(50)
                    .HasColumnName("mahoadon");
                entity.Property(e => e.Ngaytao)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("ngaytao");
                entity.Property(e => e.Phidichvu)
                    .HasPrecision(18, 2)
                    .HasColumnName("phidichvu");
                entity.Property(e => e.Tongthanhtoan)
                    .HasPrecision(18, 2)
                    .HasColumnName("tongthanhtoan");
                entity.Property(e => e.Tongtienthue)
                    .HasPrecision(18, 2)
                    .HasColumnName("tongtienthue");
                entity.Property(e => e.Trangthai)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'Draft'::character varying")
                    .HasColumnName("trangthai");

                entity.HasOne(d => d.Datxe).WithMany(p => p.Hoadons)
                    .HasForeignKey(d => d.Datxeid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_hoadon_datxe");
            });

            modelBuilder.Entity<Hotrokhachhang>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("hotrokhachhang_pkey");

                entity.ToTable("hotrokhachhang");

                entity.Property(e => e.Id)
                    .UseIdentityAlwaysColumn()
                    .HasColumnName("id");
                entity.Property(e => e.Khachhangid).HasColumnName("khachhangid");
                entity.Property(e => e.Loaiyeucau)
                    .HasMaxLength(50)
                    .HasColumnName("loaiyeucau");
                entity.Property(e => e.Mucdouutien)
                    .HasMaxLength(50)
                    .HasColumnName("mucdouutien");
                entity.Property(e => e.Ngaycapnhat)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("ngaycapnhat");
                entity.Property(e => e.Ngaygiaiquyet)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("ngaygiaiquyet");
                entity.Property(e => e.Ngaytao)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("ngaytao");
                entity.Property(e => e.Nhanvienxulyid).HasColumnName("nhanvienxulyid");
                entity.Property(e => e.Noidung).HasColumnName("noidung");
                entity.Property(e => e.Tieude)
                    .HasMaxLength(200)
                    .HasColumnName("tieude");
                entity.Property(e => e.Traloi).HasColumnName("traloi");
                entity.Property(e => e.Trangthai)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'Open'::character varying")
                    .HasColumnName("trangthai");

                entity.HasOne(d => d.Khachhang).WithMany(p => p.Hotrokhachhangs)
                    .HasForeignKey(d => d.Khachhangid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_hotrokhachhang_khachhang");

                entity.HasOne(d => d.Nhanvienxuly).WithMany(p => p.Hotrokhachhangs)
                    .HasForeignKey(d => d.Nhanvienxulyid)
                    .HasConstraintName("fk_hotrokhachhang_nhanvien");
            });

            modelBuilder.Entity<Khachhang>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("khachhang_pkey");

                entity.ToTable("khachhang");

                entity.Property(e => e.Id)
                    .UseIdentityAlwaysColumn()
                    .HasColumnName("id");
                entity.Property(e => e.Banglai)
                    .HasMaxLength(50)
                    .HasColumnName("banglai");
                entity.Property(e => e.Cmnd)
                    .HasMaxLength(50)
                    .HasColumnName("cmnd");
                entity.Property(e => e.Daxacthuc)
                    .HasDefaultValue(false)
                    .HasColumnName("daxacthuc");
                entity.Property(e => e.Diachichitiet)
                    .HasMaxLength(500)
                    .HasColumnName("diachichitiet");
                entity.Property(e => e.Gioitinh)
                    .HasMaxLength(10)
                    .HasColumnName("gioitinh");
                entity.Property(e => e.Ngaydangky)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("ngaydangky");
                entity.Property(e => e.Ngaysinh).HasColumnName("ngaysinh");
                entity.Property(e => e.Nguoidungid).HasColumnName("nguoidungid");

                entity.HasOne(d => d.Nguoidung).WithMany(p => p.Khachhangs)
                    .HasForeignKey(d => d.Nguoidungid)
                    .HasConstraintName("fk_khachhang_nguoidung");
            });

            modelBuilder.Entity<Lichsuthue>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("lichsuthue_pkey");

                entity.ToTable("lichsuthue");

                entity.Property(e => e.Id)
                    .UseIdentityAlwaysColumn()
                    .HasColumnName("id");
                entity.Property(e => e.Danhgia).HasColumnName("danhgia");
                entity.Property(e => e.Datxeid).HasColumnName("datxeid");
                entity.Property(e => e.Ghichunhanxe).HasColumnName("ghichunhanxe");
                entity.Property(e => e.Ghichutraxe).HasColumnName("ghichutraxe");
                entity.Property(e => e.Khachhangid).HasColumnName("khachhangid");
                entity.Property(e => e.Kmbatdau).HasColumnName("kmbatdau");
                entity.Property(e => e.Kmketthuc).HasColumnName("kmketthuc");
                entity.Property(e => e.Ngaynhanxe)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("ngaynhanxe");
                entity.Property(e => e.Ngaytraxe)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("ngaytraxe");
                entity.Property(e => e.Nhanxet).HasColumnName("nhanxet");
                entity.Property(e => e.Phiphatsinh)
                    .HasPrecision(18, 2)
                    .HasColumnName("phiphatsinh");
                entity.Property(e => e.Trangthaixe)
                    .HasMaxLength(50)
                    .HasColumnName("trangthaixe");
                entity.Property(e => e.Xeid).HasColumnName("xeid");

                entity.HasOne(d => d.Datxe).WithMany(p => p.Lichsuthues)
                    .HasForeignKey(d => d.Datxeid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_lichsuthue_datxe");

                entity.HasOne(d => d.Khachhang).WithMany(p => p.Lichsuthues)
                    .HasForeignKey(d => d.Khachhangid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_lichsuthue_khachhang");

                entity.HasOne(d => d.Xe).WithMany(p => p.Lichsuthues)
                    .HasForeignKey(d => d.Xeid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_lichsuthue_xe");
            });

            modelBuilder.Entity<Nguoidung>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("nguoidung_pkey");

                entity.ToTable("nguoidung");

                entity.HasIndex(e => e.Email, "ix_nguoidung_email");
                entity.HasIndex(e => e.Vaitro, "ix_nguoidung_vaitro");
                entity.HasIndex(e => e.Email, "nguoidung_email_key").IsUnique();

                entity.Property(e => e.Id)
                    .UseIdentityAlwaysColumn()
                    .HasColumnName("id");
                entity.Property(e => e.Diachi)
                    .HasMaxLength(500)
                    .HasColumnName("diachi");
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");
                entity.Property(e => e.Hoten)
                    .HasMaxLength(100)
                    .HasColumnName("hoten");
                entity.Property(e => e.Matkhau)
                    .HasMaxLength(100)
                    .HasColumnName("matkhau");
                entity.Property(e => e.Ngaytao)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("ngaytao");
                entity.Property(e => e.Sodienthoai)
                    .HasMaxLength(20)
                    .HasColumnName("sodienthoai");
                entity.Property(e => e.Trangthai)
                    .HasDefaultValue(true)
                    .HasColumnName("trangthai");
                entity.Property(e => e.Vaitro)
                    .HasMaxLength(20)
                    .HasColumnName("vaitro");
            });

            modelBuilder.Entity<Thanhtoan>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("thanhtoan_pkey");

                entity.ToTable("thanhtoan");

                entity.HasIndex(e => e.Magiaodich, "thanhtoan_magiaodich_key").IsUnique();

                entity.Property(e => e.Id)
                    .UseIdentityAlwaysColumn()
                    .HasColumnName("id");
                entity.Property(e => e.Datxeid).HasColumnName("datxeid");
                entity.Property(e => e.Ghichu).HasColumnName("ghichu");
                entity.Property(e => e.Magiaodich)
                    .HasMaxLength(50)
                    .HasColumnName("magiaodich");
                entity.Property(e => e.Ngaythanhtoan)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("ngaythanhtoan");
                entity.Property(e => e.Ngayxacnhan)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("ngayxacnhan");
                entity.Property(e => e.Phuongthucthanhtoan)
                    .HasMaxLength(50)
                    .HasColumnName("phuongthucthanhtoan");
                entity.Property(e => e.Sotien)
                    .HasPrecision(18, 2)
                    .HasColumnName("sotien");
                entity.Property(e => e.Trangthai)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'Pending'::character varying")
                    .HasColumnName("trangthai");

                entity.HasOne(d => d.Datxe).WithMany(p => p.Thanhtoans)
                    .HasForeignKey(d => d.Datxeid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_thanhtoan_datxe");
            });

            modelBuilder.Entity<Xe>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("xe_pkey");

                entity.ToTable("xe");

                entity.HasIndex(e => e.Loaixe, "ix_xe_loaixe");
                entity.HasIndex(e => e.Nhacungcapid, "ix_xe_nhacungcapid");
                entity.HasIndex(e => e.Trangthai, "ix_xe_trangthai");

                entity.Property(e => e.Id)
                    .UseIdentityAlwaysColumn()
                    .HasColumnName("id");
                entity.Property(e => e.Biensoxe)
                    .HasMaxLength(50)
                    .HasColumnName("biensoxe");
                entity.Property(e => e.Giathuetheongay)
                    .HasPrecision(18, 2)
                    .HasColumnName("giathuetheongay");
                entity.Property(e => e.Hangxe)
                    .HasMaxLength(50)
                    .HasColumnName("hangxe");
                entity.Property(e => e.Hinhanh)
                    .HasMaxLength(500)
                    .HasColumnName("hinhanh");
                entity.Property(e => e.Loaixe)
                    .HasMaxLength(50)
                    .HasColumnName("loaixe");
                entity.Property(e => e.Mauxe)
                    .HasMaxLength(50)
                    .HasColumnName("mauxe");
                entity.Property(e => e.Mota).HasColumnName("mota");
                entity.Property(e => e.Namsanxuat).HasColumnName("namsanxuat");
                entity.Property(e => e.Ngaycapnhat)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("ngaycapnhat");
                entity.Property(e => e.Ngaytao)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("ngaytao");
                entity.Property(e => e.Nhacungcapid).HasColumnName("nhacungcapid");
                entity.Property(e => e.Sochongoi).HasColumnName("sochongoi");
                entity.Property(e => e.Tenxe)
                    .HasMaxLength(100)
                    .HasColumnName("tenxe");
                entity.Property(e => e.Trangthai)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'Available'::character varying")
                    .HasColumnName("trangthai");

                entity.HasOne(d => d.Nhacungcap).WithMany(p => p.Xes)
                    .HasForeignKey(d => d.Nhacungcapid)
                    .HasConstraintName("fk_xe_nhacungcap");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
