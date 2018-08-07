# XDocumentor

.NET Core Markdown generator for C# projects. The initial data for generation are the .dll files (compiled project) and .xml (xml comments in the project compiled by Visual Studio).

Based on [MarkdownGenerator](https://github.com/neuecc/MarkdownGenerator) by [neuecc](https://github.com/neuecc).

Для организации документации вашего software рекомендуется использовать extremly light and easy to use flat file CMS on .NET [Documentor](https://github.com/askalione/documentor).

## Example

To generate Markdown put .dll and .xml project files in the same directory with XDocumentor.exe and use command:

```csharp
XDocumentor.exe MyProject.dll markdown 0 0
```

After execution, a directory markdown with  Markdown files will be created in the same directory.

## Arguments

| Name | Description | 
| --- | --- | 
| Target (required) | Name of target .dll. | 
| Output (optional) | Output directory for generated Markdown files. | 
| AccessibilityLevel (optional) | Which members was generated. 0 - All, 1 - Public only. | 
| Mode (optional) | Generation mode. 0 - Generate file per namespace, 1- Generate file per class. |
| NamespaceMatch | Regex pattern to select namespaces. |

## License

XDocumentor is Copyright © 2018  [Alexey Drapash](https://github.com/askalione), [Creacode](http://creacode.ru)  under the  [MIT license](https://github.com/askalione/xdocumentor/blob/master/LICENSE).
