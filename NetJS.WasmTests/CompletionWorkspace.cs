using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace MonacoRoslynCompletionProvider
{
    public class CompletionWorkspace
    {
        private Project _project;
        private AdhocWorkspace _workspace;
        private List<MetadataReference> _metadataReferences;

        public static CompletionWorkspace Create(params string[] assemblies) 
        { 
            Assembly[] lst = new[] {
                Assembly.Load("Microsoft.CodeAnalysis.Workspaces"),
                Assembly.Load("Microsoft.CodeAnalysis.CSharp.Workspaces"),
                Assembly.Load("Microsoft.CodeAnalysis.Features"),
                Assembly.Load("Microsoft.CodeAnalysis.CSharp.Features")
            };

            var host = MefHostServices.Create(lst);
            var workspace = new AdhocWorkspace(host);

            var references = DefaultMetadataReferences.References;

            if (assemblies != null && assemblies.Length > 0)
            {
                for (int i = 0; i < assemblies.Length; i++)
                {
                    references.Add(MetadataReference.CreateFromFile(assemblies[i]));
                }
            }

            var projectInfo = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "TempProject", "TempProject", LanguageNames.CSharp)
                .WithMetadataReferences(references);
            var project = workspace.AddProject(projectInfo);


            return new CompletionWorkspace() { _workspace = workspace, _project = project, _metadataReferences = references };
        }

        public async Task<CompletionDocument> CreateDocument(string code)
        {
            var document = _workspace.AddDocument(_project.Id, "MyFile2.cs", SourceText.From(code));
            var st = await document.GetSyntaxTreeAsync();
            var compilation =
            CSharpCompilation
                .Create("Temp",
                    new[] { st },
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                    references: _metadataReferences
                );

            var result = compilation.Emit("temp");
            var semanticModel = compilation.GetSemanticModel(st, true);

            
            return new CompletionDocument(document, semanticModel, result); 
        }
    }
}
