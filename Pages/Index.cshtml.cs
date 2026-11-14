using LocalAiLibrary.AiLibrary;
using LocalAiLibrary.AiLibrary.AiSessionService;
using LocalAiLibrary.AiLibrary.ChatService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LocalAiChat_4.Pages;

public class IndexModel : PageModel
{
    private readonly IChatSessionService _chatSessionService;
    private readonly IAiManager _aiManager;

    public IndexModel(IAiManager aiManager, IChatSessionService chatSessionService)
    {
        _chatSessionService = chatSessionService;
        _aiManager = aiManager;
    }

    [BindProperty]
    public string SelectedApplicationId { get; set; }
    [BindProperty]
    public string SelectedCategoryId { get; set; }
    [BindProperty]
    public string UserPrompt { get; set; } = string.Empty;
    [BindProperty]
    public bool Disabled { get; set; }
    [BindProperty]
    public string SubmitText { get; set; } = "Send";
    public List<ChatRequestMessage> ConversationHistory { get; set; } = new();
    public List<SelectListItem> ChatCategoryOptions { get; set; } = new();
    public List<SelectListItem> ChatApplicationOptions { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        string categoryId = SelectedCategoryId ?? "0";

        ConversationHistory = await _chatSessionService.GetHistory(categoryId);
        _chatSessionService.SetApplicationName(Convert.ToInt16(SelectedApplicationId));
        if (string.IsNullOrWhiteSpace(UserPrompt))
        {
            await GetApplications();
            await GetCategories();
            return Page();
        }

        await _aiManager.MakeAiCallAsync(ConversationHistory, UserPrompt);
        _chatSessionService.ClearHistory();
        ConversationHistory = await _chatSessionService.GetHistory(categoryId);

        ModelState.Clear();
        UserPrompt = string.Empty;
        Disabled = false;
        SubmitText = "Send";

        await GetCategories();
        await GetApplications();
        return Page();
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await GetCategories();
        await GetApplications();
        ConversationHistory = await _chatSessionService.GetHistory("0");
        return Page();
    }

    private async Task GetApplications()
    {
        var chatApplicatons = await _chatSessionService.GetApplications();
        if (chatApplicatons != null)
            ChatApplicationOptions = chatApplicatons.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
    }

    private async Task GetCategories()
    {
        var chatCategories = await _chatSessionService.GetCategories();
        if (chatCategories != null)
            ChatCategoryOptions = chatCategories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CategoryName
            }).ToList();
    }
}