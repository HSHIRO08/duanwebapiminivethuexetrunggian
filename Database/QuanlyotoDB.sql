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

select * from Xe

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

    -- Insert Customer User và lấy Id
    INSERT INTO NguoiDung (HoTen, Email, MatKhau, SoDienThoai, DiaChi, VaiTro)
    VALUES ('Nguyễn Văn A', 'khachhang@example.com', 'Kh@123', '0912345678', 'Hà Nội', 'KhachHang')
    RETURNING Id INTO customer_user_id;

    -- Insert Customer Details
    INSERT INTO KhachHang (NguoiDungId, CMND, BangLai, NgaySinh, GioiTinh, DiaChiChiTiet, DaXacThuc)
    VALUES (customer_user_id, '001234567890', 'B2-123456', '1990-01-01', 'Nam', '123 Đường ABC, Quận 1, Hà Nội', TRUE);

END $$;

select * from nguoidung


select * from xe


DO $$
DECLARE 
    supplier_id INT;
    customer_user_id INT;
    customer_id INT;
BEGIN
INSERT INTO Xe (
    NhaCungCapId,
    TenXe,
    BienSoXe,
    HangXe,
    MauXe,
    NamSanXuat,
    SoChoNgoi,
    LoaiXe,
    GiaThueTheoNgay,
    MoTa,
    HinhAnh,
    TrangThai
) VALUES

(2, 'Toyota Vios 2022', '51A-12345', 'Toyota', 'Trắng', 2022, 5, 'Sedan', 700000,
 'Xe tiết kiệm nhiên liệu, phù hợp đi thành phố',
 'https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb?w=800&h=600&fit=crop', 'Available'),

(2, 'Honda City 2021', '51A-23456', 'Honda', 'Đen', 2021, 5, 'Sedan', 750000,
 'Xe bền bỉ, nội thất rộng rãi',
 'https://images.unsplash.com/photo-1583121274602-3e2820c69888?w=800&h=600&fit=crop', 'Available'),

(2, 'Mazda CX-5 2023', '51A-34567', 'Mazda', 'Đỏ', 2023, 5, 'SUV', 1200000,
 'SUV cao cấp, nhiều công nghệ an toàn',
 'https://images.unsplash.com/photo-1617654112368-307921291f42?w=800&h=600&fit=crop', 'Available'),

(2, 'Ford Everest 2022', '51A-45678', 'Ford', 'Xám', 2022, 7, 'SUV', 1500000,
 'Xe 7 chỗ mạnh mẽ, phù hợp đi gia đình',
 'https://images.unsplash.com/photo-1519641471654-76ce0107ad1b?w=800&h=600&fit=crop', 'Available'),

(2, 'Kia Morning 2020', '51A-56789', 'Kia', 'Xanh', 2020, 4, 'Hatchback', 500000,
 'Xe nhỏ gọn, dễ di chuyển trong đô thị',
 'https://images.unsplash.com/photo-1552519507-da3b142c6e3d?w=800&h=600&fit=crop', 'Available'),

(2, 'Hyundai Accent 2021', '51A-67890', 'Hyundai', 'Bạc', 2021, 5, 'Sedan', 650000,
 'Xe phổ thông, giá thuê hợp lý',
 'https://images.unsplash.com/photo-1605559424843-9e4c228bf1c2?w=800&h=600&fit=crop', 'Available'),

(2, 'Toyota Fortuner 2023', '51A-78901', 'Toyota', 'Đen', 2023, 7, 'SUV', 1600000,
 'SUV 7 chỗ cao cấp, vận hành ổn định',
 'https://images.unsplash.com/photo-1619405399517-d7fce0f13302?w=800&h=600&fit=crop', 'Available'),

(2, 'VinFast Lux A2.0 2022', '51A-89012', 'VinFast', 'Trắng', 2022, 5, 'Sedan', 1300000,
 'Sedan cao cấp thương hiệu Việt',
 'https://images.unsplash.com/photo-1614200187524-dc4b892acf16?w=800&h=600&fit=crop', 'Available'),

(2, 'Mitsubishi Xpander 2021', '51A-90123', 'Mitsubishi', 'Cam', 2021, 7, 'MPV', 900000,
 'Xe gia đình 7 chỗ, tiết kiệm nhiên liệu',
 'https://images.unsplash.com/photo-1609521263047-f8f205293f24?w=800&h=600&fit=crop', 'Available'),

(2, 'Toyota Innova 2020', '51A-01234', 'Toyota', 'Bạc', 2020, 7, 'MPV', 850000,
 'Xe rộng rãi, phù hợp đi nhóm',
 'https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?w=800&h=600&fit=crop', 'Available');

END $$;

UPDATE Xe 
SET GiaThueTheoNgay = 550000,
    MoTa = 'Xe sedan 5 chỗ tiết kiệm nhiên liệu, có camera lùi, cảm biến lùi, màn hình cảm ứng',
    HinhAnh = 'https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb?w=800&h=600&fit=crop',
    NgayCapNhat = CURRENT_TIMESTAMP
WHERE BienSoXe = '30A-12345';

-- Cập nhật Honda CR-V 2023 (BienSoXe: 30B-67890)
UPDATE Xe 
SET GiaThueTheoNgay = 950000,
    MoTa = 'SUV 7 chỗ cao cấp, rộng rãi, có hệ thống an toàn Honda Sensing, camera 360 độ',
    HinhAnh = 'https://images.unsplash.com/photo-1606664515524-ed2f786a0bd6?w=800&h=600&fit=crop',
    NgayCapNhat = CURRENT_TIMESTAMP
WHERE BienSoXe = '30B-67890';

-- Cập nhật Toyota Vios 2022 (BienSoXe: 51A-12345)
UPDATE Xe 
SET GiaThueTheoNgay = 700000,
    MoTa = 'Xe sedan tiết kiệm nhiên liệu, phù hợp đi thành phố, nội thất thoải mái',
    HinhAnh = 'https://images.unsplash.com/photo-1590362891991-f776e747a588?w=800&h=600&fit=crop',
    NgayCapNhat = CURRENT_TIMESTAMP
WHERE BienSoXe = '51A-12345';

-- Cập nhật Honda City 2021 (BienSoXe: 51A-23456)
UPDATE Xe 
SET GiaThueTheoNgay = 750000,
    MoTa = 'Xe bền bỉ, nội thất rộng rãi, động cơ 1.5L tiết kiệm nhiên liệu',
    HinhAnh = 'https://images.unsplash.com/photo-1605559424843-9e4c228bf1c2?w=800&h=600&fit=crop',
    NgayCapNhat = CURRENT_TIMESTAMP
WHERE BienSoXe = '51A-23456';

-- Cập nhật Mazda CX-5 2023 (BienSoXe: 51A-34567)
UPDATE Xe 
SET GiaThueTheoNgay = 1250000,
    MoTa = 'SUV cao cấp, nhiều công nghệ an toàn, động cơ Skyactiv, màn hình HUD',
    HinhAnh = 'https://images.unsplash.com/photo-1617654112368-307921291f42?w=800&h=600&fit=crop',
    MauXe = 'Đỏ Soul Red',
    NgayCapNhat = CURRENT_TIMESTAMP
WHERE BienSoXe = '51A-34567';

-- Cập nhật Ford Everest 2022 (BienSoXe: 51A-45678)
UPDATE Xe 
SET GiaThueTheoNgay = 1500000,
    MoTa = 'SUV 7 chỗ mạnh mẽ, phù hợp đi gia đình, địa hình phức tạp, có chế độ off-road',
    HinhAnh = 'https://images.unsplash.com/photo-1519641471654-76ce0107ad1b?w=800&h=600&fit=crop',
    NgayCapNhat = CURRENT_TIMESTAMP
WHERE BienSoXe = '51A-45678';

-- Cập nhật Kia Morning 2020 (BienSoXe: 51A-56789)
UPDATE Xe 
SET GiaThueTheoNgay = 500000,
    MoTa = 'Xe nhỏ gọn, dễ di chuyển trong đô thị, tiết kiệm nhiên liệu, giá thuê hợp lý',
    HinhAnh = 'https://images.unsplash.com/photo-1552519507-da3b142c6e3d?w=800&h=600&fit=crop',
    NgayCapNhat = CURRENT_TIMESTAMP
WHERE BienSoXe = '51A-56789';

-- Cập nhật Hyundai Accent 2021 (BienSoXe: 51A-67890)
UPDATE Xe 
SET GiaThueTheoNgay = 650000,
    MoTa = 'Xe phổ thông, giá thuê hợp lý, nội thất hiện đại, tiện nghi cơ bản đầy đủ',
    HinhAnh = 'https://images.unsplash.com/photo-1583121274602-3e2820c69888?w=800&h=600&fit=crop',
    NgayCapNhat = CURRENT_TIMESTAMP
WHERE BienSoXe = '51A-67890';

-- Cập nhật Toyota Fortuner 2023 (BienSoXe: 51A-78901)
UPDATE Xe 
SET GiaThueTheoNgay = 1600000,
    MoTa = 'SUV 7 chỗ cao cấp, vận hành ổn định, mạnh mẽ, phù hợp đường dài và địa hình xấu',
    HinhAnh = 'https://images.unsplash.com/photo-1549317661-bd32c8ce0db2?w=800&h=600&fit=crop',
    NgayCapNhat = CURRENT_TIMESTAMP
WHERE BienSoXe = '51A-78901';

-- Cập nhật VinFast Lux A2.0 2022 (BienSoXe: 51A-89012)
UPDATE Xe 
SET GiaThueTheoNgay = 1350000,
    MoTa = 'Sedan cao cấp thương hiệu Việt, nội thất sang trọng, động cơ mạnh mẽ, công nghệ hiện đại',
    HinhAnh = 'https://images.unsplash.com/photo-1614200187524-dc4b892acf16?w=800&h=600&fit=crop',
    NgayCapNhat = CURRENT_TIMESTAMP
WHERE BienSoXe = '51A-89012';

-- Cập nhật Mitsubishi Xpander 2021 (BienSoXe: 51A-90123)
UPDATE Xe 
SET GiaThueTheoNgay = 900000,
    MoTa = 'MPV gia đình 7 chỗ, tiết kiệm nhiên liệu, không gian rộng rãi, phù hợp đi du lịch',
    HinhAnh = 'https://images.unsplash.com/photo-1609521263047-f8f205293f24?w=800&h=600&fit=crop',
    NgayCapNhat = CURRENT_TIMESTAMP
WHERE BienSoXe = '51A-90123';

-- Cập nhật Toyota Innova 2020 (BienSoXe: 51A-01234)
UPDATE Xe 
SET GiaThueTheoNgay = 850000,
    MoTa = 'MPV 7 chỗ rộng rãi, phù hợp đi nhóm, đi du lịch, động cơ bền bỉ',
    HinhAnh = 'https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?w=800&h=600&fit=crop',
    NgayCapNhat = CURRENT_TIMESTAMP
WHERE BienSoXe = '51A-01234';

