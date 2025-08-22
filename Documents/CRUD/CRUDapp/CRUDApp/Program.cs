using CRUDApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ⭐ 세션 서비스 추가
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 세션 유지 시간 (예: 30분)
    options.Cookie.HttpOnly = true; // 클라이언트 스크립트에서 접근 불가
    options.Cookie.IsEssential = true; // 필수 쿠키로 설정 (GDPR 준수 등)
});


// 데이터베이스 연결 문자열 설정
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))); // SQLite 사용

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ⭐ 세션 미들웨어 추가 (인증/인가 미들웨어보다 앞에 위치하는 것이 일반적입니다)
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();