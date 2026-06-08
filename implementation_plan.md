# Kế hoạch: Menu Cấu hình Chuyên sâu cho AI Tools (Phase 5)

Mục tiêu: Cho phép người dùng tùy biến hành vi của công cụ AI (ngôn ngữ đích, cách tóm tắt, chỉ thị thêm) thông qua một màn hình cài đặt riêng biệt trong giao diện chính.

## 1. Cấu trúc Dữ liệu (UserSettings.cs)
Sẽ bổ sung các trường cấu hình sau vào hệ thống lưu trữ hiện tại:
- **ExtractLanguage** (Ngôn ngữ bóc tách): Tự động / Tiếng Anh / Tiếng Việt.
- **ExtractCustomPrompt** (Chỉ thị bổ sung cho Extract): Ví dụ: "Chỉ tập trung lấy ngày tháng và số tiền".
- **SummaryLanguage** (Ngôn ngữ Tóm tắt): Tiếng Việt / Tiếng Anh.
- **TranslationLanguage** (Ngôn ngữ Dịch): Tiếng Việt / Tiếng Anh.

## 2. Giao diện (UI) - Cấu hình AI Tools
**Tạo một Menu mới trong phần Cài đặt (Settings):**
- [NEW] `Views/Settings/AIToolsConfigPanel.xaml`: Giao diện bao gồm các Dropdown (ComboBox) để chọn ngôn ngữ và TextBox để nhập yêu cầu thêm (Custom Prompt) cho từng chức năng.
- [NEW] `ViewModels/Settings/AIToolsConfigViewModel.cs`: Quản lý logic lưu và tải cài đặt.
- [MODIFY] `Views/Settings/DashboardPanel.xaml`: Thêm nút điều hướng (tab) "AI Tools" nằm cạnh phần "General", "Hotkeys", "AI Settings".

## 3. Cập nhật Dịch vụ AI (AIService.cs)
- Lấy thông tin cấu hình từ `SettingsManager`.
- Gài các tham số này vào phần **`system` prompt** gửi cho OpenAI.
  - Ví dụ đối với Extract: *"You are an intelligent data extractor. Always return output in {ExtractLanguage}. Additional rules: {ExtractCustomPrompt}"*.
  - Đối với Summarize & Translate: *"Provide a summary in {SummaryLanguage} and a full translation in {TranslationLanguage}."*

---

## User Review Required

> [!IMPORTANT]
> 1. Bạn muốn màn hình Cấu hình này nằm **chung** trong tab AI Settings hiện tại (chỗ nhập API Key), hay tách thành một **tab (menu) riêng biệt** mang tên "AI Behaviors / AI Tools" bên trong mục Cài đặt?
> 2. Có cấu hình nào khác bạn muốn thêm không? (Ví dụ: Giọng điệu dịch thuật: Nghiêm túc/Thoải mái).
