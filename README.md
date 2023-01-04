# Polyglot Notebooks PowerFx Extension

[![NuGet version (PowerFxInteractive)](https://img.shields.io/nuget/v/PowerFxInteractive.svg)](https://www.nuget.org/packages/PowerFxInteractive/2022.10.29.144)

To get started with PowerFx Expressions in Polyglot Notebooks, first install the `PowerFxInteractive` NuGet package. In a new `C# (.NET Interactive)` cell enter and run the following:

```
#r "nuget: PowerFxInteractive, 2022.10.29.144"
```

Using the `#!powerfx` magic command your code cell will be parsed by a PowerFx engine and the results displayed using the `"txt/markdown"` mime type.

```powerfx
#!powerfx

Set(a,1);
```
