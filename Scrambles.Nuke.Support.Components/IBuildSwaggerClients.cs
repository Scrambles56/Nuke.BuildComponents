using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.Docker;
using Scrambles.Nuke.Support.Components.Have;
using Scrambles.Nuke.Support.Components.Support;

namespace Scrambles.Nuke.Support.Components;

public enum SwaggerLanguage
{
    [SwaggerLangCode("dart")] Dart,
    [SwaggerLangCode("aspnetcore")] AspNetCore,
    [SwaggerLangCode("csharp")] CSharp,
    [SwaggerLangCode("csharp-dotnet2")] CSharpDotNet2,
    [SwaggerLangCode("go")] Go,
    [SwaggerLangCode("go-server")] GoServer,
    [SwaggerLangCode("dynamic-html")] DynamicHTML,
    [SwaggerLangCode("html")] HTML,
    [SwaggerLangCode("html2")] HTML2,
    [SwaggerLangCode("java")] Java,
    [SwaggerLangCode("jaxrs-cxf-client")] JaxrsCxfClient,
    [SwaggerLangCode("jaxrs-cxf")] JaxrsCxf,
    [SwaggerLangCode("inflector")] Inflector,
    [SwaggerLangCode("jaxrs-cxf-cdi")] JaxrsCxfCdi,
    [SwaggerLangCode("jaxrs-spec")] JaxrsSpec,
    [SwaggerLangCode("jaxrs-jersey")] JaxrsJersey,
    [SwaggerLangCode("jaxrs-di")] JaxrsDi,
    [SwaggerLangCode("jaxrs-resteasy-eap")] JaxrsResteasyEap,
    [SwaggerLangCode("jaxrs-resteasy")] JaxrsResteasy,
    [SwaggerLangCode("java-vertx")] JavaVertx,
    [SwaggerLangCode("micronaut")] Micronaut,
    [SwaggerLangCode("spring")] Spring,
    [SwaggerLangCode("nodejs-server")] NodejsServer,
    [SwaggerLangCode("openapi")] Openapi,
    [SwaggerLangCode("openapi-yaml")] OpenapiYaml,
    [SwaggerLangCode("kotlin-client")] KotlinClient,
    [SwaggerLangCode("kotlin-server")] KotlinServer,
    [SwaggerLangCode("php")] PHP,
    [SwaggerLangCode("python")] Python,
    [SwaggerLangCode("python-flask")] PythonFlask,
    [SwaggerLangCode("r")] R,
    [SwaggerLangCode("ruby")] Ruby,
    [SwaggerLangCode("scala")] Scala,
    [SwaggerLangCode("scala-akka-http-server")] ScalaAkkaHttpServer,
    [SwaggerLangCode("swift3")] Swift3,
    [SwaggerLangCode("swift4")] Swift4,
    [SwaggerLangCode("swift5")] Swift5,
    [SwaggerLangCode("typescript-angular")] TypescriptAngular,
    [SwaggerLangCode("typescript-axios")] TypescriptAxios,
    [SwaggerLangCode("typescript-fetch")] TypescriptFetch,
    [SwaggerLangCode("javascript")] JavaScript

}

public record SwaggerClientLibrary(
    SwaggerLanguage Language,
    string ClientFolderName,
    AbsolutePath SwaggerFilePath,
    IEnumerable<string>? AdditionalArguments = null
)
{
    public SwaggerClientLibrary(
        SwaggerLanguage language, 
        string clientFolderName, 
        Project srcProject,
        string srcFramework,
        Configuration configuration,
        string swaggerFileName = "swagger.json",
        IEnumerable<string>? additionalArguments = null)
        :
        this(
            language, 
            clientFolderName, 
            srcProject.Directory / "bin" / configuration / srcFramework / swaggerFileName,
            AdditionalArguments: additionalArguments
        )
    {
    }
};

public interface IBuildSwaggerClients : ICompile, IHaveArtifacts
{
    AbsolutePath ClientsDirectory => ArtifactsDirectory / "clients";
    
    Target BuildSwaggerClients => _ => _
        .DependsOn(Compile)
        .TryDependsOn<ITest>(x => x.Test)
        .Executes(() =>
        {
            Parallel.ForEach(
                SwaggerClientLibraries, 
                new ParallelOptions{ MaxDegreeOfParallelism = BuildSwaggerClientParallelism }, 
                CreateClientLibrary
            );
        });
    
    
    IEnumerable<SwaggerClientLibrary> SwaggerClientLibraries => Enumerable.Empty<SwaggerClientLibrary>();
    int BuildSwaggerClientParallelism => Environment.ProcessorCount;

    private void CreateClientLibrary(SwaggerClientLibrary library)
    {
        var volumeDirectory = AbsolutePath.Create("/local");
        var clientDirectory = ClientsDirectory / library.ClientFolderName;
        Directory.CreateDirectory(clientDirectory);
        
        var args = new List<string>
        {
            "-i", volumeDirectory / RootDirectory.GetRelativePathTo(library.SwaggerFilePath).ToUnixRelativePath(),
            "-l", library.Language.ToSwaggerClientLanguage(),
            "-o", volumeDirectory / RootDirectory.GetRelativePathTo(clientDirectory).ToUnixRelativePath()
        }.Concat(library.AdditionalArguments ?? Enumerable.Empty<string>());

        DockerTasks.DockerRun(_ => _
            .SetImage("swaggerapi/swagger-codegen-cli-v3")
            .SetVolume($"{RootDirectory}:{volumeDirectory}")
            .SetCommand("generate")
            .SetArgs(args)
        );
    }
}

internal static class SwaggerClientLanguageExtensions
{
    public static string ToSwaggerClientLanguage(this SwaggerLanguage language)
    {
        var name = typeof(SwaggerLanguage).GetEnumName(language) ?? throw new EnumValueNotFoundException("Enum value not found");
        var member = typeof(SwaggerLanguage).GetMember(name).FirstOrDefault(m => m.DeclaringType == typeof(SwaggerLanguage));
        var attribute = member?.GetCustomAttributes(typeof(SwaggerLangCodeAttribute), false).FirstOrDefault();

        var typedAttr = attribute as SwaggerLangCodeAttribute ?? throw new AttributeNotFoundException($"Attribute {nameof(SwaggerLangCodeAttribute)} not found on enum value {language}");

        return typedAttr.Language;
    }
}

public class AttributeNotFoundException : Exception
{
    public AttributeNotFoundException(string? message) : base(message)
    {
    }
}

public class EnumValueNotFoundException : Exception
{
    public EnumValueNotFoundException(string? message) : base(message)
    {
    }
}

internal class SwaggerLangCodeAttribute : Attribute
{
    public string Language { get; }

    public SwaggerLangCodeAttribute(string language)
    {
        Language = language ?? throw new ArgumentNullException(nameof(language));
    }
}
