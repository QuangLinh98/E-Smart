using E_Smart.Areas.Admin.Repository;
using E_Smart.Areas.Admin.Service;
using E_Smart.Areas.Client.Repository;
using E_Smart.Areas.Client.Service;
using E_Smart.Data;
using E_Smart.Mail;
using E_Smart.Service;
using E_Smart.Utilities;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký DatabaseContext
builder.Services.AddDbContext<DatabaseContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectDB"));
});

builder.Services.AddScoped<ICategoryRepository,CategoryService>();
builder.Services.AddScoped<IProductRepository,ProductService>();
builder.Services.AddScoped<ICustomerRepository,CustomerService>();
builder.Services.AddScoped<IOrderRepository,OrderService>();
builder.Services.AddScoped<IOrderDetailRepository,OrderDetailService>();
builder.Services.AddScoped<IUserRepository,UserService>();

//Đăng ký Service Email
builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSetting"));
builder.Services.AddTransient<EmailService>();

//Đăng ký VnPayCLient dạng Singleton() - chỉ có 1 instance duy nhất trong toàn ứng dụng
builder.Services.AddSingleton<IVnPayService, VnPayService>();


//Đăng ký dịch vụ lưu trữ Cache cho phần lưu dữ liệu của order
builder.Services.AddDistributedMemoryCache();

// Đăng ký Session
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.IsEssential = true;
});



var app = builder.Build();

//Đăng ký Sử dụng Session
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

/*//Cấu hình Thanh toán Stripe
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();*/

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}"
);

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
