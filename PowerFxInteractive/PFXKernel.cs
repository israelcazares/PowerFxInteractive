using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.PowerFx;


namespace PowerFxInteractive
{
    internal class PFXKernel : Kernel, IKernelCommandHandler<SubmitCode>
    {
        private static RecalcEngine myPFXEngine;
        public PFXKernel(RecalcEngine engine) : base("powerfx")
        {
            myPFXEngine = engine;
        }

        public Task HandleAsync(SubmitCode exp, KernelInvocationContext invocationContext)
        {
            var getResult = new PFXInteractive(myPFXEngine, exp.Code).ParseFx();
            var resultObject = getResult.Result.Select(c => $"<tr><td>{c.formula.Replace(";","")}</td><td>{c.result}</td></tr>");
            var tableResult = $"<table><style>table {{ width: 50vw;}}</style><tr><th>Expression</th><th>Result</th></tr>{string.Join("",resultObject)}</table>";

            invocationContext.DisplayAs(tableResult, "text/markdown");

            return Task.CompletedTask;
        }
    }
}
