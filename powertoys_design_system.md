# Design rules — PowerToys style (Fluent Design)

Đây là tài liệu quy chuẩn thiết kế giao diện ứng dụng ClipboardAI, được biên dịch dựa trên hệ thống thiết kế Windows 11 Fluent Design và tiện ích PowerToys. Toàn bộ mã XAML phải tuân thủ nghiêm ngặt các quy tắc này.

## 1. Layout
- **Sidebar:** Điều hướng cố định bên trái, độ rộng `~220px`. Background sáng hơn content một chút để tạo chiều sâu (depth).
- **Content Area:** Khu vực bên phải cuộn độc lập (Scrollable). Padding chuẩn: `20px 24px`. Không có thanh tiêu đề nổi (No floating header bar).
- **Nav Groups:** Sidebar chia thành các sections bằng nhãn nhỏ in hoa (uppercase). Mỗi item gồm: `Icon + Label + Status Indicator`.

## 2. Component patterns

- **Hero toggle:** 
  - Mỗi trang tính năng (utility) bắt đầu bằng một toggle lớn để Bật/Tắt toàn bộ tính năng đó.
  - Phải nổi bật hơn các toggle thường bằng padding nhiều hơn và có border riêng.

- **Section card:**
  - Nhóm các cài đặt liên quan vào một thẻ (card).
  - Có tiêu đề `section-title` in hoa ở trên cùng.
  - Các hàng (row) bên trong được ngăn cách bởi viền dưới `border-bottom 0.5px`. 
  - Hàng cuối cùng KHÔNG có border.

- **Setting row:**
  - Bố cục `space-between`: Bên trái là `Tên + Mô tả`, bên phải là Control (Công tắc, Select, Badge...).
  - Hiệu ứng Hover: Có background nền mờ sáng lên nhẹ nhàng.

- **Toggle Switch:**
  - Kích thước: `40 × 22px`.
  - Rãnh (Track) đổi màu accent khi được bật (checked). Nút trượt (Thumb) kích thước `18px`.

- **Keyboard badge (Keycap):**
  - Text hiển thị phím tắt phải được style giống keycap bàn phím vật lý.
  - Viền dưới (border-bottom) phải dày hơn để tạo cảm giác nổi 3D nhẹ.

## 3. Màu sắc (Colors)
- **Accent Color:** `#0078d4` (Xanh Windows) — CHỈ DÙNG cho toggle đang bật, link, primary button, và focus border.
- **Surface Content:** `#2c2c2c` (Dark mode nền tối).
- **Surface Nav (Sidebar):** Phải sáng hơn `#2c2c2c` một chút (ví dụ `#333333`) để tạo Depth. Hoặc ngược lại tuỳ tone chuẩn.
- **Status dot:** 
  - Màu xanh lá (`#4CAF50` hoặc `#00CC6A`) = Enabled.
  - Màu xám = Disabled.
  - KHÔNG dùng text cho trạng thái này trên sidebar.

## 4. Typography (Phông chữ)
- **Font:** `Segoe UI` (fallback về system-ui).
- **Weight:** KHÔNG dùng weight 600/700. Chỉ dùng `400` (Regular) và `500` (Medium).
- **Section title:** Font size `11px`, In hoa (Uppercase), Letter-spacing `0.5px`, màu xám nhạt (muted).

## 5. Tương tác (Interactions)
- **Hover Rows:** Tất cả Setting rows khi hover phải có hiệu ứng đổi background (transition 0.1s).
- Tham số tương tác thay đổi trực tiếp (real-time) nếu có.
