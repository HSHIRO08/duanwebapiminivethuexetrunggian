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


select * from datxe


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

-- CHÈN DỮ LIỆU MẪU TIẾP THEO
DO $$
DECLARE 
    v_khach_hang_id INT;
    v_xe_id_1 INT;
    v_xe_id_2 INT;
    v_dat_xe_id INT;
BEGIN
    -- Lấy ID khách hàng và xe hiện có
    SELECT Id INTO v_khach_hang_id FROM KhachHang LIMIT 1;
    SELECT Id INTO v_xe_id_1 FROM Xe WHERE TenXe LIKE '%Toyota Vios%' LIMIT 1;
    SELECT Id INTO v_xe_id_2 FROM Xe WHERE TenXe LIKE '%Mazda CX-5%' LIMIT 1;

    -------------------------------------------------------
    -- 1. TẠO ĐƠN ĐẶT XE (DatXe)
    -------------------------------------------------------
    -- Đơn 1: Đã hoàn thành
    INSERT INTO DatXe (KhachHangId, XeId, NgayBatDau, NgayKetThuc, SoNgayThue, GiaTheoNgay, TongTien, TrangThai)
    VALUES (v_khach_hang_id, v_xe_id_1, NOW() - INTERVAL '5 days', NOW() - INTERVAL '2 days', 3, 700000, 2100000, 'Completed')
    RETURNING Id INTO v_dat_xe_id;

    -- Đơn 2: Đang chờ duyệt
    INSERT INTO DatXe (KhachHangId, XeId, NgayBatDau, NgayKetThuc, SoNgayThue, GiaTheoNgay, TongTien, TrangThai)
    VALUES (v_khach_hang_id, v_xe_id_2, NOW() + INTERVAL '1 days', NOW() + INTERVAL '3 days', 2, 1250000, 2500000, 'Pending');

    -------------------------------------------------------
    -- 2. TẠO THANH TOÁN (ThanhToan) cho đơn đã hoàn thành
    -------------------------------------------------------
    INSERT INTO ThanhToan (DatXeId, MaGiaoDich, SoTien, PhuongThucThanhToan, TrangThai, NgayXacNhan)
    VALUES (v_dat_xe_id, 'PAY-123456789', 2100000, 'BankTransfer', 'Completed', NOW() - INTERVAL '5 days');

    -------------------------------------------------------
    -- 3. TẠO HÓA ĐƠN (HoaDon)
    -------------------------------------------------------
    INSERT INTO HoaDon (DatXeId, MaHoaDon, TongTienThue, PhiDichVu, GiamGia, TongThanhToan, TrangThai)
    VALUES (v_dat_xe_id, 'INV-2024-001', 2100000, 50000, 0, 2150000, 'Paid');

    -------------------------------------------------------
    -- 4. TẠO LỊCH SỬ THUÊ (LichSuThue)
    -------------------------------------------------------
    INSERT INTO LichSuThue (XeId, KhachHangId, DatXeId, NgayNhanXe, NgayTraXe, KmBatDau, KmKetThuc, TrangThaiXe, DanhGia, NhanXet)
    VALUES (v_xe_id_1, v_khach_hang_id, v_dat_xe_id, NOW() - INTERVAL '5 days', NOW() - INTERVAL '2 days', 15000, 15250, 'Good', 5, 'Xe sạch sẽ, chạy rất êm!');

    -------------------------------------------------------
    -- 5. TIN NHẮN HỖ TRỢ (ChatMessage & HoTroKhachHang)
    -------------------------------------------------------
    INSERT INTO ChatMessage (NguoiDungId, SessionId, NoiDung, LoaiTinNhan)
    VALUES (v_khach_hang_id, 'SESSION-001', 'Chào bạn, tôi muốn hỏi về thủ tục thuê xe', 'User');

    INSERT INTO HoTroKhachHang (KhachHangId, TieuDe, NoiDung, LoaiYeuCau, TrangThai, MucDoUuTien)
    VALUES (v_khach_hang_id, 'Hỏi về bảo hiểm', 'Xe có bảo hiểm thân vỏ không shop?', 'Inquiry', 'Open', 'Medium');

END $$;

DO $$
SELECT 
    h.NgayNhanXe, 
    x.TenXe, 
    k.CMND, 
    h.KmBatDau, 
    h.KmKetThuc, 
    h.DanhGia, 
    h.NhanXet
FROM LichSuThue h
JOIN Xe x ON h.XeId = x.Id
JOIN KhachHang k ON h.KhachHangId = k.Id;


-- 1. Tạo Function xử lý logic
CREATE OR REPLACE FUNCTION fn_CapNhatTrangThaiXe()
RETURNS TRIGGER AS $$
BEGIN
    -- Nếu đơn hàng được xác nhận (Confirmed), chuyển xe sang Rented
    IF (TG_OP = 'INSERT' OR TG_OP = 'UPDATE') AND NEW.TrangThai = 'Confirmed' THEN
        UPDATE Xe SET TrangThai = 'Rented', NgayCapNhat = CURRENT_TIMESTAMP 
        WHERE Id = NEW.XeId;
    
    -- Nếu đơn hàng hoàn thành hoặc bị hủy, trả xe về Available
    ELSIF (TG_OP = 'UPDATE') AND (NEW.TrangThai = 'Completed' OR NEW.TrangThai = 'Cancelled') THEN
        UPDATE Xe SET TrangThai = 'Available', NgayCapNhat = CURRENT_TIMESTAMP 
        WHERE Id = NEW.XeId;
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- 2. Tạo Trigger gắn vào bảng DatXe
CREATE TRIGGER trg_CapNhatTrangThaiXe
AFTER INSERT OR UPDATE ON DatXe
FOR EACH ROW
EXECUTE FUNCTION fn_CapNhatTrangThaiXe();

-- -- Chèn một đơn đặt xe mới (Chưa cần tính TongTien vì Trigger sẽ tự làm)
-- INSERT INTO DatXe (KhachHangId, XeId, NgayBatDau, NgayKetThuc, GiaTheoNgay, TrangThai)
-- VALUES (1, 1, '2024-05-01 08:00:00', '2024-05-04 08:00:00', 700000, 'Pending');

-- -- Kiểm tra xem TongTien có tự nhảy lên 2.100.000 không:
-- SELECT Id, SoNgayThue, TongTien FROM DatXe WHERE Id = (SELECT MAX(Id) FROM DatXe);

-- -- Cập nhật trạng thái đơn hàng sang Confirmed
-- UPDATE DatXe SET TrangThai = 'Confirmed' WHERE Id = (SELECT MAX(Id) FROM DatXe);

-- -- Kiểm tra xem Xe tương ứng đã chuyển sang 'Rented' chưa:
-- SELECT Id, TenXe, TrangThai FROM Xe WHERE Id = 1;

ALTER TABLE DatXe ALTER COLUMN SoNgayThue SET DEFAULT 0;
ALTER TABLE DatXe ALTER COLUMN TongTien SET DEFAULT 0;

CREATE OR REPLACE FUNCTION fn_TinhTongTienDatXe()
RETURNS TRIGGER AS $$
BEGIN
    -- 1. Kiểm tra nếu ngày bị trống thì báo lỗi nghiệp vụ thay vì lỗi database
    IF NEW.NgayBatDau IS NULL OR NEW.NgayKetThuc IS NULL THEN
        RAISE EXCEPTION 'NgayBatDau và NgayKetThuc không được để trống';
    END IF;

    -- 2. Tính số ngày (Sử dụng DATE để tính khoảng cách ngày chính xác)
    NEW.SoNgayThue := (NEW.NgayKetThuc::date - NEW.NgayBatDau::date);
    
    -- Nếu thuê trong ngày hoặc lấy xe trả xe cùng ngày, tính là 1 ngày
    IF NEW.SoNgayThue <= 0 THEN 
        NEW.SoNgayThue := 1; 
    END IF;

    -- 3. Tính tổng tiền
    NEW.TongTien := NEW.SoNgayThue * NEW.GiaTheoNgay;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


INSERT INTO DatXe (KhachHangId, XeId, NgayBatDau, NgayKetThuc, GiaTheoNgay, TrangThai)
VALUES (1, 1, '2026-02-01 08:00:00', '2026-02-05 08:00:00', 700000, 'Pending');

-- Xem kết quả (SoNgayThue sẽ tự là 4, TongTien là 2.800.000)
SELECT Id, SoNgayThue, TongTien FROM DatXe ORDER BY Id DESC LIMIT 1;