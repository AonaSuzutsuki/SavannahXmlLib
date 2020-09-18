# SavannahXmlLib
This library wraps the .Net Framework standard XML library.
It removes indentation and other unnecessary strings by parsing the XML text independently using a standard library.
The ToString method allows you to generate InnerXml, output unformatted XML in a formatted state.

# Development environment
1. Visual Studio 2019
2. [Microsoft .NET Framework 4.8](https://docs.microsoft.com/dotnet/framework/install/guide-for-developers)

# How to use
## Create a reader from file

```cs
var reader = new CommonXmlReader("file.xml");
// Set false 2nd arg if you want comments.
var reader = new CommonXmlReader("file.xml", false);
```

## Create a reader from Stream

```cs
using var stream = new FileStream("file.xml");
var reader = new CommonXmlReader(stream);
// Set false 2nd arg if you want comments.
var reader = new CommonXmlReader(stream, false);
```

## Get root node

```cs
var root = reader.GetAllNodes();
```

## Get nodes with XPath

```cs
var node = reader.GetNodes("/ServerSettings/property[contains(@name, 'ServerName')]");
```

## Get informations from node

```cs

```