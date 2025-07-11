﻿using Microsoft.EntityFrameworkCore;
using Ajimen.Data;

var builder = WebApplication.CreateBuilder(args);

// 🔧 SQLiteデータベースを使用するように設定（app.db というファイルに保存される）
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

// REST APIのための各種サービスを登録
builder.Services.AddControllers();             // コントローラー機能
builder.Services.AddEndpointsApiExplorer();    // SwaggerでAPI情報表示のため
builder.Services.AddSwaggerGen();              // Swagger UIの自動生成

// 🔓 CORSポリシーの定義：Vue.js（http://localhost:5173）からのアクセスを許可、vueのURLのポート番号
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowVueApp");

app.MapControllers();

// DB初期化（マイグレーション不要、テーブル作成＆初期データ登録）
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    SeedItem.Initialize(db);
    SeedOrdeLog.Initialize(db);
    SeedStockLog.Initialize(db);
}

app.Run();
