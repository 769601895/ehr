using IdCardReaderAgent;

var builder = WebApplication.CreateBuilder(args);

var idCardOpt = new IdCardOptions();
builder.Configuration.GetSection("IdCard").Bind(idCardOpt);
builder.Services.AddSingleton(idCardOpt);
builder.Services.AddSingleton<IdCardReadService>();

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(_ => true)));

var urls = builder.Configuration["Urls"] ?? "http://127.0.0.1:19196";
builder.WebHost.UseUrls(urls);

var app = builder.Build();
app.UseCors();

app.MapGet("/health", (IdCardOptions opt) => Results.Json(new
{
    ok = true,
    service = "IdCardReaderAgent",
    api = "sdtapi",
    mock = opt.Mock,
    commPort = opt.CommPort
}));

app.MapPost("/idcard/read", (IdCardReadService svc) =>
{
    var r = svc.Read();
    return Results.Json(r);
});

app.MapGet("/idcard/read", (IdCardReadService svc) =>
{
    var r = svc.Read();
    return Results.Json(r);
});

Console.WriteLine($"IdCardReaderAgent listening on {urls} (公安部标准 sdtapi)");
app.Run();
