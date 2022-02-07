using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using MonacoRoslynCompletionProvider.Api;
using System;
using System.Threading.Tasks;

namespace MonacoRoslynCompletionProvider
{
    internal class TabCompletionProvider
    {
        // Thanks to https://www.strathweb.com/2018/12/using-roslyn-c-completion-service-programmatically/
        public async Task<TabCompletionResult[]> Provide(Document document, int position)
        {
            int line = 0;
            int count = -1;
            if (document == null)
            {
                count = -2;
            }
            try
            {
                var completionService = CompletionService.GetService(document);
                line++;
                var results = await completionService.GetCompletionsAsync(document, position);
                line++;

                if (results != null)
                {
                    var tabCompletionDTOs = new TabCompletionResult[results.Items.Length];
                    var suggestions = new string[results.Items.Length];
                    line++;

                    count = results.Items.Length;
                    
                    for (int i = 0; i < results.Items.Length; i++)
                    {
                        var itemDescription = await completionService.GetDescriptionAsync(document, results.Items[i]);
                        line++;

                        var dto = new TabCompletionResult();
                        line++;
                        dto.Suggestion = results.Items[i].DisplayText;
                        line++;
                        dto.Description = itemDescription.Text;
                        line++;

                        tabCompletionDTOs[i] = dto;
                        line++;
                        suggestions[i] = results.Items[i].DisplayText;
                        line++;
                    }

                    return tabCompletionDTOs;
                }
                else
                {
                    return Array.Empty<TabCompletionResult>();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"{line} {count} {e.Message}");
            }
        }
    }
}