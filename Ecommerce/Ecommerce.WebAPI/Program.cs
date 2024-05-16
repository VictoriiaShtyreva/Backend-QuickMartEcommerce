using System.Text;
using CloudinaryDotNet;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Entities.CartAggregate;
using Ecommerce.Core.src.Entities.OrderAggregate;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.Service.src.Interfaces;
using Ecommerce.Service.src.Services;
using Ecommerce.Service.src.Shared;
using Ecommerce.WebAPI.src.AuthorizationPolicy;
using Ecommerce.WebAPI.src.Data;
using Ecommerce.WebAPI.src.ExternalService;
using Ecommerce.WebAPI.src.Middleware;
using Ecommerce.WebAPI.src.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

// add automapper dependency injection
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

//Cloudinary
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

//Cashing
builder.Services.AddMemoryCache();

// Add all controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put Bearer + your token in the box below",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

// adding db context into app
var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("Localhost"));
dataSourceBuilder.MapEnum<UserRole>();
dataSourceBuilder.MapEnum<OrderStatus>();
var dataSource = dataSourceBuilder.Build();
builder.Services.AddDbContext<AppDbContext>
(
options =>
options.UseNpgsql(dataSource)
        .UseSnakeCaseNamingConvention()
        .UseLazyLoadingProxies()
        .EnableDetailedErrors()
        .AddInterceptors(new TimeStampInteceptor())
);

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


// service registration -> automatically create all instances of dependencies
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
// Auth
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
// Category
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
// Review
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
// Product
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
// ProductImage
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
// Cart and CartItems
builder.Services.AddScoped<ICartItemService, CartItemService>();
builder.Services.AddScoped<IBaseRepository<CartItem, QueryOptions>, CartItemRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();
//Order and OrderItems
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IBaseRepository<OrderItem, QueryOptions>, OrderItemRepository>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
// Address
builder.Services.AddScoped<IBaseRepository<Address, QueryOptions>, AddressRepository>();
//UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//Cloudinary
builder.Services.AddScoped<ICloudinaryImageService, CloudinaryImageService>();
// Configure Cloudinary
var cloudinaryAccount = new Account(
    builder.Configuration["CloudinarySettings:CloudName"],
    builder.Configuration["CloudinarySettings:ApiKey"],
    builder.Configuration["CloudinarySettings:ApiSecret"]
);
var cloudinary = new Cloudinary(cloudinaryAccount);
builder.Services.AddSingleton(cloudinary);

// Add authentication instructions
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Secrets:JwtKey"]!)),
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Secrets:Issuer"]
        };
    }
);

// Add authorization instructions
builder.Services.AddAuthorizationBuilder()
     // Add authorization instructions
     .AddPolicy("AdminOrOwnerAccount", policy => policy.Requirements.Add(new AdminOrOwnerAccountRequirement()))
     // Add authorization instructions
     .AddPolicy("AdminOrOwnerOrder", policy => policy.Requirements.Add(new AdminOrOwnerOrderRequirement()))
     // Add authorization instructions
     .AddPolicy("AdminOrOwnerReview", policy => policy.Requirements.Add(new AdminOrOwnerReviewRequirement()));

var app = builder.Build();

app.UseCors("AllowAllOrigins");

app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler("/Error");
app.UseDeveloperExceptionPage();
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
