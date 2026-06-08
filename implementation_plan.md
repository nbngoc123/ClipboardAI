# Phase 4: Snipping Tool & Auto OCR (Chụp màn hình và Tự động trích xuất)

Mục tiêu: Xây dựng một tính năng giống hệt Snipping Tool của Windows nhưng thông minh hơn. Khi bấm phím tắt, bạn có thể quét một vùng trên màn hình, ứng dụng sẽ tự động chụp vùng đó, nhận diện toàn bộ chữ viết bên trong (OCR) và lưu thẳng vào Clipboard/Lịch sử.

## Ý tưởng cốt lõi
1. **Phím tắt toàn cầu (Global Hotkey):** Cấu hình một phím tắt mới (ví dụ: `Ctrl + Shift + O` hoặc `Win + Shift + E`).
2. **Snipping Overlay (Lớp phủ màn hình):** Khi bấm phím tắt, một màn hình làm mờ sẽ phủ lên toàn bộ các màn hình hiện tại. Con trỏ chuột biến thành dấu thập (`+`). Bạn nhấn giữ và kéo chuột để chọn một vùng sáng trên màn hình.
3. **Capture & OCR (Chụp & Nhận diện):** Ngay khi nhả chuột, ứng dụng sẽ chụp vùng sáng đó và ném vào công cụ `Windows.Media.Ocr.OcrEngine`.
4. **Kết quả tức thì:** Chữ viết bóc tách được sẽ tự động được copy thẳng vào Clipboard (và lưu vào Lịch sử) để bạn dán ngay vào bất cứ đâu.

---

## Các thành phần thay đổi (Proposed Changes)

### 1. Cấu hình Project
- Đổi `TargetFramework` thành `net10.0-windows10.0.19041.0` để kích hoạt API WinRT (cho phép gọi `Windows.Media.Ocr`).

---

### 2. Dịch vụ OCR (Services)
#### [NEW] `Services/OCR/IOcrService.cs` & `Services/OCR/OcrService.cs`
- Hàm `ExtractTextFromImageAsync(BitmapSource image)`: Nhận vào bức ảnh, dùng `OcrEngine` (siêu nhanh, nhẹ, đa ngôn ngữ, built-in của Win10/11) để trích xuất text.

---

### 3. Giao diện Chụp màn hình (Snipping Overlay)
#### [NEW] `Views/Windows/SnippingWindow.xaml`
- Một Window trong suốt, `WindowStyle="None"`, `Topmost="True"`, `AllowsTransparency="True"`, phủ lên toàn bộ màn hình.
- Xử lý các sự kiện chuột (`MouseDown`, `MouseMove`, `MouseUp`) để vẽ một hình chữ nhật biểu diễn vùng đang chọn.
- Dùng `System.Drawing.Graphics.CopyFromScreen` hoặc tương đương để chụp phần ảnh bên dưới vùng chữ nhật.

#### [NEW] `ViewModels/Snipping/SnippingViewModel.cs`
- (Hoặc đưa logic trực tiếp vào Code-behind của Window do bản chất vẽ đồ hoạ và tọa độ chuột gắn chặt chặt với View).

---

### 4. Quản lý Hotkey và Luồng hoạt động
#### [MODIFY] `Infrastructure/ServiceRegistration.cs`
- Đăng ký `OcrService`.

#### [MODIFY] `ViewModels/MainViewModel.cs` (Hoặc nơi quản lý Hotkey)
- Khởi tạo hotkey `Ctrl + Shift + O`.
- Khi kích hoạt:
  1. Mở `SnippingWindow.ShowDialog()`.
  2. Lấy được `BitmapSource` vùng cắt.
  3. Gửi cho `OcrService`.
  4. Lấy kết quả Text -> Ghi vào `Clipboard.SetText()`.
  5. Gọi API lưu vào DB/Lịch sử (đã có sẵn).

---

## Kế hoạch kiểm thử (Verification Plan)
1. Bấm `Ctrl + Shift + O`.
2. Màn hình tối đi một chút.
3. Kéo chuột chọn một vùng có chứa chữ.
4. Ngay khi nhả chuột, màn hình trở lại bình thường, và một âm báo nhỏ / thông báo nhỏ (Toast) báo hiệu OCR thành công.
5. Ấn `Ctrl + V` vào Notepad, chữ sẽ xuất hiện y hệt trong ảnh chụp.

> [!TIP]  
> Bằng cách này, chúng ta bỏ qua bước "Phải mở Clipboard Popup rồi mới bấm nút OCR". Mọi thứ liền mạch và tốc độ hơn rất nhiều!

---

## User Review Required

> [!IMPORTANT]  
> 1. Bạn đồng ý sử dụng phím tắt **`Ctrl + Shift + O`** cho tính năng chụp màn hình lấy chữ này chứ? (Hay bạn muốn dùng tổ hợp phím khác?)
> 2. Có cần lưu cả Bức ảnh vừa chụp vào lịch sử không, hay **chỉ cần lưu Chữ** (Text) vừa bóc tách được để tiết kiệm bộ nhớ?
