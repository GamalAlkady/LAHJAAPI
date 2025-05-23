using AutoGenerator.Code.Services;
using AutoGenerator.Code.VM;
using AutoGenerator.Code.VM.v1;
using AutoGenerator.Custom.Data;
using AutoGenerator.TM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoGenerator.Code.Controllers
{
    [Route("api/Admin/Code/[controller]")]
    [ApiController]
    public class CodeValidatorController : ControllerBase
    {
        private static InMemoryCodeRepository _repository = new InMemoryCodeRepository(BaseGenerator.TGenerators, InMemoryCodeRepository.PathModels);
        private static AdvGeminiCodeService aiService = new AdvGeminiCodeService("AIzaSyC2jbzfAtMHvXLLqEXiSmATpFDCqa_CGZ4");

       

      
 
public class ModelInput
    {
        public string model_name { get; set; }
        public string model_structure { get; set; }
        public string template_instructions { get; set; }
    }

    public class ModelOutput
    {
        public string result { get; set; }
    }

    public static class ApiClient
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<ModelOutput> SendRequestAsync(ModelInput input)
        {
            string url = "https://modelspeech.onrender.com/predict";

            var json = JsonSerializer.Serialize(input);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Request failed with status code {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            var output = JsonSerializer.Deserialize<ModelOutput>(responseContent);
            return output;
        }
}

      

    

        private ObjectResult CreateProblem(string title, int statusCode, string? detail = null, IDictionary<string, object?>? extensions = null)
        {
            var problem = new ExtendedProblemDetails
            {
                Title = title,
                Status = statusCode,
                Detail = detail ?? title,
                Extensions = extensions ?? new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            };
            return new ObjectResult(problem) { StatusCode = statusCode };
        }

    

        [HttpPost("GenerateAllCode")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(ExtendedProblemDetails), 500)]
        public async Task<ActionResult<string>> GenerateAllCode([FromBody] TextGenerationRequest request)
        {
            var description = request?.Description ?? "Generate a validator for the model structure. all state";
            var smodels = _repository.GetSModels();

            var codes = _repository.GetCodes<ValidatorGenerator>();
            
            var result = await GenerateValidatorsForAllContextsAsync2(description,smodels);


            if (string.IsNullOrWhiteSpace(result))
            {
                return CreateProblem("No validators were successfully generated or validated.", 500);
            }
            description = "You must use the checker to access any condition in another model. The process must be professionally adjusted.";
            var finalres = await GenerateValidatorsForAllContextsAsync2(description, smodels, result);
            if (string.IsNullOrWhiteSpace(finalres))
            {
                return CreateProblem("No validators finalres were successfully generated or validated.", 500);
            }
            return Ok(finalres);
        }
        struct ProcessResult
        {
            public ValidationResult Result;
            public CodeGeneratorM OriginalContext;
            public string? GeneratedCode;
            public Exception? Error;
        }
      
        private async Task<string> GenerateValidatorsForAllContextsAsync(string description, string sourceModels, string? templateCode = null)
        {

            var validatorContexts = _repository.GetCodes<ValidatorGenerator>()
                .Where(x => x.Name.Contains("Validator") && x.Code.Contains("ValidatorContext")&&!x.Code.Contains("ValidatorContext<TContext,EValidator>"))
                .ToList();

           // var aiService = new AdvGeminiCodeService("AIzaSyAIj-GIZFsePalugpnpm26s4MjdvPf7cJU");

            var output = new StringBuilder();

            foreach (var context in validatorContexts)
            {
                try
                {
                    var modelIdentifier = context.TypeModel.Name ?? context.Code;

                    if (string.IsNullOrWhiteSpace(modelIdentifier))
                    {
                        Console.WriteLine($"Skipped context '{context.Name}' - No model identifier found.");
                        continue;
                    }



                    var input = new ModelInput
                    {
                        model_name = modelIdentifier,
                        model_structure = $"{description} -- models: {sourceModels}",
                        template_instructions = templateCode ?? context.Code
                    };

                
                    var result = await ApiClient.SendRequestAsync(input);



                    string generatedCode = result.result;

                    var validationResult = CodeSaveValidator.ValidationCode(generatedCode);
                    validationResult.Code = generatedCode;

                    if (validationResult.IsSuccess)
                    {
                        context.Code = generatedCode;

                        _repository.AddOrUpdate(context);
                    

                        output.AppendLine(generatedCode);
                        output.AppendLine("============================================");
                    }
                    else
                    {
                        Console.WriteLine($"Validation failed for '{context.Name}': {validationResult.ErrorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing context '{context.Name}': {ex.Message}");
                }
            }

            return output.ToString();
        }




        private async Task<string> GenerateValidatorsForAllContextsAsync2(string description, string sourceModels, string? templateCode = null)
        {
            var validatorContexts = _repository.GetCodes<ValidatorGenerator>()
                .Where(x => x.Name.Contains("Validator") && x.Code.Contains("ValidatorContext")&& !x.PathFile.Contains("ValidatorContext.cs"))
                .ToList();

            var output = new StringBuilder();
            var tasks = new List<Task>();

            var outputLock = new object(); // لتفادي تعارض الكتابة على StringBuilder

            foreach (var context in validatorContexts)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var modelIdentifier = context.TypeModel.Name ?? context.Code;

                        if (string.IsNullOrWhiteSpace(modelIdentifier))
                        {
                            Console.WriteLine($"Skipped context '{context.Name}' - No model identifier found.");
                            return;
                        }

                        var input = new ModelInput
                        {
                            model_name = modelIdentifier,
                            model_structure = $"{description} -- models: {sourceModels}",
                            template_instructions = templateCode ?? context.Code
                        };

                        var result = await ApiClient.SendRequestAsync(input);
                        string generatedCode = result.result;

                        var validationResult = CodeSaveValidator.ValidationCode(generatedCode);
                        validationResult.Code = generatedCode;

                        if (validationResult.IsSuccess)
                        {
                            context.Code = generatedCode;
                            _repository.AddOrUpdate(context);

                            lock (outputLock)
                            {
                                output.AppendLine(generatedCode);
                                output.AppendLine("============================================");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Validation failed for '{context.Name}': {validationResult.ErrorMessage}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing context '{context.Name}': {ex.Message}");
                    }
                }));
            }

            await Task.WhenAll(tasks);
            return output.ToString();
        }

        [HttpPost("SaveChanges")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExtendedProblemDetails), 500)]
        public  IActionResult SaveChanges()
        {
            try
            {
                _repository.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving changes: {ex.Message}");
                return CreateProblem("Failed to save changes.", 500, $"An internal error occurred while saving: {ex.Message}");
            }
        }


        [HttpGet("GetPromptBefore")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExtendedProblemDetails), 500)]

        public IActionResult GetPromptBefore(string  des="")
        {

            var validatorContexts = _repository.GetCodes<ValidatorGenerator>()
               .Where(x => x.Name.Contains("Validator") && x.Code.Contains("ValidatorContext") && !x.Code.Contains("ValidatorContext<TContext,EValidator>"))
               .ToList();
            var sp = new List<string>();
            var tp = new StringBuilder();

            foreach (var validatorContext in validatorContexts) {
            
               sp.Add(validatorContext.TypeModel.Name);
               tp.AppendLine(validatorContext.Code);
               tp.AppendLine("========================");
              
            }
            var description = des+"  Generate a validator for the model structure. all state";
            var smodels = _repository.GetSModels();

            var generator = new ValidatorPromptGenerator(tp.ToString(), TmValidators.GetTmRoleVoidator());
            var prompts = generator.GenerateValidatorPromptsForModels(sp,smodels, description);


            return Ok(prompts);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            

        }



        private static string GenerateValidatorPrompt(string modelName, string modelStructure, string templateInstructions)
        {
            var prompt = $@"
Generate a C# Validator class for the model '{modelName}' with the following structure:
Model Name: {modelName}
Model Structure (C# Class Definition): {modelStructure}
Validator Template Pattern Requirements:
- The class must be named '{modelName}ValidatorContext'.
- It must inherit from 'ValidatorContext<{modelName}, {modelName}ValidatorStates>'.
- It must implement 'ITValidator'.
- It needs a constructor taking 'IConditionChecker checker'.
- Generate a public enum named '{modelName}ValidatorStates'. This enum should contain a state entry for EACH PUBLIC PROPERTY in the model structure (e.g., 'HasPropertyName').
- For EACH PUBLIC PROPERTY in the Model Structure, create a corresponding private method to perform validation.
    - The method signature should be 'private Task<ConditionResult> ValidatePropertyName(DataFilter<PropertyType, {modelName}> f)'. Use the actual PropertyType from the Model Structure.
    - Use 'async' only if 'await' is used inside the function body.
    - If the function uses 'async', the return reference must be either ConditionResult.ToFailureAsync or ToSuccessAsync.
    - If the function does NOT use 'async', the return reference must be ConditionResult.ToFailure or ToSuccess (use Task.FromResult(...) where necessary if not async).
    - Each validation method must use the '[RegisterConditionValidator(typeof({modelName}ValidatorStates), {modelName}ValidatorStates.HasPropertyName, ""Error message"")]' attribute.
- Implement the 'GetModel' protected async method, ensuring it correctly handles caching if needed, similar to the provided template example.
- dont remove any usespaces in code
- If you use the res.Success condition and it is of type bool? the condition must be as follows (res.Success == true)
- Apply the following specific validation rules based on property types, as detailed in Template Instructions.

Reference the pattern shown in the architecture below:
--- START C# VALIDATOR TEMPLATE ARCHITECTURE REFERENCE ---
{TmValidators.GetTmRoleVoidator()}
--- END C# VALIDATOR TEMPLATE ARCHITECTURE REFERENCE ---

Your response MUST be ONLY the raw C# code for the '{modelName}ValidatorContext' class, including the enum, using statements (if necessary based on common patterns like AutoGenerator.Conditions), constructors, methods, and attributes. Do NOT include any surrounding text, explanations, or markdown. Ensure the output is just the C# code block.

Example Validation Logic Based on Common Types (Guideline for AI):
- string properties: Check for null or whitespace.
- string properties (potential URLs): Use Uri.TryCreate.
- bool properties: Often just check if the property exists or is default.
- Collection properties (like ICollection<Item>): Check if the collection is null or empty if required, or validate each item within the collection based on specific instructions.

Ensure all generated code adheres to the specified template pattern and the Template Instructions provided below.
Template Instructions:
{templateInstructions}
";
            return prompt;
        }

    }



public class ValidatorPromptGenerator
    {
        private readonly string _templateInstructions;
        private readonly string _csharpTemplateString;

        public ValidatorPromptGenerator(string templateInstructions, string csharpTemplateString)
        {
            _templateInstructions = templateInstructions;
            _csharpTemplateString = csharpTemplateString;
        }

        public string GenerateValidatorPromptsForModels(List<string> modelNames, string allModelClassesCode, string des)
        {
            var finalPrompt = new StringBuilder();

            foreach (var className in modelNames)
            {
                // Optional: تحقق أن الموديل موجود في النص
             

                string prompt = $@"
//-------------//
// {className}ValidatorContext.cs
//

You will receive a single C# model. Generate a C# Validator class that follows the exact architecture pattern below without adding any comments inside the generated code.

--- START C# VALIDATOR TEMPLATE ARCHITECTURE REFERENCE ---
{_csharpTemplateString}
--- END C# VALIDATOR TEMPLATE ARCHITECTURE REFERENCE ---

Validator Template Pattern Requirements:
- The class must be named '{className}ValidatorContext'.
- It must inherit from 'ValidatorContext<{className}, {className}ValidatorStates>'.
- It must implement 'ITValidator'.
- It needs a constructor taking 'IConditionChecker checker'.
- Generate a public enum named '{className}ValidatorStates'. This enum should contain a state entry for EACH PUBLIC PROPERTY in the model structure (e.g., 'HasPropertyName').
- For EACH PUBLIC PROPERTY in the Model Structure, create a corresponding private method to perform validation.
    - The method signature should be 'private Task<ConditionResult> ValidatePropertyName(DataFilter<PropertyType, {className}> f)'.
    - Use 'async' only if 'await' is used inside the function body.
    - If the function uses 'async', the return reference must be either ConditionResult.ToFailureAsync or ToSuccessAsync.
    - If the function does NOT use 'async', the return reference must be ConditionResult.ToFailure or ToSuccess (use Task.FromResult(...) where necessary if not async).
    - Each validation method must use the '[RegisterConditionValidator(typeof({className}ValidatorStates), {className}ValidatorStates.HasPropertyName, ""Error message"")]' attribute.
- Implement the 'GetModel' protected async method, ensuring it correctly handles caching if needed.
- Do not include any comments in the code.
- Do not remove any 'using' statements in the code.
- If you use the res.Success condition and it is of type bool? the condition must be (res.Success == true).
- Apply the following specific validation rules based on property types, as detailed in Template Instructions.

Template Instructions:
{_templateInstructions}

Validation Rules:
{des}

Model Name: {className}
Model Structure (C# Class Definition):
{allModelClassesCode}

Your response MUST be ONLY the raw C# code for the '{className}ValidatorContext' class. No explanations, no markdown, and absolutely no comments inside the code.
";

                finalPrompt.AppendLine(prompt);
                finalPrompt.AppendLine();
            }

            return finalPrompt.ToString();
        }
    }

}