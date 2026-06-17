## 🚀 What's New in v2.0.0

Welcome to **ClipboardAI v2.0**! This major release brings powerful AI and OCR capabilities to your clipboard, transforming how you copy, extract, and process text. 

### ✨ Major Features
- **Snipping OCR (Image to Text)** 📸
  - Added a global `Ctrl + Shift + O` hotkey to draw a snip anywhere on your screen.
  - Instantly extracts text from the captured image using fast, local Windows Media OCR.
  - Automatically copies the extracted text to your clipboard.

- **AI Smart Extraction** 🧠
  - Connect your own OpenAI or Azure OpenAI API keys securely.
  - Extract structured data (names, emails, phone numbers, JSON) from messy clipboard text.
  - **New "AI Behaviors" Menu:** Fully customize the target extraction language and write your own custom rules/prompts!

- **AI Translate & Summarize** 🌐
  - Instantly translate copied text into your language of choice.
  - Generate concise summaries for long texts.
  - Choose the exact **Tone** of the AI (Professional, Friendly, Academic, etc.).
  - Output format beautifully structured with the summary on top and the translation below.

### 🛠 Improvements & Fixes
- Added a new Settings Tab for **AI Behaviors** to handle custom languages and prompts.
- Solved GDI resource leaks (`ObjectDisposedException`) during multiple rapid screen captures.
- Enhanced OpenAI Function Calling integration to guarantee strict JSON schema responses.
- Removed huge `.exe` binaries from git history for a clean, lightweight repository.

*Upgrade now and experience the smartest clipboard manager on Windows 11!*
