using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace ChatApp.Core.Application.SKPlugins
{
    public class MessagePlugin
    {
        private readonly Kernel _kernel;

        public MessagePlugin(Kernel kernel)
        {
            _kernel = kernel;
        }

        [KernelFunction]
        [Description("Paraphrases the input text into a different form while preserving the original meaning.")]
        public async Task<string> ParaphraseMessage(
            [Description("The text to paraphrase")] string input,
            [Description("The style of paraphrasing, e.g., 'formal', 'standard', 'simplified'")] string style = "standard")
        {
            var prompt = @$"SYSTEM: You are an expert writer specializing in paraphrasing text while maintaining the original meaning.

                            USER: I need you to paraphrase the following text in a {style} style. 
                            Please only return the paraphrased text without any additional comments or explanations.

                            Original text: {input}

                            ASSISTANT:";

            return await InvokePromptAsync(prompt);
        }

        [KernelFunction]
        [Description("Checks and corrects grammar in the input text while preserving the original meaning.")]
        public async Task<string> CheckGrammar([Description("The text to check for grammar errors")] string input)
        {
            var prompt = @$"SYSTEM: You are an expert editor specializing in grammar correction while maintaining the original meaning.
                    USER: I need you to check and correct any grammar, spelling, or punctuation errors in the following text. 
                    Please only return the corrected text without any additional comments or explanations.
                    If the text is already grammatically correct, return it unchanged.
                    Original text: {input}
                    ASSISTANT:";

            return await InvokePromptAsync(prompt);
        }

        private async Task<string> InvokePromptAsync(string prompt)
        {
            var result = await _kernel.InvokePromptAsync(prompt);
            var response = result.GetValue<string>() ?? string.Empty;

            if (response.StartsWith("ASSISTANT:", StringComparison.OrdinalIgnoreCase))
                response = response.Substring("ASSISTANT:".Length).Trim();

            return response;
        }
    }
}
