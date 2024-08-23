using CompanyEmployees.Presentation.ActionFilters;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using NLog;
using UltimateASPNET.Extensions;

namespace UltimateASPNET
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/NLog.config"));

            builder.Services.ConfigureCors();
            builder.Services.ConfigureIISIntegration();
            builder.Services.ConfigureLoggerService();
            builder.Services.ConfigureRepositoryManager();
            builder.Services.ConfigureServiceManager();
            builder.Services.ConfigureSqlContext(builder.Configuration);

            builder.Services.AddAutoMapper(typeof(Program));

            //��������� ��������� ��������� �������, ������� ������ ������� [ApiController]
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddScoped<ValidationFilterAttribute>();

            builder.Services.AddControllers(config =>
            {
                config.RespectBrowserAcceptHeader = true;
                config.ReturnHttpNotAcceptable = true;
                //������� ���������� NewtonsoftJsonPatchInputFormatter � ������ ������ �����������
                //������ �� ��������� JSON Patch ��������, �� �� ����������� ���������� System.Text.Json ��� ������� JSON ��������.
                //���� ��������� ����������� � ������, ����� �������������, ��� �� ����� �������������� ��� ��������� ��������
                //� ����� �������� application/json-patch+json.
               config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
            }).AddXmlDataContractSerializerFormatters()
            .AddCustomCSVFormatter()
            .AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);

            var app = builder.Build();

            app.ConfigureExceptionHandler(app.Services.GetRequiredService<ILoggerManager>());

            if (app.Environment.IsProduction())
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseForwardedHeaders(new ForwardedHeadersOptions()
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();

            //��������� ������� - ������� ��������� ���������, ��������� ������� ��������� ������������ � MVC
            //� ���������� JSON, ��������� Newtonsoft.Json. �������� ��������� ��������. 
            //�� ���������� �������� ��������� ����� MvcOptions, ����� ������� ��������� ��������� InputFormatters.
            //���������� ������ ��������� ���� NewtonsoftJsonPatchInputFormatter �� ���� ���������.
            NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
                new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
                    .Services.BuildServiceProvider()
                    .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
                    .OfType<NewtonsoftJsonPatchInputFormatter>().First();
        }
    }
}