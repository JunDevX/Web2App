# Web2App (W2A)

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white) ![DevKit](https://img.shields.io/badge/DevKit-0078D4?style=for-the-badge&logo=visual-studio&logoColor=white)

[Russian](../README.md) | English | [Help with Translation](./readme-help.md)

Convert any website into a full-featured desktop application.

## Description

Web2App is a utility written in C# using WPF and WebView2 that allows you to package any website or web application as a separate desktop application for Windows. Each created application runs in an isolated environment with its own configuration and icon.

## Features

- Create desktop applications from web addresses
- Support for custom icons (ICO and PNG formats)
- Automatic PNG to ICO format conversion
- Desktop shortcut creation
- User-Agent configuration for each application
- Isolated directory for each application
- Configuration saved in JSON format

## Requirements

- Windows 7 or higher
- .NET 8.0 (Windows)
- Microsoft Edge WebView2 Runtime

## Installation

### From Source Code

1. Clone the repository:
```bash
git clone https://github.com/JunDevX/Web2App.git
cd Web2App
```

2. Open the project in Visual Studio:
```bash
start Web2AppLauncher.sln
```

3. Restore NuGet dependencies:
```bash
dotnet restore
```

4. Build the project:
```bash
dotnet build
```

5. Run the application:
```bash
dotnet run
```

## Usage

### Application Generator

1. Run Web2AppLauncher.exe
2. Enter the website URL
3. (Optional) Specify the application name
4. (Optional) Select an icon for the application
5. (Optional) Set a custom User-Agent
6. Click "Create"

The application will be created and added to the Desktop.

### Created Application Structure

Each created application is stored in:
```
%AppData%\Web2Apps\{application_name}\
├── config.json      # Application configuration
└── app.ico          # Application icon
```

## Project Structure

```
Web2App/
├── App.xaml           # Application entry point
├── App.xaml.cs        # Initialization logic
├── MainWindow.xaml    # Application generator UI
├── MainWindow.xaml.cs # Application creation logic
├── AppWindow.xaml     # Web application UI
├── AppWindow.xaml.cs  # Web application logic
├── AssemblyInfo.cs    # Assembly information
└── Web2AppLauncher.csproj # Project configuration
```

## Dependencies

- **Microsoft.Web.WebView2** (v1.0.2592.51) - for displaying web content
- **System.Drawing.Common** (v10.0.10) - for working with images and icons

## Implementation Details

### Application Isolation

Each created application works in a separate directory with its own configuration, allowing multiple applications to run simultaneously without conflicts.

### Icon Conversion

PNG icons are automatically converted to ICO format for correct display on the Windows desktop.

### Argument Passing

When launched via a Desktop shortcut, the application receives the path to the configuration directory through command line arguments, allowing it to load the correct URL and other settings.

## Development

### Build for Release

```bash
dotnet publish -c Release -o ./publish
```

### Run Tests

```bash
dotnet test
```

## Possible Improvements

- Support for additional operating systems (macOS, Linux)
- Saving application state between launches
- Data synchronization between web and application versions
- Content caching for offline mode
- Support for extensions and plugins

## Troubleshooting

### Application won't start

- Ensure Microsoft Edge WebView2 Runtime is installed
- Check for the presence of config.json in the application directory
- Verify the correct JSON format in config.json

### Icon not displaying

- Check the icon file format (ICO or PNG)
- Ensure the file is not corrupted
- Try using an icon with a size of at least 256x256 pixels

## License

This project is distributed under the Apache License 2.0. See the LICENSE file for details.

## Author

JunDevX

## Contributing

We welcome contributions to the project. Please create Pull Requests with a description of the proposed changes.

---

**Made with ❤️ for the developer community**
