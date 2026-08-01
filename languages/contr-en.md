# Contributing Guidelines

[Russian](../CONTRIBUTING.md) | English

Thank you for your interest in Web2App! We welcome contributions from all community members.

## How to Contribute

### Reporting Bugs

If you found a bug, please open a new Issue with a description:

1. **Issue Title** - briefly describe the problem
2. **Version** - specify the application version and OS
3. **Steps to Reproduce** - step-by-step instructions
4. **Expected Behavior** - what should have happened
5. **Actual Behavior** - what actually happened
6. **Screenshots** - if possible, add screenshots

Example:

```
Title: Error when converting PNG larger than 2000px

Version: 0.1.0, Windows 10

Steps to Reproduce:
1. Open Web2App
2. Load a PNG icon sized 3000x3000px
3. Click "Create"

Expected: Icon successfully converts
Actual: OutOfMemoryException appears
```

### Suggesting Features

Have an idea for improvement? Open an Issue with the **[FEATURE]** prefix:

1. Describe the problem this solves
2. Propose a solution
3. List alternative solutions (if any)
4. Provide additional context

### Submitting a Pull Request

1. **Fork** the repository
2. Create a branch for the feature:
   ```bash
   git checkout -b feature/description-of-feature
   ```
3. Make changes and add tests
4. Commit with a clear message:
   ```bash
   git commit -m "feat: Add new feature description"
   ```
5. Push to your branch:
   ```bash
   git push origin feature/description-of-feature
   ```
6. Open a Pull Request with a description of your changes

## Development Recommendations

### Code Style

The project uses C# with Microsoft coding standards:

- **Classes and Methods** - PascalCase
  ```csharp
  public class MainWindow
  {
      public void CreateApplication() { }
  }
  ```

- **Variables and Fields** - camelCase (private) or _camelCase (fields)
  ```csharp
  private string _selectedIconPath;
  private void ProcessInput(string userInput) { }
  ```

- **Constants** - UPPER_CASE
  ```csharp
  private const string CONFIG_FILE_NAME = "config.json";
  ```

- **Indentation** - 4 spaces
- **Braces** - Allman style
  ```csharp
  if (condition)
  {
      DoSomething();
  }
  ```

### Comments

- Document complex logic
- Use XML comments for public members:
  ```csharp
  /// <summary>
  /// Converts PNG image to ICO format.
  /// </summary>
  /// <param name="pngPath">Path to PNG file</param>
  /// <param name="icoPath">Path to save ICO file</param>
  private void ConvertPngToIco(string pngPath, string icoPath)
  {
  }
  ```

### Commit Structure

Use clear commit messages:

- `feat:` - new feature
- `fix:` - bug fix
- `docs:` - documentation changes
- `refactor:` - code refactoring
- `test:` - test additions or changes
- `chore:` - dependency updates, configuration

Examples:
```bash
git commit -m "feat: Add support for custom window size"
git commit -m "fix: Handle empty URL validation"
git commit -m "docs: Update installation instructions"
git commit -m "refactor: Simplify icon conversion logic"
```

### Testing

Before submitting a Pull Request:

1. Test locally:
   ```bash
   dotnet build
   dotnet run
   ```

2. Check that it compiles without errors
3. Ensure the application runs without exceptions
4. Test the new feature in different scenarios

### Pull Request Requirements

- Description of what you're doing
- Link to related Issue (if applicable)
- Screenshots for UI changes
- Code follows guidelines
- All tests pass

## Review Process

1. At least one maintainer will review your PR
2. Changes may be requested
3. After approval, the PR will be merged
4. Your name will be added to CHANGELOG

## License Terms

By submitting a Pull Request, you agree to license your code under the Apache License 2.0.

## Code of Conduct

We expect from all participants:

- Respect for other developers
- Constructive criticism
- No spam or insults
- Openness to different perspectives

## Questions?

If you have questions:

1. Check existing Issues
2. Review README and documentation
3. Open a new Issue with your question

Thank you for your contribution to Web2App!
