using AutoGenerator;
using AutoGenerator.Notifications.Config;
using AutoMapper;
using LAHJAAPI;
using LAHJAAPI.CustomPolicy;
using LAHJAAPI.Data;
using LAHJAAPI.Middlewares;
using LAHJAAPI.Models;
using LAHJAAPI.Utilities;
using LAHJAAPI.V1.Validators.Conditions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;
using V1.DyModels.Dso.Responses;
using V1.DyModels.Dto.Build.Responses;
using V1.DyModels.Dto.Share.Responses;
using V1.DyModels.VMs;

var builder = WebApplication.CreateBuilder(args);

#region External Services
builder.Services
    .AddStripeGateway(builder.Configuration)
    .AddDataContext(builder.Configuration)
    ;
#endregion

var arrlist = args.ToList();
//arrlist.Add("generate");
//arrlist.Add("/bpr");
builder.Services
       .AddAutoBuilderApiCore<DataContext, ApplicationUser>(new()
       {
           //Arags = arrlist.ToArray(),
           Arags = args,
           NameRootApi = "V1",
           IsMapper = true,
           Assembly = Assembly.GetExecutingAssembly(),
           AssemblyModels = typeof(LAHJAAPI.Models.Advertisement).Assembly,
           ProjectName = "LAHJAAPI",
           PathModels = "G:\\ProgramOfStudy\\VisaulPrograming\\web\\LAHJAAPI\\Models\\Models\\"
       })
    //.AddAutoValidator()
    //.AddAutoConfigScheduler()

    .AddAutoNotifier(new()
    {

        MailConfiguration = new MailConfig()
        {
            SmtpUsername = "gamal333ge@gmail.com",
            SmtpPassword = "bxed hnwv vqlt ddwy",
            SmtpHost = "smtp.gmail.com",
            SmtpPort = 587,
            FromEmail = "gamal333ge@gmail.com",
            NameApp = "LAHJA-API" // عيّن اسم التطبيق هنا كما يناسبك
        }
    });

builder.Services.AddAutoValidator();


try
{

    // بناء الخرائط في وقت التشغيل لتخفيف التحميل وقت الطلب
    var validatorWatch = Stopwatch.StartNew();
    var provider = builder.Services.BuildServiceProvider();
    var mapper = provider.GetRequiredService<IMapper>();
    mapper.Map<ApplicationUserResponseBuildDto>(new ApplicationUser());
    mapper.Map<ApplicationUserResponseShareDto>(new ApplicationUserResponseBuildDto());
    mapper.Map<ApplicationUserResponseDso>(new ApplicationUserResponseShareDto());
    mapper.Map<ApplicationUserOutputVM>(new ApplicationUserResponseDso());

    // تأكد من أن جميع الخرائط صحيحة
    // حاليا معلقة لانها تنتج خطأ سببه التكرار عند بناء الخريطة CreateMap
    //mapper.ConfigurationProvider.AssertConfigurationIsValid(); // ⬅️ يجبر AutoMapper يبني كل الخرائط الآن
    validatorWatch.Stop();
    Console.WriteLine($"✅ Validators registered in: {validatorWatch.ElapsedMilliseconds}ms");
}
catch (AutoMapper.DuplicateTypeMapConfigurationException ex)
{
    foreach (var error in ex.Errors)
    {
        Console.WriteLine($"Conflict for mapping: {error.Types.SourceType.Name} -> {error.Types.DestinationType.Name}");
        Console.WriteLine("Defined in profiles: " + string.Join(", ", error.ProfileNames));
    }
}



// Add services to the container.

builder.Services.AddProblemDetails();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.RespectRequiredConstructorParameters = true;
    });

builder.Services.AddLogging();
builder.Logging.AddConsole().SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddDebug();

// Scoped Services
builder.Services.AddApiServices(builder.Configuration);



builder.Services.AddTransient<IClaimsTransformation, MyClaimsTransformation>();

var appSettings = builder.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

// تمكين عرض الأخطاء المتعلقة بـ JWT
IdentityModelEventSource.ShowPII = true;

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "Keys")))
    .SetApplicationName("LahjaApi");



builder.Services.AddDynamicAuthentication(builder.Configuration, appSettings);

// Configure authorization
builder.Services.AddAuthorizationBuilder();


// Add identity and opt-in to endpoints
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders()
    //.AddUserConfirmation<ApplicationUser>()
    .AddApiEndpoints();

builder.Services.Configure<IdentityOptions>(options =>
{
    /*
     *  يسمح للمستخدمين الجدد بتجربة تسجيل الدخول دون القلق من حظر حساباتهم.
     */
    options.Lockout.AllowedForNewUsers = true;
});
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("CanViewPlan", policy =>
//        policy.Requirements.Add(new PermissionRequirement("Permission", Permissions.ViewPlan))
//        ); // Example: Requires a claim
//});

// Add a CORS policy for the client
builder.Services.AddCors(
    options => options.AddPolicy(
        "wasm",
        policy => policy.WithOrigins(["https://lahja-api.runasp.net", "https://localhost:7001", "https://localhost:5002"])
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSwaggerGen(c =>
{
    //c.EnableAnnotations();
    //c.SchemaFilter<NullDefaultsSchemaFilter>();
    //c.SwaggerDoc("User", new OpenApiInfo { Title = "User API", Version = "v1" });
    //c.SwaggerDoc("V1", new OpenApiInfo { Title = "Public Api", Version = "v1" });
    //c.SwaggerDoc("Admin", new OpenApiInfo { Title = "Admin API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert token with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,

    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer",

            }
            },
            Array.Empty<string>()
        }
    });
});


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    //SeedData.EnsureSeedData(app);
}
//app.UseSchedulerV1board();
app.UseCors("wasm");
app.UseMiddleware<ProblemDetailsMiddleware>();

// Configure the HTTP request pipeline.

//app.Use(async (context, next) =>
//{
//    await next();

//    if (context.Response.StatusCode == 401)
//    {
//        Console.WriteLine($"Unauthorized request. Token: {context.Request.Headers["Authorization"]}");
//    }
//});

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    //o.SwaggerEndpoint("/swagger/User/swagger.json", "User API v1");
    //o.SwaggerEndpoint("/swagger/Admin/swagger.json", "Admin API v1");
    //o.SwaggerEndpoint("/swagger/V1/swagger.json", "My API v1");
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    //o.RoutePrefix = string.Empty;
    // collapse endpoints 
    o.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);

});


app.CustomMapIdentityApi<ApplicationUser>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.UseAutoGeneratorCustomApi(new()
{
    LoginRequest = new()
    {
        Username = "anas",
        Password = "Anas$123",
        ProjectId = "899"

    },
    PathDataContext = "G:\\ProgramOfStudy\\VisaulPrograming\\web\\LAHJAAPI\\LAHJAAPI\\V1\\Data\\",
    PathModels = "G:\\ProgramOfStudy\\VisaulPrograming\\web\\LAHJAAPI\\LAHJAAPI\\V1\\Models\\"
});

// protection from cross-site request forgery (CSRF/XSRF) attacks with empty body
// form can't post anything useful so the body is null, the JSON call can pass
// an empty object {} but doesn't allow cross-site due to CORS.
app.MapPost("/api/logout", async (
    SignInManager<ApplicationUser> signInManager,
    [FromBody] object empty) =>
{
    if (empty is not null)
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    }
    return Results.NotFound();
}).RequireAuthorization().WithTags("Auth");


app.Run();

