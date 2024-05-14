當然，這是 markdown 格式的 Readme.md 文件內容，您可以直接將其複製並粘貼到您的 readme.md 文件中：

````markdown
# WpfToolkit

This WpfToolkit project provides a custom toolkit for WPF applications, containing some useful controls and classes to help developers build WPF applications more easily.

## Features

- **ItemSourceProvider<T>**: This class provides management and notification functionality for data sources.
- **TDataGrid<T>**: This class is a custom DataGrid control for displaying data.

## Usage

1. Reference WpfToolkit in your WPF project.
2. Create an instance of ItemSourceProvider<T> and initialize the data.
3. Use ItemSourceProvider<T> to initialize the TDataGrid control.
4. Add, update, or delete data items through ItemSourceProvider<T>.

### Example

```csharp
// Create an instance of ItemSourceProvider<T> and initialize the data
var mDataItemProvider = new ItemSourceProvider<DataItem>(new Dictionary<string, DataItem>
{
    { "1", new DataItem { Tone = "Tone1", Rx = 1, Ry = 1, Rz = 1 } },
    { "2", new DataItem { Tone = "Tone2", Rx = 2, Ry = 2, Rz = 2 } }
});

// Create a TDataGrid control and initialize it
var mCustomDataGrid = new TDataGrid<DataItem>(mDataItemProvider);

// Add the TDataGrid control to the UI
UserControlContent = mCustomDataGrid;
```

## Contributions

Contributions of code, issues, and suggestions are welcome. You can contribute by submitting pull requests or issues.

## License

This WpfToolkit project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more information.
````