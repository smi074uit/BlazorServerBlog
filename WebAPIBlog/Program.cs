using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using WebAPIBlog;
using WebAPIBlog.Data;
using WebAPIBlog.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme { In = ParameterLocation.Header, Name = "Authorization", Type = SecuritySchemeType.ApiKey });
    options.OperationFilter<SecurityRequirementsOperationFilter>();

});

builder.Services.AddTransient<IAccountsRepository, AccountsRepository>();
builder.Services.AddTransient<IBlogRepository, BlogRepository>();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddRoleManager<RoleManager<IdentityRole>>();


//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//	.AddRoleManager<RoleManager<IdentityRole>>()
//	.AddSignInManager<SignInManager<IdentityUser>>()
//	.AddDefaultUI()
//	//.AddEntityFrameworkStores<ApplicationDbContext>()
//	.AddDefaultTokenProviders();
// støtte for JWT & Cookies
var confKey = builder.Configuration.GetSection("TokenSettings")["SecretKey"];
var key = Encoding.ASCII.GetBytes(confKey);

builder.Services.AddAuthentication()
    .AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.PrepareDatabase()
    .GetAwaiter()
    .GetResult();

app.Run();
