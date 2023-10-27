using Org.BouncyCastle;
using System.Text;
using XmlDocMarkdown.Core;

const string baseNamespace = "SecurityDataParsers";
string[] _args = Environment.GetCommandLineArgs();

if (_args.Length < 4) {
	Console.WriteLine( "Usage: DocsGenerator.exe <projectName> <projectUrl> <solutionRoot>" );
	return;
}
int argStart = _args[1].StartsWith( "-" ) ? 1 : 0;
string projName = _args[1 + argStart];
string projUrl = _args[2 + argStart];
string solutionRoot = _args[3 + argStart];

string assemblyPath = Path.Combine( solutionRoot, projName, "dist", $"{projName}.dll" );

string outDir = Path.Combine( solutionRoot, projName, "docs" );
Org.BouncyCastle.Utilities.Date.DateTimeUtilities.CurrentUnixMs();
new DirectoryInfo( outDir ).Delete( true );
Directory.CreateDirectory( outDir );

Console.WriteLine( $"... Generating docs for {projName} to {outDir}" );
XmlDocMarkdownGenerator.Generate( new XmlDocInput() {
	AssemblyPath = assemblyPath,
}, outDir, new XmlDocMarkdownSettings() {
	SourceCodePath = projUrl,
	RootNamespace = $"{baseNamespace}.{projName}",
	SkipUnbrowsable = true,
	VisibilityLevel = XmlDocVisibilityLevel.Public,
	NewLine = "\n",
	ShouldClean = true,
	GenerateToc = true,
	NamespacePages = true,
	PermalinkStyle = "pretty",
} );
Console.WriteLine( $"Generated docs for {projName} to {outDir}" );

Console.WriteLine( "Finding root namespaces to combine" );
FileInfo[] baseFiles = new DirectoryInfo( outDir ).GetFiles();
Console.WriteLine( $"Found {baseFiles.Length} files in {outDir}" );
StringBuilder combinedContent = new();
foreach (FileInfo file in baseFiles) {
	if (file.Name.ToLower().EndsWith( ".md" ) && file.Name.ToLower().StartsWith( baseNamespace.ToLower() )) {
		combinedContent = combinedContent.AppendLine( File.ReadAllText( file.FullName ) );
	}
}
File.WriteAllText( Path.Combine( outDir, "README.md" ), combinedContent.ToString() );
Console.WriteLine( $"Combined {baseFiles.Length} files into README.md" );
