using FluentValidation;
using LocalAiLibrary.AiLibrary;
using LocalAiLibrary.AiLibrary.AiDbService;
using LocalAiLibrary.AiLibrary.AiSessionService;
using LocalAiLibrary.AiLibrary.CategoryClassification;
using LocalAiLibrary.AiLibrary.CategoryClassification.AiCategoryDbService;
using LocalAiLibrary.AiLibrary.CategoryClassification.AiCategoryService;
using LocalAiLibrary.AiLibrary.CategoryClassification.Models;
using LocalAiLibrary.AiLibrary.ChatService;
using LocalAiLibrary.AiLibrary.ChatService.Models;
using LocalAiLibrary.AiLibrary.Network;
using Utilities.Email;
using Utilities.Network;
using Utilities.Repo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.Configure<AiConfig>(builder.Configuration.GetSection("AI-Config"));
builder.Services.Configure<Utilities.Email.EmailConfig>(builder.Configuration.GetSection("Email-Config"));
builder.Services.Configure<AiChatCategoryConfig>(builder.Configuration.GetSection("AI-ChatCategory"));

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddValidatorsFromAssembly(typeof(AiCommunicationManager).Assembly);

builder.Services.AddScoped<IAiManager, AiManager>();
builder.Services.AddScoped<IChatSessionService, ChatSessionService>();
builder.Services.AddScoped<IAiCommunicationManager, AiCommunicationManager>();
builder.Services.AddScoped<IEmail, Email>();
builder.Services.AddScoped<INetwork, Network>();
builder.Services.AddScoped<IRepo, Repository>();
builder.Services.AddScoped<IAiNetwork, AiNetwork>();
builder.Services.AddScoped<IAiChatCategoryRepo, AiChatCategoryRepo>();
builder.Services.AddScoped<IManageChatEntryClassificationService, ManageChatEntryClassificationService>();
builder.Services.AddScoped<IAiCategoryManager, AiCategoryManager>();
builder.Services.AddScoped<IAiRepo, AiRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseSession(); // Add before app.MapRazorPages()
app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();