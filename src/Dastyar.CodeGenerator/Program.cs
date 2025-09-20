using Arzfy.CodeGenerator;

var kernelDomainDll = @"D:\Projects\arzfy-backend\src\Kernel\Ddd.Kernel.Domain\bin\Debug\net9.0\Ddd.Kernel.Domain.dll";
var domainDll = @"D:\Projects\arzfy-backend\src\Arzfy.Domain\bin\Debug\net9.0\Arzfy.Domain.dll";

var engine = GeneratorEngine.CreateEngine(
    sharedDomainDllPath: kernelDomainDll,
    domainDllPath: domainDll,
    iAggregateRootNamespace: "Ddd.Kernel.Domain.IAggregateRoot");

engine.GenerateTemplates();
