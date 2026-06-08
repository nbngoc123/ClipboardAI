# ClipboardAI — Project Rules & Guidelines

> WPF Desktop App | .NET 10 | MVVM | SQLite | AI + OCR  
> Mục tiêu: Clipboard Manager chuyên nghiệp chạy nền, nhẹ, nhanh, thông minh.

---

## 1. Cấu trúc thư mục

# ClipboardAI — Cấu trúc thư mục chi tiết

> Nguyên tắc: **1 file = 1 trách nhiệm = tối đa ~100 dòng**  
> Mỗi class chỉ làm đúng 1 việc. Nếu cần đặt tên với "And" hoặc "Manager" → tách tiếp.

---

## Tổng quan

```
ClipboardAI/
├── Models/
├── Data/
│   ├── Migrations/
│   └── Repositories/
├── Services/
│   ├── Clipboard/
│   ├── Hotkey/
│   ├── Tray/
│   ├── AI/
│   └── OCR/
├── ViewModels/
│   ├── History/
│   ├── AI/
│   └── Settings/
├── Views/
│   ├── Controls/
│   ├── Popups/
│   └── Windows/
├── Helpers/
│   ├── Converters/
│   └── Extensions/
├── Infrastructure/
└── App.xaml
```

---

## Chi tiết từng thư mục

### Models/
> POCO thuần — không có logic, không có dependency. Mỗi file = 1 class.

```
Models/
├── ClipboardItem.cs              # Entity chính: Id, Content, Type, CreatedAt, IsPinned
├── ClipboardContentType.cs       # Enum: Text, Image, FilePath, RichText, Html
├── AppSettings.cs                # Tất cả settings của user
├── AIRequest.cs                  # Input gửi lên AI API
├── AIResponse.cs                 # Output nhận về từ AI API
├── AIActionType.cs               # Enum: Summarize, Translate, FixGrammar, ExplainCode
├── HotkeyDefinition.cs           # Model 1 hotkey: Keys, ActionName, IsEnabled
├── OcrResult.cs                  # Kết quả OCR: Text, Confidence, BoundingBox
└── TrayMenuItem.cs               # Model cho từng item trong tray menu
```

---

### Data/
> Chỉ biết về SQLite và Models. Không biết về UI hay Services.

```
Data/
├── DatabaseContext.cs            # Mở connection, tạo tables, PRAGMA settings
├── DatabaseInitializer.cs        # Chạy migration, seed default settings
├── Migrations/
│   ├── Migration_001_Initial.cs  # CREATE TABLE ClipboardItems, AppSettings
│   ├── Migration_002_Tags.cs     # ALTER TABLE thêm cột Tags
│   └── MigrationRunner.cs        # Chạy migrations theo thứ tự, track version
└── Repositories/
    ├── IClipboardRepository.cs   # Interface
    ├── ClipboardRepository.cs    # GetRecent, GetPinned, Insert, Delete, Search
    ├── ISettingsRepository.cs    # Interface
    └── SettingsRepository.cs     # Get<T>, Set<T> theo key
```

---

### Services/

#### Services/Clipboard/
> Tách biệt: ai detect thay đổi, ai xử lý content, ai hash, ai raise event.

```
Services/Clipboard/
├── IClipboardService.cs          # Interface public của toàn bộ service
├── ClipboardService.cs           # Orchestrator: khởi động/dừng polling, wire các phần
├── ClipboardPoller.cs            # DispatcherTimer 500ms, gọi IClipboardReader
├── ClipboardReader.cs            # Đọc Clipboard.GetText/GetImage/GetFileDropList
├── ClipboardContentDetector.cs   # Nhận raw data → trả ClipboardContentType
├── ClipboardHasher.cs            # MD5 hash content → detect trùng lặp
├── ClipboardItemFactory.cs       # Tạo ClipboardItem từ raw clipboard data
└── ClipboardImageProcessor.cs    # Resize ảnh → thumbnail, convert Bitmap → byte[]
```

#### Services/Hotkey/
```
Services/Hotkey/
├── IHotkeyService.cs             # Register, Unregister, OnHotkeyPressed event
├── HotkeyService.cs              # Wrap NHotkey.Wpf, quản lý danh sách hotkeys
├── HotkeyRegistrar.cs            # Đăng ký từng hotkey, xử lý conflict
└── HotkeyActionDispatcher.cs     # Map hotkey → action (OpenPopup, PasteSlot, OCR...)
```

#### Services/Tray/
```
Services/Tray/
├── ITrayIconService.cs           # Show, Hide, UpdateTooltip, SetIcon event
├── TrayIconService.cs            # Wrap Hardcodet.NotifyIcon, lifecycle
├── TrayMenuBuilder.cs            # Xây dựng ContextMenuStrip từ danh sách action
└── TrayIconStateManager.cs       # Đổi icon theo trạng thái (normal/recording/ai-busy)
```

#### Services/AI/
```
Services/AI/
├── IAIService.cs                 # ProcessAsync(item, action) → AIResponse
├── AIService.cs                  # Orchestrator: cache check → call → parse
├── AIApiClient.cs                # HttpClient, gọi endpoint, retry logic
├── AIRequestBuilder.cs           # Tạo prompt từ AIRequest + AIActionType
├── AIResponseParser.cs           # Parse JSON response → AIResponse model
├── AIResultCache.cs              # Dictionary<hash, AIResponse>, eviction policy
└── AIPromptTemplates.cs          # Const strings: template cho từng ActionType
```

#### Services/OCR/
```
Services/OCR/
├── IOCRService.cs                # CaptureAndExtractAsync() → OcrResult
├── OCRService.cs                 # Orchestrator: chụp → xử lý → trả text
├── ScreenRegionSelector.cs       # Overlay WPF chọn vùng màn hình, trả Rectangle
├── ScreenCapture.cs              # Chụp vùng màn hình → Bitmap
├── TesseractEngine.cs            # Wrap Tesseract.NET, init engine, OCR bitmap
└── OCRPostProcessor.cs           # Làm sạch text: trim, remove artifact chars
```

---

### ViewModels/

```
ViewModels/
├── MainViewModel.cs              # Shell: quản lý panel nào đang active, startup logic
│
├── History/
│   ├── HistoryViewModel.cs       # ObservableCollection, pagination, selected item
│   ├── HistorySearchViewModel.cs # Search query, filter logic, debounce timer
│   ├── HistoryItemViewModel.cs   # ViewModel cho 1 item trong list (wrap ClipboardItem)
│   └── HistoryActionsViewModel.cs# Pin, Delete, CopyTo actions trên item
│
├── AI/
│   ├── AIViewModel.cs            # Panel AI: current item, action buttons, result
│   ├── AIActionViewModel.cs      # 1 nút action: label, icon, command, IsLoading
│   └── AIResultViewModel.cs      # Hiển thị kết quả: text, CopyResult command
│
└── Settings/
    ├── SettingsViewModel.cs      # Shell settings, điều hướng giữa các tab
    ├── GeneralSettingsViewModel.cs   # Startup, giới hạn lịch sử, theme
    ├── HotkeySettingsViewModel.cs    # Danh sách hotkeys, cho phép rebind
    ├── AISettingsViewModel.cs        # API key, model, ngôn ngữ dịch
    └── StorageSettingsViewModel.cs   # Đường dẫn db, export, clear history
```

---

### Views/

```
Views/
├── Windows/
│   ├── MainWindow.xaml / .cs         # Shell chính
│   ├── MainWindow.Startup.cs         # Partial: logic khởi động, DI wiring
│   ├── SettingsWindow.xaml / .cs     # Cửa sổ settings
│   └── OcrOverlayWindow.xaml / .cs   # Fullscreen overlay chọn vùng OCR
│
├── Popups/
│   ├── ClipboardPopup.xaml / .cs     # Popup Ctrl+Shift+V (Popup hoặc Window nhỏ)
│   └── QuickPastePopup.xaml / .cs    # Popup Ctrl+1..9 chọn slot
│
├── Controls/
│   ├── HistoryPanel.xaml / .cs           # Danh sách lịch sử
│   ├── HistoryItemControl.xaml / .cs     # 1 item trong danh sách
│   ├── SearchBar.xaml / .cs              # Thanh tìm kiếm có debounce
│   ├── AIPanel.xaml / .cs               # Panel AI bên phải
│   ├── AIActionButton.xaml / .cs         # 1 nút action AI
│   ├── AIResultDisplay.xaml / .cs        # Hiển thị kết quả AI
│   └── ContentTypeIcon.xaml / .cs        # Icon nhỏ Text/Image/File
│
└── Settings/
    ├── GeneralSettingsPanel.xaml / .cs
    ├── HotkeySettingsPanel.xaml / .cs
    ├── AISettingsPanel.xaml / .cs
    └── StorageSettingsPanel.xaml / .cs
```

---

### Helpers/

```
Helpers/
├── Converters/                        # IValueConverter cho XAML binding
│   ├── BoolToVisibilityConverter.cs
│   ├── ContentTypeToIconConverter.cs
│   ├── DateTimeToRelativeConverter.cs # "2 phút trước", "hôm qua"...
│   ├── StringTruncateConverter.cs     # Cắt ngắn text dài trong list
│   └── BytesToImageConverter.cs       # byte[] → BitmapImage cho ảnh
│
├── Extensions/
│   ├── StringExtensions.cs            # Truncate, IsNullOrWhiteSpace, GetHashMd5
│   ├── BitmapExtensions.cs            # ToByteArray, ToThumbnail, ToDpi96
│   ├── ClipboardItemExtensions.cs     # GetPreview, IsExpired, ToDisplayString
│   └── TaskExtensions.cs              # FireAndForget, WithTimeout
│
├── DispatcherHelper.cs                # RunOnUIThread, InvokeAsync wrapper
├── SecureStorage.cs                   # DPAPI: Encrypt/Decrypt string (cho API key)
├── AppPaths.cs                        # Const: đường dẫn db, log, temp folder
└── AppConstants.cs                    # MAX_HISTORY, POLL_INTERVAL, VERSION...
```

---

### Infrastructure/
> Wiring toàn bộ app — chỉ file này được biết tất cả các class.

```
Infrastructure/
├── ServiceLocator.cs              # DI container setup (Microsoft.Extensions.DI)
├── ServiceRegistration.cs         # Đăng ký tất cả services, viewmodels, repos
├── AppBootstrapper.cs             # Khởi động theo thứ tự: DB → Services → UI
├── AppShutdownHandler.cs          # Cleanup khi tắt: VACUUM db, unregister hotkeys
└── GlobalExceptionHandler.cs      # Catch unhandled exception, log, show dialog
```

---

## Ví dụ tách file thực tế

### Trước (1 file bẩn ~250 dòng)
```
Services/ClipboardService.cs  ← polling + hash + detect + factory + event
```

### Sau (6 file sạch ~50–80 dòng mỗi file)
```
Services/Clipboard/
├── ClipboardService.cs           # ~60 dòng  — chỉ orchestrate
├── ClipboardPoller.cs            # ~50 dòng  — chỉ timer
├── ClipboardReader.cs            # ~70 dòng  — chỉ đọc clipboard
├── ClipboardContentDetector.cs   # ~40 dòng  — chỉ detect type
├── ClipboardHasher.cs            # ~30 dòng  — chỉ hash
└── ClipboardItemFactory.cs       # ~60 dòng  — chỉ tạo model
```

---

## Quy tắc tách file

| Dấu hiệu cần tách | Cách tách |
|---|---|
| Method > 20 dòng | Tách method ra class riêng |
| Class có > 3 dependencies inject | Tách bớt trách nhiệm |
| Tên class chứa "And", "Manager", "Helper" chung chung | Đặt tên cụ thể hơn, tách ra |
| Constructor > 5 dòng khởi tạo | Tách Factory hoặc Builder |
| File > 80 dòng (warning) / > 100 dòng (bắt buộc tách) | Tách ngay |
| Có 2 vòng lặp lồng nhau | Tách vòng trong ra method/class riêng |

---

## Số lượng file ước tính

| Thư mục | Số file | Dòng TB/file |
|---|---|---|
| Models | 9 | ~30 |
| Data | 7 | ~60 |
| Services | 24 | ~60 |
| ViewModels | 13 | ~80 |
| Views | 16 | ~70 |
| Helpers | 12 | ~50 |
| Infrastructure | 5 | ~70 |
| **Tổng** | **~86 files** | **~60 dòng** |

> ~86 files × ~60 dòng = ~5,200 dòng code tổng — một app WPF hoàn chỉnh, sạch, dễ maintain.


---

## 2. Tech Stack & Packages

| Package | Phiên bản | Mục đích |
|---|---|---|
| `CommunityToolkit.Mvvm` | 8.4.2 | ObservableObject, RelayCommand, Messenger |
| `Microsoft.Data.Sqlite` | 10.0.8 | SQLite database |
| `Dapper` | latest | Query SQLite đơn giản hơn |
| `Hardcodet.NotifyIcon.Wpf` | 2.0.1 | System tray icon |
| `NHotkey.Wpf` | 4.0.0 | Global hotkeys |
| `Tesseract` | latest | OCR engine |
| `System.Text.Json` | built-in | Serialize settings, AI response |

---

## 3. Kiến trúc — MVVM Strict

### Nguyên tắc bắt buộc

- **View** chỉ chứa XAML + code-behind tối thiểu (event → delegate tới ViewModel).
- **ViewModel** không được `import` bất cứ thứ gì từ `System.Windows` (không dùng `MessageBox`, `Dispatcher` trực tiếp).
- **Service** không biết gì về UI. Chỉ nhận input thuần, trả về output thuần.
- **Model** là POCO thuần — không có logic, không có dependency.
- Giao tiếp giữa ViewModel ↔ ViewModel dùng `WeakReferenceMessenger` của CommunityToolkit.

### Luồng dữ liệu

```
Windows Clipboard
      ↓
ClipboardService (event ClipboardChanged)
      ↓
MainViewModel (subscribe event, gọi Repository)
      ↓
ClipboardRepository (lưu SQLite)
      ↓
HistoryViewModel (ObservableCollection → bind tới View)
      ↓
HistoryPanel.xaml (hiển thị danh sách)
```

---

## 4. Quy tắc đặt tên

### C# — theo chuẩn Microsoft

| Loại | Quy tắc | Ví dụ |
|---|---|---|
| Class, Interface | PascalCase | `ClipboardService`, `IRepository` |
| Method, Property | PascalCase | `GetAllItems()`, `IsPinned` |
| Private field | `_camelCase` | `_dbContext`, `_items` |
| Constant | `UPPER_SNAKE` | `MAX_HISTORY_ITEMS` |
| Enum value | PascalCase | `ContentType.Text` |
| Event | `OnXxx` / PascalCase | `ClipboardChanged`, `OnItemSelected` |
| Async method | suffix `Async` | `GetItemsAsync()` |

### XAML

- Name của control: `PascalCase` + hậu tố loại — `HistoryListBox`, `SearchTextBox`
- Resource key: `PascalCase` — `PrimaryButtonStyle`, `HistoryItemTemplate`
- Binding dùng `x:Bind` ưu tiên hơn `{Binding}` khi có thể

---

## 5. Quy tắc code

### Bắt buộc

```csharp
// ✅ Luôn dùng async/await cho I/O
public async Task<List<ClipboardItem>> GetRecentAsync(int limit = 50)
{
    using var conn = _context.CreateConnection();
    return (await conn.QueryAsync<ClipboardItem>(
        "SELECT * FROM ClipboardItems ORDER BY CreatedAt DESC LIMIT @limit",
        new { limit }
    )).ToList();
}

// ✅ Nullable enable — luôn xử lý null
public string? PreviewText { get; set; }

// ✅ Dùng pattern matching thay if-else dài
var label = item.ContentType switch
{
    ClipboardContentType.Text     => "📄 Text",
    ClipboardContentType.Image    => "🖼️ Image",
    ClipboardContentType.FilePath => "📁 File",
    _                             => "Unknown"
};
```

### Cấm

```csharp
// ❌ Không bắt Exception chung chung
catch (Exception) { } // BAD — nuốt lỗi im lặng

// ❌ Không để connection mở tự do
var conn = new SqliteConnection(cs); // BAD — phải dùng using

// ❌ Không gọi UI từ thread khác
Task.Run(() => MyCollection.Add(item)); // BAD — sẽ crash

// ✅ Đúng cách — marshal về UI thread
await Application.Current.Dispatcher.InvokeAsync(() => Items.Add(item));
```

### Error handling

```csharp
// Dùng try/catch có log, không nuốt lỗi
try
{
    await _aiService.ProcessAsync(text);
}
catch (HttpRequestException ex)
{
    Debug.WriteLine($"[AIService] HTTP error: {ex.Message}");
    StatusMessage = "Không kết nối được AI. Kiểm tra API key.";
}
catch (OperationCanceledException)
{
    StatusMessage = "Đã hủy.";
}
```

---

## 6. Database — SQLite Schema

```sql
-- ClipboardItems
CREATE TABLE IF NOT EXISTS ClipboardItems (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    Content     TEXT    NOT NULL,
    ContentType INTEGER NOT NULL DEFAULT 0,  -- enum ClipboardContentType
    CreatedAt   TEXT    NOT NULL,            -- ISO 8601: "2026-06-08T09:22:00"
    IsPinned    INTEGER NOT NULL DEFAULT 0,
    PreviewText TEXT,                        -- tóm tắt ngắn hoặc thumbnail path
    Tags        TEXT                         -- JSON array: ["url","code","email"]
);

-- AppSettings (key-value)
CREATE TABLE IF NOT EXISTS AppSettings (
    Key   TEXT PRIMARY KEY,
    Value TEXT
);

-- Index tìm kiếm nhanh
CREATE INDEX IF NOT EXISTS idx_createdat ON ClipboardItems(CreatedAt DESC);
CREATE INDEX IF NOT EXISTS idx_pinned    ON ClipboardItems(IsPinned DESC);
```

### Quy tắc database

- File db lưu tại: `%LOCALAPPDATA%\ClipboardAI\clipboard.db`
- Tự động tạo folder nếu chưa có khi khởi động.
- Giới hạn lịch sử: mặc định **500 items** — tự xóa items cũ nhất khi vượt quá (trừ pinned).
- Không dùng ORM nặng (EF Core) — dùng Dapper cho gọn.
- Mỗi lần app tắt: gọi `VACUUM` để giảm kích thước db.

---

## 7. Các Service chính

### ClipboardService

```csharp
// Polling 500ms — đơn giản, ổn định hơn WndProc hook
// Detect: Text, Image (Bitmap), FilePath (StringCollection)
// Bỏ qua nếu content trùng với item trước (so sánh hash MD5)
// Raise event trên UI thread
```

### HotkeyService

| Hotkey | Hành động |
|---|---|
| `Ctrl+Shift+V` | Mở/đóng popup lịch sử |
| `Ctrl+Shift+C` | Chụp OCR vùng màn hình |
| `Ctrl+1` đến `Ctrl+9` | Paste item được pin thứ 1–9 |
| `Escape` | Đóng popup |

### AIService

- Model: `claude-3-haiku` (nhanh, rẻ) cho các tác vụ thường.
- Timeout: 10 giây — hiển thị spinner trong lúc chờ.
- Cache kết quả AI theo `Content.GetHashCode()` — tránh gọi API lặp lại.
- Các tính năng:
  - **Summarize** — tóm tắt đoạn text dài
  - **Translate** — dịch sang ngôn ngữ được chọn trong Settings
  - **Fix grammar** — sửa lỗi chính tả, ngữ pháp
  - **Explain code** — giải thích đoạn code
  - **Smart tag** — tự phân loại: URL, email, số điện thoại, code, địa chỉ...

### OCRService

```csharp
// 1. Dùng PrintScreen hook hoặc vẽ overlay để chọn vùng
// 2. Crop bitmap theo vùng chọn
// 3. Tesseract.NET xử lý → trả string
// 4. Tự động copy vào clipboard + lưu lịch sử
// Language pack: eng + vie (tiếng Việt)
```

---

## 8. Phát triển theo Phase

### Phase 1 — Core Foundation (Tuần 1–2)

**Mục tiêu:** App chạy được, lưu lịch sử clipboard.

- [ ] Tạo `DatabaseContext` + migration tự động
- [ ] Tạo `ClipboardItem` model + `ClipboardRepository`
- [ ] Viết `ClipboardService` — polling 500ms, detect Text/Image/File
- [ ] `MainViewModel` + `HistoryViewModel` — ObservableCollection
- [ ] `MainWindow` + `HistoryPanel` — ListView hiển thị items
- [ ] Click item → copy vào clipboard
- [ ] App minimize xuống tray thay vì đóng

**Definition of Done Phase 1:**
> Copy text bất kỳ → thấy trong danh sách → click vào → paste được.

---

### Phase 2 — Hotkey + Multi Clipboard (Tuần 3)

**Mục tiêu:** Dùng được mà không cần chuột.

- [ ] `HotkeyService` — đăng ký `Ctrl+Shift+V` global
- [ ] Popup overlay hiện ra tại vị trí chuột, tắt khi mất focus
- [ ] `TrayIconService` — right-click menu: Open, Clear History, Settings, Exit
- [ ] Pin items — giữ ở đầu danh sách, không bị xóa auto
- [ ] `Ctrl+1..9` paste item pin nhanh
- [ ] Thanh tìm kiếm realtime trong popup (filter theo content)
- [ ] Startup with Windows (Registry `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`)

**Definition of Done Phase 2:**
> Ctrl+Shift+V bất kỳ đâu → popup hiện ra → gõ tìm → Enter để paste.

---

### Phase 3 — AI Integration (Tuần 4–5)

**Mục tiêu:** AI xử lý nội dung clipboard thông minh.

- [ ] `AIService` — gọi API, xử lý response, cache kết quả
- [ ] `AIPanel` — sidebar bên phải MainWindow
- [ ] Nút action trên mỗi item: Summarize / Translate / Fix / Explain
- [ ] Auto-tag items sau khi lưu (background task)
- [ ] Filter theo tag trong lịch sử
- [ ] Settings: nhập API key, chọn model, chọn ngôn ngữ dịch

**Definition of Done Phase 3:**
> Copy đoạn text tiếng Anh → bấm Translate → ra tiếng Việt → copy tiếp.

---

### Phase 4 — OCR + Polish (Tuần 6–7)

**Mục tiêu:** Chụp màn hình lấy text, app hoàn chỉnh.

- [ ] `OCRService` — tích hợp Tesseract, language pack eng+vie
- [ ] Overlay chọn vùng màn hình (dimmed background + crosshair)
- [ ] `Ctrl+Shift+C` → chọn vùng → text tự vào clipboard + lịch sử
- [ ] Xem trước ảnh trong danh sách (thumbnail)
- [ ] `SettingsWindow` đầy đủ: hotkeys, giới hạn lịch sử, theme, API key
- [ ] Export lịch sử ra `.txt` / `.json`
- [ ] Auto-update check (GitHub Releases)

**Definition of Done Phase 4:**
> Chụp ảnh chứa text → OCR → paste text vào bất kỳ đâu.

---

## 9. Quy tắc Git

### Branch

```
main          ← production, luôn build được
develop       ← integration branch
feature/xxx   ← tính năng mới (từ develop)
fix/xxx       ← bug fix
```

### Commit message

```
feat: thêm ClipboardService polling
fix: crash khi clipboard chứa ảnh lớn
refactor: tách AIService ra interface
docs: cập nhật README
chore: thêm package Dapper
```

### Quy tắc

- Không commit thẳng vào `main`.
- Mỗi feature = 1 branch, merge qua PR.
- Không commit: `bin/`, `obj/`, `.vs/`, `*.user`, file db, API key.
- File `.gitignore` phải có từ ngày đầu.

---

## 10. Performance Rules

- **Clipboard polling** chỉ chạy khi app active hoặc có tray icon — dừng khi user tắt tính năng.
- **Lịch sử hiển thị** dùng `VirtualizingStackPanel` — không render tất cả 500 items cùng lúc.
- **Ảnh trong clipboard** lưu dưới dạng thumbnail (max 200×200px) vào db, không lưu full size.
- **AI calls** luôn chạy trên background thread, không block UI.
- **Database queries** đều `async` — không có synchronous query nào trên UI thread.
- App chiếm RAM < 80MB khi chạy nền bình thường.

---

## 11. Security Rules

- **API key** lưu bằng `Windows Data Protection API (DPAPI)` — không lưu plaintext trong db hay registry.
- **Clipboard content** không gửi lên server trừ khi user chủ động bấm nút AI.
- Hiển thị warning trước khi gửi nội dung clipboard lên AI lần đầu.
- Không log clipboard content ra console/file.

```csharp
// Lưu API key an toàn
var encrypted = ProtectedData.Protect(
    Encoding.UTF8.GetBytes(apiKey),
    null,
    DataProtectionScope.CurrentUser
);
```

---

## 12. UI/UX Rules

- App không có taskbar button khi chạy nền — chỉ hiện tray icon.
- Popup lịch sử: mở trong < 100ms, đóng ngay khi mất focus.
- Tất cả action có thể dùng bằng bàn phím (Tab, Enter, Arrow keys).
- Mỗi item trong danh sách hiển thị: preview text (truncate 100 ký tự), thời gian tương đối ("2 phút trước"), icon loại content.
- Không có dialog xác nhận cho thao tác thường — chỉ hỏi khi "Xóa toàn bộ lịch sử".
- Dark mode / Light mode theo Windows system setting (`SystemParameters`).
- Animation: chỉ fade in/out popup — không dùng animation phức tạp làm chậm app.

---

## 13. File cần tạo ngay từ đầu

```
.gitignore              ← Bắt buộc ngày đầu
README.md               ← Mô tả ngắn + cách chạy
CHANGELOG.md            ← Ghi lại thay đổi theo version
appsettings.json        ← Default config (không chứa secret)
ClipboardAI.sln
```

### `.gitignore` tối thiểu

```
bin/
obj/
.vs/
*.user
*.db
*.db-shm
*.db-wal
**/secrets.json
```

---

*Tài liệu này là source of truth cho dự án. Cập nhật khi có quyết định kiến trúc mới.*
