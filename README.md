# SaleManagerApp
## Thông tin ứng dụng:
+ Ứng dụng quản lý bán hàng dành cho các cửa hàng muốn quản lý công việc kinh doanh, tự động hóa nhiều quy trình quản lý, tính toán, xuất file, xem báo cáo thu chi cho cửa hàng.
+ Chức năng chính :
  - Hiển thị combo gợi ý cho người bán hàng để gợi ý cho khách
  - Hiển thị đồ thị báo cáo thu chi theo ngày, báo cáo phần trăm số lượng món ăn bán ra.
  - Cho phép xem được danh sách món ăn, thêm món ăn, thêm khách hàng, thay đổi trạng thái bàn ăn hiện tại, tạo đơn hàng, xuất hóa đơn, xem danh sách hóa đơn.
  - Cho phép xem chi tiết đơn hàng và thay đổi trạng thái phục vụ của đơn hàng.
  - Cho phép tìm kiếm món ăn.
  - Quản lý nhân viêm. Thêm xóa sửa cập nhật thông tin nhân viên, chấm công nhân viên, set lịch làm cho nhân viên.
  - Quản lý nhập kho xuất kho cho cửa hàng.
  - Chức năng đăng nhập, thay đổi thông tin tài khoản người sử dụng.

## Hướng dẫn chạy ứng dụng:
+ B1: Mở git bash ở một folder rồi tiến hành viết câu lệnh này để clone dự án về máy.
``` Bash
git clone https://github.com/Tuanlee5214/SaleManagerApp.git
```
+ B2: Chạy file SaleManagement1.sql trên ứng dụng SSMS để tạo database cho ứng dụng.
+ B3: Mở file SaleManagerApp.sln để mở code trong VS 2022. 
+ B4: Tiến hành vào folder Service, chọn file DatabaseConnection để chỉnh lại connectString kết nối DB cho phù hợp.
```Bash
Server=TUANLEE\\SQLEXPRESS;Database=SaleManagement20251_12;Trusted_Connection=True
```
+ B5: Đổi tên server TUANLEE sang tên server máy bạn cho phù hợp.
+ B6: Tiến hành chạy và thử nghiệm ứng dụng
  - Tài khoản : admin1
  - Mật khẩu : admin123
  
 
## Công nghệ sử dụng:
+ Database : SQL Server
+ Ngôn ngữ : C#
+ UI : WPF C#
+ Kiến trúc thick client không có backend, UI vừa chứa logic xử lí giao diện và tương tác với db.

## Người thực hiện
+ Lê Anh Tuấn: 23521711
+ Hồ Nhật Thành : 23521439
+ Bùi Đức Anh : 24520083
