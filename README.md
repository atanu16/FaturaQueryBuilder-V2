# Fatura Query Builder

A lightweight Windows desktop app that converts a list of Fatura lines into a ready-to-paste email subject search query.

<img width="652" height="587" alt="image" src="https://github.com/user-attachments/assets/90becb6c-2f91-4ca7-9c21-df40755a78ff" />
<img width="658" height="589" alt="image" src="https://github.com/user-attachments/assets/7a949d80-41f4-41a2-aacb-3e634679575a" />



---

## What it does

If you work with invoices tracked by subject lines like:

```
Fatura 1234567 cliente 1234567
Fatura 1234567 cliente 1234567
Fatura 1234567 cliente 1234567
```

This tool turns that list into a single query you can paste directly into any email client's search box:

```
Subject: "Fatura 1234567 cliente 1234567" OR Subject: "Fatura 1234567 cliente 1234567" OR Subject: "Fatura 1234567 cliente 1234567"
```

---

## Features

- **Batch processing** — paste any number of Fatura lines at once
- **Live counters** — shows line count and character count as you type
- **Validation** — skips and reports malformed lines without blocking valid ones
- **One-click copy** — copies the full query to clipboard instantly
- **Dark / Light mode** — toggle between themes with the switch in the header
- **Toast notifications** — subtle confirmation when copying to clipboard

---

## Requirements

- Windows 10 or Windows 11
- [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

---

## Getting started

### Run from source

```bash
git clone https://github.com/your-username/FaturaQueryBuilder.git
cd FaturaQueryBuilder
dotnet run
```

### Build a release executable

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The executable will be in `bin\Release\net8.0-windows\win-x64\publish\`.

---

## Usage

1. Paste your Fatura lines into the **INPUT** box (one per line).
2. Click **Process**.
3. Review the generated query in the **OUTPUT** box.
4. Click **Copy to Clipboard** and paste into your email client's search field.

### Expected input format

```
Fatura <invoice-number> cliente <client-number>
```

Lines that do not match this format are skipped, and a warning is shown with the line numbers that were ignored.

---

## Project structure

```
FaturaQueryBuilder/
├── MainWindow.xaml          # UI layout
├── MainWindow.xaml.cs       # Logic: processing, validation, clipboard, toast
├── App.xaml / App.xaml.cs   # Application entry point and theme switching
├── Converters/
│   └── Converters.cs        # BoolToVisibility converter
└── Themes/
    ├── LightTheme.xaml      # Light color palette
    └── DarkTheme.xaml       # Dark color palette
```

---

## Tech stack

| | |
|---|---|
| Language | C# 12 |
| Framework | .NET 8.0 |
| UI | WPF (Windows Presentation Foundation) |
| Target OS | Windows |

---

## License

MIT
