using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Formatting;
using Microsoft.DotNet.Interactive.Commands;
using System.Linq;
using Microsoft.PowerFx;
using System.Globalization;

namespace PowerFxInteractive;
public class PFXKernelExtension : IKernelExtension
{
    private static RecalcEngine myPFXEngine;
    public string Name => "PowerFx";
    public async Task OnLoadAsync(Kernel kernel)
    {
        if (kernel is CompositeKernel compositeKernel)
        {
            var config = new PowerFxConfig(new CultureInfo("en-US"));
            myPFXEngine = new RecalcEngine(config);
            compositeKernel.Add(new PFXKernel(myPFXEngine));
        }

        var message = new HtmlString($@"
        <details>
            <summary>Power FX Core Version: {typeof(RecalcEngine).Assembly.GetName().Version}</summary>
            <p>This extension adds a new kernel that can render Power FX Core Commands.</p>
            <a href=""https://learn.microsoft.com/en-us/power-platform/power-fx/formula-reference"" target=""_blank"">Power Fx formula reference for Power Apps</a>
            <p>These are the supported Power Fx Core functions:</p>
            <p>{String.Join("\r\n | ", myPFXEngine.GetAllFunctionNames().Select(x => $@"<a href=""https://docs.microsoft.com/en-us/search/?terms={x}&scope=Power%20Apps"" >{x}</a>"))}</p>            
            
        </details>");

        var formattedValue = new FormattedValue(
            HtmlFormatter.MimeType,
            message.ToDisplayString(HtmlFormatter.MimeType));

        await kernel.SendAsync(new DisplayValue(formattedValue, Guid.NewGuid().ToString()));

    }
}