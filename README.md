# Polyglot Notebooks PowerFx Extension

[![NuGet version (PowerFxInteractive)](https://img.shields.io/nuget/v/PowerFxInteractive.svg)](https://www.nuget.org/packages/PowerFxInteractive/2022.10.29.144)

To get started with PowerFx Expressions in Polyglot Notebooks, first install the `PowerFxInteractive` NuGet package. In a new `C# (.NET Interactive)` cell enter and run the following:

```
#r "nuget: PowerFxInteractive, 2022.10.29.144"
```

Using the `#!powerfx` magic command your code cell will be parsed by a PowerFx engine and the results displayed using the `"txt/markdown"` mime type.

```powerfx
#!powerfx
Set(a,1)
```

For complex Expressions like ClearCollect you will need to use a semicolon at the end of the code.

```powerfx
#!powerfx
ClearCollect(IceCream, Table(
    { Flavor: "Chocolate", Quantity: 100 },
    { Flavor: "Vanilla", Quantity: 200 }
));
```

For regular Expressions there's no need to use a semicolon at the end of the sentence.

```powerfx
#!powerfx
x = Acos(0.5)
Sum(x,5)
```
