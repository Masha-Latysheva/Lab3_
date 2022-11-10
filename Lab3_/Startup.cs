using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lab3_.Data;
using Lab3_.Middleware;
using Lab3_.Models;
using Lab3_.Services;
using Lab3_.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lab3_
{
    public class Startup
    {
        private static readonly string CacheKey = "Cars20";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // внедрение зависимости для доступа к БД с использованием EF
            var connection = Configuration.GetConnectionString("SqlServerConnection");
            services.AddDbContext<LogisticContext>(options => options.UseSqlServer(connection));
            // внедрение зависимости OperationService
            services.AddTransient<ICarsService, CarsService>();
            // добавление кэширования
            services.AddMemoryCache();
            // добавление поддержки сессии
            services.AddDistributedMemoryCache();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Home/Error");

            // добавляем поддержку статических файлов
            app.UseStaticFiles();

            // добавляем поддержку сессий
            app.UseSession();

            app.UseMiddleware<InfoMiddleware>();

            // добавляем компонент middleware по инициализации базы данных и производим инициализацию базы
            app.UseDbInitializer();

            // добавляем компонент middleware для реализации кэширования и записывем данные в кэш
            app.UseOperatinCache(CacheKey);

            app.MapWhen(ctx => ctx.Request.Path == "/", Index);
            app.MapWhen(ctx => ctx.Request.Path == "/searchform1", CookieSearch);
            app.MapWhen(ctx => ctx.Request.Path == "/search1", CookieSearchHandler);
            app.MapWhen(ctx => ctx.Request.Path == "/searchform2", SessionSearch);
            app.MapWhen(ctx => ctx.Request.Path == "/search2", SessionSearchHandler);

            app.UseRouting();
        }

        private static string AppendCarsTable(string htmlString, IEnumerable<CarViewModel> cars)
        {
            var stringBuilder = new StringBuilder(htmlString);

            stringBuilder.Append("<div style=\"display: inline-block;\"><H1>Машины</H1>");
            stringBuilder.Append("<TABLE BORDER=1>");
            stringBuilder.Append("<TH>");
            stringBuilder.Append("<TD>#</TD>");
            stringBuilder.Append("<TD>Марка</TD>");
            stringBuilder.Append("<TD>Организация</TD>");
            stringBuilder.Append("<TD>Максимальный объем</TD>");
            stringBuilder.Append("<TD>Максимальный вес</TD>");
            stringBuilder.Append("</TH>");

            foreach (var car in cars)
            {
                stringBuilder.Append("<TR>");
                stringBuilder.Append("<TD>-</TD>");
                stringBuilder.Append($"<TD>{car.Id}</TD>");
                stringBuilder.Append($"<TD>{car.Mark}</TD>");
                stringBuilder.Append($"<TD>{car.Organization}</TD>");
                stringBuilder.Append($"<TD>{car.CarryingVolume}</TD>");
                stringBuilder.Append($"<TD>{car.CarryingWeight}</TD>");
                stringBuilder.Append("</TR>");
            }

            stringBuilder.Append("</table>");
            stringBuilder.Append("</div>");

            return stringBuilder.ToString();
        }

        private static string AppendOrganizationsTable(string htmlString, IEnumerable<Organization> organizations)
        {
            var stringBuilder = new StringBuilder(htmlString);

            stringBuilder.Append("<div style=\"display: inline-block; margin-left: 35px;\"><H1>Организации</H1>");
            stringBuilder.Append("<TABLE BORDER=1>");
            stringBuilder.Append("<TH>");
            stringBuilder.Append("<TD>#</TD>");
            stringBuilder.Append("<TD>Наименование</TD>");
            stringBuilder.Append("</TH>");

            foreach (var organization in organizations)
            {
                stringBuilder.Append("<TR>");
                stringBuilder.Append("<TD>-</TD>");
                stringBuilder.Append($"<TD>{organization.Id}</TD>");
                stringBuilder.Append($"<TD>{organization.Name}</TD>");
                stringBuilder.Append("</TR>");
            }

            stringBuilder.Append("</table>");
            stringBuilder.Append("</div>");

            return stringBuilder.ToString();
        }

        private static string AppendDriversTable(string htmlString, IEnumerable<Driver> drivers)
        {
            var stringBuilder = new StringBuilder(htmlString);

            stringBuilder.Append("<div style=\"display: inline-block; margin-left: 35px;\"><H1>Водители</H1>");
            stringBuilder.Append("<TABLE BORDER=1>");
            stringBuilder.Append("<TH>");
            stringBuilder.Append("<TD>#</TD>");
            stringBuilder.Append("<TD>Фамилия</TD>");
            stringBuilder.Append("<TD>Имя</TD>");
            stringBuilder.Append("<TD>Паспорт</TD>");
            stringBuilder.Append("</TH>");

            foreach (var driver in drivers)
            {
                stringBuilder.Append("<TR>");
                stringBuilder.Append("<TD>-</TD>");
                stringBuilder.Append($"<TD>{driver.Id}</TD>");
                stringBuilder.Append($"<TD>{driver.LastName}</TD>");
                stringBuilder.Append($"<TD>{driver.FirstName}</TD>");
                stringBuilder.Append($"<TD>{driver.Passport}</TD>");
                stringBuilder.Append("</TR>");
            }

            stringBuilder.Append("</table>");
            stringBuilder.Append("</div>");

            return stringBuilder.ToString();
        }

        private static string AppendMenu(string htmlString)
        {
            return htmlString +
                   "<div style=\"margin-top: 15px; margin-bottom: 15px;\">" +
                        "<div style=\"display: inline-block; margin-right: 10px;\"><a href=\"/info\">Информация</a></div>" +
                        "<div style=\"display: inline-block; margin-right: 10px;\"><a href=\"/\">Таблица с кешированием</a></div>" +
                        "<div style=\"display: inline-block; margin-right: 10px;\"><a href=\"/searchform1\">Поиск (куки)</a></div>" +
                        "<div style=\"display: inline-block; margin-right: 10px;\"><a href=\"/searchform2\">Поиск (сессия)</a></div>" +
                   "</div>";
        }

        private static void Index(IApplicationBuilder app)
        {
            app.Run(context =>
            {
                var service = context.RequestServices.GetService<ICarsService>();

                if (service == null)
                {
                    throw new InvalidOperationException($"Unable to retrieve {nameof(ICarsService)} service");
                }

                var models = service.GetHomeViewModel(CacheKey);
                var htmlString = "<HTML><HEAD>" +
                                 "<TITLE>С кешированием</TITLE></HEAD>" +
                                 "<META http-equiv='Content-Type' content='text/html; charset=utf-8 />'" +
                                 "<BODY>";
                htmlString = AppendMenu(htmlString);
                htmlString = AppendCarsTable(htmlString, models.Cars);
                htmlString = AppendOrganizationsTable(htmlString, models.Organizations);
                htmlString = AppendDriversTable(htmlString, models.Drivers);

                htmlString += "</BODY></HTML>";

                return context.Response.WriteAsync(htmlString);
            });
        }

        private static void SessionSearchHandler(IApplicationBuilder app)
        {
            app.Run(context =>
            {
                var carMarkParam = context.Request.Query["carMark"].ToString();
                var carOrganizationParam = context.Request.Query["carOrganization"].ToString();

                context.Session.Set("carMark", Encoding.Default.GetBytes(carMarkParam));
                context.Session.Set("carOrganization", Encoding.Default.GetBytes(carOrganizationParam));

                var service = context.RequestServices.GetService<ICarsService>();

                if (service == null)
                {
                    throw new InvalidOperationException($"Unable to retrieve {nameof(ICarsService)} service");
                }

                var htmlString = "<HTML><HEAD>" +
                                 "<TITLE>Поиск (сессия)</TITLE></HEAD>" +
                                 "<META http-equiv='Content-Type' content='text/html; charset=utf-8 />'" +
                                 "<BODY>";

                var cars = service.SearchCars(carOrganizationParam, carMarkParam);
                htmlString = AppendMenu(htmlString);
                htmlString = AppendCarsTable(htmlString, cars);

                return context.Response.WriteAsync(htmlString);
            });
        }

        private static void SessionSearch(IApplicationBuilder app)
        {
            app.Run(context =>
            {
                var htmlString = "<HTML><HEAD>" +
                                 "<TITLE>Поиск (сессия)</TITLE></HEAD>" +
                                 "<META http-equiv='Content-Type' content='text/html; charset=utf-8 />'" +
                                 "<BODY>";
                htmlString = AppendMenu(htmlString);

                if (!context.Session.TryGetValue("carMark", out var carMarkArray))
                {
                    carMarkArray = null;
                }

                var carMark = carMarkArray == null
                    ? string.Empty
                    : Encoding.Default.GetString(carMarkArray);

                if (!context.Session.TryGetValue("carOrganization", out var carOrganizationArray))
                {
                    carOrganizationArray = null;
                }

                var carOrganization = carOrganizationArray == null
                    ? string.Empty
                    : Encoding.Default.GetString(carOrganizationArray);

                var service = context.RequestServices.GetService<ICarsService>();

                if (service == null)
                {
                    throw new InvalidOperationException($"Unable to retrieve {nameof(ICarsService)} service");
                }

                var marks = service.GetMarks();

                var selectHtml = "<select name='carMark'>";
                foreach (var mark in marks)
                {
                    if (carMark != string.Empty && carMark == mark)
                    {
                        selectHtml += $"<option selected=\"selected\" value='{mark}'>" + mark + "</option>";
                    }

                    selectHtml += $"<option value='{mark}'>" + mark + "</option>";
                }

                selectHtml += "</select>";

                htmlString += "<form action = /search2 >" +
                "<br>Марка: " + selectHtml +
                "<br>Организация: " + $"<input type = 'text' name = 'carOrganization' value='{carOrganization}'>" +
                    "<br><input type = 'submit' value = 'Найти' ></form>";


                htmlString += "</BODY></HTML>";

                return context.Response.WriteAsync(htmlString);
            });
        }

        private static void CookieSearchHandler(IApplicationBuilder app)
        {
            app.Run(context =>
            {
                var carMarkParam = context.Request.Query["carMark"].ToString();
                var carOrganizationParam = context.Request.Query["carOrganization"].ToString();

                context.Response.Cookies.Append("carMark", carMarkParam);
                context.Response.Cookies.Append("carOrganization", carOrganizationParam);

                var service = context.RequestServices.GetService<ICarsService>();

                if (service == null)
                {
                    throw new InvalidOperationException($"Unable to retrieve {nameof(ICarsService)} service");
                }

                var htmlString = "<HTML><HEAD>" +
                                 "<TITLE>Поиск (куки)</TITLE></HEAD>" +
                                 "<META http-equiv='Content-Type' content='text/html; charset=utf-8 />'" +
                                 "<BODY>";

                var cars = service.SearchCars(carOrganizationParam, carMarkParam);
                htmlString = AppendMenu(htmlString);
                htmlString = AppendCarsTable(htmlString, cars);

                return context.Response.WriteAsync(htmlString);
            });
        }

        private static void CookieSearch(IApplicationBuilder app)
        {
            app.Run(context =>
            {
                var htmlString = "<HTML><HEAD>" +
                                 "<TITLE>Поиск (куки)</TITLE></HEAD>" +
                                 "<META http-equiv='Content-Type' content='text/html; charset=utf-8 />'" +
                                 "<BODY>";
                htmlString = AppendMenu(htmlString);

                if (!context.Request.Cookies.TryGetValue("carMark", out var carMark))
                {
                    carMark = string.Empty;
                }

                if (!context.Request.Cookies.TryGetValue("carOrganization", out var carOrganization))
                {
                    carOrganization = string.Empty;
                }

                var service = context.RequestServices.GetService<ICarsService>();

                if (service == null)
                {
                    throw new InvalidOperationException($"Unable to retrieve {nameof(ICarsService)} service");
                }

                var marks = service.GetMarks();

                var selectHtml = "<select name='carMark'>";
                foreach (var mark in marks)
                {
                    if (carMark != string.Empty && carMark == mark)
                    {
                        selectHtml += $"<option selected=\"selected\" value='{mark}'>" + mark + "</option>";
                    }

                    selectHtml += $"<option value='{mark}'>" + mark + "</option>";
                }

                selectHtml += "</select>";

                htmlString += "<form action = /search1 >" +
                "<br>Марка: " + selectHtml +
                "<br>Организация: " + $"<input type = 'text' name = 'carOrganization' value='{carOrganization}'>" +
                    "<br><input type = 'submit' value = 'Найти' ></form>";


                htmlString += "</BODY></HTML>";

                return context.Response.WriteAsync(htmlString);
            });
        }
    }
}