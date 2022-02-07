using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MonacoRoslynCompletionProvider
{
    public class DefaultMetadataReferences
    {
        public static readonly List<MetadataReference> References = new(20);
        private static HttpClient _client = null;

        public static void InitHttpClient(string basePath)
        {
            _client = new HttpClient() { BaseAddress = new Uri(basePath + "_framework/") };
        }

        public static async Task LoadFromPath(string path)
        {
            var stream = await _client.GetStreamAsync(path);
            References.Add(MetadataReference.CreateFromStream(peStream: stream, filePath: path));
        }
    }
}