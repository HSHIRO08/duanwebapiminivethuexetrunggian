CREATE TABLE NguoiDung (
    Id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    HoTen VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    MatKhau VARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(20),
    DiaChi VARCHAR(500),
    VaiTro VARCHAR(20) NOT NULL,
    NgayTao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    TrangThai BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE INDEX IX_NguoiDung_Email ON NguoiDung(Email);
CREATE INDEX IX_NguoiDung_VaiTro ON NguoiDung(VaiTro);


CREATE TABLE KhachHang (
    Id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    NguoiDungId INT NOT NULL,
    CMND VARCHAR(50),
    BangLai VARCHAR(50),
    NgaySinh DATE,
    GioiTinh VARCHAR(10),
    DiaChiChiTiet VARCHAR(500),
    DaXacThuc BOOLEAN NOT NULL DEFAULT FALSE,
    NgayDangKy TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_KhachHang_NguoiDung FOREIGN KEY (NguoiDungId) 
        REFERENCES NguoiDung(Id) ON DELETE CASCADE
);

CREATE INDEX IX_KhachHang_NguoiDungId ON KhachHang(NguoiDungId);


CREATE TABLE Xe (
    Id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    NhaCungCapId INT NOT NULL,
    TenXe VARCHAR(100) NOT NULL,
    BienSoXe VARCHAR(50),
    HangXe VARCHAR(50),
    MauXe VARCHAR(50),
    NamSanXuat INT NOT NULL,
    SoChoNgoi INT NOT NULL,
    LoaiXe VARCHAR(50),
    GiaThueTheoNgay DECIMAL(18,2) NOT NULL,
    MoTa TEXT,
    HinhAnh VARCHAR(500),
    TrangThai VARCHAR(50) NOT NULL DEFAULT 'Available', 
    NgayTao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    NgayCapNhat TIMESTAMP,
    CONSTRAINT FK_Xe_NhaCungCap FOREIGN KEY (NhaCungCapId) 
        REFERENCES NguoiDung(Id) ON DELETE CASCADE
);

CREATE INDEX IX_Xe_NhaCungCapId ON Xe(NhaCungCapId);
CREATE INDEX IX_Xe_TrangThai ON Xe(TrangThai);
CREATE INDEX IX_Xe_LoaiXe ON Xe(LoaiXe);


CREATE TABLE DatXe (
    Id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    KhachHangId INT NOT NULL,
    XeId INT NOT NULL,
    NgayBatDau TIMESTAMP NOT NULL,
    NgayKetThuc TIMESTAMP NOT NULL,
    SoNgayThue INT NOT NULL,
    GiaTheoNgay DECIMAL(18,2) NOT NULL,
    TongTien DECIMAL(18,2) NOT NULL,
    DiaDiemNhan VARCHAR(500),
    DiaDiemTra VARCHAR(500),
    GhiChu TEXT,
    TrangThai VARCHAR(50) NOT NULL DEFAULT 'Pending', 
    NgayDat TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    NgayXacNhan TIMESTAMP,
    NgayHoanThanh TIMESTAMP,
    CONSTRAINT FK_DatXe_KhachHang FOREIGN KEY (KhachHangId) 
        REFERENCES KhachHang(Id),
    CONSTRAINT FK_DatXe_Xe FOREIGN KEY (XeId) 
        REFERENCES Xe(Id)
);

CREATE INDEX IX_DatXe_KhachHangId ON DatXe(KhachHangId);
CREATE INDEX IX_DatXe_XeId ON DatXe(XeId);
CREATE INDEX IX_DatXe_TrangThai ON DatXe(TrangThai);


CREATE TABLE ThanhToan (
    Id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    DatXeId INT NOT NULL,
    MaGiaoDich VARCHAR(50) NOT NULL UNIQUE,
    SoTien DECIMAL(18,2) NOT NULL,
    PhuongThucThanhToan VARCHAR(50) NOT NULL, -- Online, Cash, BankTransfer
    TrangThai VARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, Completed, Failed, Refunded
    NgayThanhToan TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    NgayXacNhan TIMESTAMP,
    GhiChu TEXT,
    CONSTRAINT FK_ThanhToan_DatXe FOREIGN KEY (DatXeId) 
        REFERENCES DatXe(Id)
);


CREATE TABLE HoaDon (
    Id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    DatXeId INT NOT NULL,
    MaHoaDon VARCHAR(50) NOT NULL UNIQUE,
    TongTienThue DECIMAL(18,2) NOT NULL,
    PhiDichVu DECIMAL(18,2) NOT NULL DEFAULT 0,
    GiamGia DECIMAL(18,2) NOT NULL DEFAULT 0,
    TongThanhToan DECIMAL(18,2) NOT NULL,
    NgayTao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    TrangThai VARCHAR(50) NOT NULL DEFAULT 'Draft', -- Draft, Issued, Paid, Cancelled
    GhiChu TEXT,
    CONSTRAINT FK_HoaDon_DatXe FOREIGN KEY (DatXeId) 
        REFERENCES DatXe(Id)
);

CREATE TABLE LichSuThue (
    Id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    XeId INT NOT NULL,
    KhachHangId INT NOT NULL,
    DatXeId INT NOT NULL,
    NgayNhanXe TIMESTAMP NOT NULL,
    NgayTraXe TIMESTAMP,
    KmBatDau INT NOT NULL,
    KmKetThuc INT,
    PhiPhatSinh DECIMAL(18,2),
    GhiChuNhanXe TEXT,
    GhiChuTraXe TEXT,
    TrangThaiXe VARCHAR(50), 
    DanhGia INT CHECK (DanhGia >= 1 AND DanhGia <= 5),
    NhanXet TEXT,
    CONSTRAINT FK_LichSuThue_Xe FOREIGN KEY (XeId) 
        REFERENCES Xe(Id),
    CONSTRAINT FK_LichSuThue_KhachHang FOREIGN KEY (KhachHangId) 
        REFERENCES KhachHang(Id),
    CONSTRAINT FK_LichSuThue_DatXe FOREIGN KEY (DatXeId) 
        REFERENCES DatXe(Id)
);

CREATE TABLE ChatMessage (
    Id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    NguoiDungId INT,
    SessionId VARCHAR(50) NOT NULL,
    NoiDung TEXT NOT NULL,
    LoaiTinNhan VARCHAR(20) NOT NULL, -- User, Bot
    ThoiGian TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DaDoc BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT FK_ChatMessage_NguoiDung FOREIGN KEY (NguoiDungId) 
        REFERENCES NguoiDung(Id)
);


CREATE TABLE HoTroKhachHang (
    Id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    KhachHangId INT NOT NULL,
    TieuDe VARCHAR(200) NOT NULL,
    NoiDung TEXT NOT NULL,
    LoaiYeuCau VARCHAR(50) NOT NULL, 
    TrangThai VARCHAR(50) NOT NULL DEFAULT 'Open',
    MucDoUuTien VARCHAR(50), 
    NgayTao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    NgayCapNhat TIMESTAMP,
    NgayGiaiQuyet TIMESTAMP,
    TraLoi TEXT,
    NhanVienXuLyId INT,
    CONSTRAINT FK_HoTroKhachHang_KhachHang FOREIGN KEY (KhachHangId) 
        REFERENCES KhachHang(Id),
    CONSTRAINT FK_HoTroKhachHang_NhanVien FOREIGN KEY (NhanVienXuLyId) 
        REFERENCES NguoiDung(Id)
);



-- Dùng khối DO để xử lý biến tạm trong PostgreSQL
DO $$
DECLARE 
    supplier_id INT;
    customer_user_id INT;
    customer_id INT;
BEGIN
    -- Insert Admin
    INSERT INTO NguoiDung (HoTen, Email, MatKhau, SoDienThoai, DiaChi, VaiTro)
    VALUES ('Administrator', 'admin@chothuexeoto.com', 'Admin@123', '0123456789', 'Hà Nội', 'Admin');

    -- Insert Supplier và lấy Id
    INSERT INTO NguoiDung (HoTen, Email, MatKhau, SoDienThoai, DiaChi, VaiTro)
    VALUES ('Nhà Cung Cấp Demo', 'nhacungcap@example.com', 'Ncc@123', '0987654321', 'TP. Hồ Chí Minh', 'NhaCungCap')
    RETURNING Id INTO supplier_id;

    -- Insert Cars
    INSERT INTO Xe (NhaCungCapId, TenXe, BienSoXe, HangXe, MauXe, NamSanXuat, SoChoNgoi, LoaiXe, GiaThueTheoNgay, MoTa, HinhAnh, TrangThai)
    VALUES 
    (supplier_id, 'Toyota Vios 2023', '30A-12345', 'Toyota', 'Trắng', 2023, 5, 'Sedan', 500000, 'Xe sedan 5 chỗ, tiết kiệm nhiên liệu', '/images/xe/toyota-vios.jpg', 'Available'),
    (supplier_id, 'Honda CR-V 2023', '30B-67890', 'Honda', 'Đen', 2023, 7, 'SUV', 900000, 'SUV 7 chỗ cao cấp, rộng rãi', '/images/xe/honda-crv.jpg', 'Available');

    -- Insert Customer User và lấy Id
    INSERT INTO NguoiDung (HoTen, Email, MatKhau, SoDienThoai, DiaChi, VaiTro)
    VALUES ('Nguyễn Văn A', 'khachhang@example.com', 'Kh@123', '0912345678', 'Hà Nội', 'KhachHang')
    RETURNING Id INTO customer_user_id;

    -- Insert Customer Details
    INSERT INTO KhachHang (NguoiDungId, CMND, BangLai, NgaySinh, GioiTinh, DiaChiChiTiet, DaXacThuc)
    VALUES (customer_user_id, '001234567890', 'B2-123456', '1990-01-01', 'Nam', '123 Đường ABC, Quận 1, Hà Nội', TRUE);

END $$;

select * from nguoidung


-- Admin@123 -> e86f78a8a3caf0b60d8e74e5942aa6d86dc150cd3c03338aef25b7d2d7e3acc7
UPDATE nguoidung 
SET matkhau = 'e86f78a8a3caf0b60d8e74e5942aa6d86dc150cd3c03338aef25b7d2d7e3acc7'
WHERE email = 'admin@chothuexeoto.com' 


-- Ncc@123 -> 5e3a8e5c8f9e6b7d4a2c1f8e9b6c3d7a4e5f2b1c8d9e6a7f3b4c5d2e1a8f9b6c
UPDATE nguoidung 
SET matkhau = '5e3a8e5c8f9e6b7d4a2c1f8e9b6c3d7a4e5f2b1c8d9e6a7f3b4c5d2e1a8f9b6c'
WHERE email = 'nhacungcap@example.com' 


-- Kh@123 -> 8b4c7d1e9f2a6b5c3d8e4f7a1b9c6d2e5f3a8b7c1d4e9f6a2b5c8d3e7f1a4b9c
UPDATE nguoidung 
SET matkhau = '8b4c7d1e9f2a6b5c3d8e4f7a1b9c6d2e5f3a8b7c1d4e9f6a2b5c8d3e7f1a4b9c'
WHERE email = 'khachhang@example.com' 

