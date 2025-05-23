using AutoGenerator.Helper.Translation;
using LAHJAAPI.Data;
using LAHJAAPI.Services2;
using LAHJAAPI.V1.Validators;
using V1.DyModels.Dso.Requests;
using V1.DyModels.Dto.Build.Requests;
using V1.Services.Services;

namespace LAHJAAPI.Seeds
{
    public static class DefaultModals
    {
        public static async Task SeedAsync(IServiceScope scope, DataContext context)
        {
            //await context.ModelAis.ExecuteDeleteAsync();
            //await context.ModelGateways.ExecuteDeleteAsync();
            var modelGatewayService = scope.ServiceProvider.GetService<IUseModelGatewayService>();

            //if (await context.ModelGateways.FirstOrDefaultAsync(p => p.Name == "web") == null)
            if (!await modelGatewayService.ExistsAsync("Core", "Name"))
            {
                var Token = TokenService.GenerateSecureToken();

                await modelGatewayService.CreateAsync(new ModelGatewayRequestDso
                {
                    Name = "Core",
                    Url = "https://modelspeech.onrender.com",
                    Token = Token,
                    IsDefault = true,
                    ModelAis = new List<ModelAiRequestBuildDto>
                    {
                        new ModelAiRequestBuildDto
                        {
                            Name = "Wasm Speeker",
                            AbsolutePath = "wasm-speeker",
                            Token = TokenService.GenerateSecureToken(),
                            IsStandard = true,
                            Category = new TranslationData
                            {
                                Value = new Dictionary<string, string>
                                {
                                    { "ar", "تحويل النص إلى لهجة" },
                                    { "en", "Text-to-Dialect" }
                                }
                            },
                            Dialect = new TranslationData
                            {
                                Value = new Dictionary<string, string>
                                {
                                    { "ar", "العامية" },
                                    { "en", "Dialect" }
                                }
                            },
                            Language = "en",
                            Gender="Male",
                            Type = "Text-to-Speech",
                            Services = new List<ServiceRequestBuildDto>
                            {
                                new ServiceRequestBuildDto
                                {
                                    Name = "wasm-speeker",
                                    AbsolutePath = "wasm-speeker",
                                    Token = TokenService.GenerateSecureToken(),
                                }
                            }

                        },
                        new()
                        {
                            Name = "General Services",
                            AbsolutePath = "general-services",
                            Token = TokenService.GenerateSecureToken(),
                            IsStandard = true,
                            Category = new TranslationData
                            {
                                Value = new Dictionary<string, string>
                                {
                                    { "ar", "خدمات عامة" },
                                    { "en", "General Services" }
                                }
                            },
                            Dialect = new TranslationData
                            {
                                Value = new Dictionary<string, string>
                                {
                                    { "ar", "العامية" },
                                    { "en", "Dialect" }
                                }
                            },
                            Language = "en",
                            Gender="en",
                            Type = "Text-to-Speech",
                            Services = new List<ServiceRequestBuildDto>
                            {
                                new ()
                                {
                                    Name = "Create Space",
                                    AbsolutePath = ServiceType.CreateSpace,
                                    Token = TokenService.GenerateSecureToken(),
                                },
                                new()
                                {
                                    Name = "V1board",
                                    AbsolutePath = ServiceType.Dashboard,
                                    Token = TokenService.GenerateSecureToken(),
                                }
                            }
                        }
                    }
                });
                //await context.ModelGateways.AddRangeAsync([new ModelGateway
                //{
                //    Name = "Core",
                //    Url = "https://modelspeech.onrender.com",
                //    Token = "https://modelspeech.onrender.com",
                //    IsDefault = true,

                //},
                //    new ModelGateway
                //{
                //    Name = "huggingface",
                //    Url = "https://huggingface.co/wasmV1ai",
                //    Token = Token,
                //    IsDefault = false
                //}
                //]);


                //await context.ModelAis.AddAsync(new ModelAi
                //{
                //    Name = "Wasm Speeker",
                //    AbsolutePath = "wasm-speeker",
                //    Token = Token,
                //});


                //await context.SaveChangesAsync();
            }



        }

    }
}