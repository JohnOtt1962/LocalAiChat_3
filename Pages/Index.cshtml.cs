using LocalAiLibrary.AiLibrary;
using LocalAiLibrary.AiLibrary.AiSessionService;
using LocalAiLibrary.AiLibrary.ChatService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LocalAiChat_2.Pages;

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
    public string SelectedCategoryId { get; set; }
    [BindProperty]
    public string UserPrompt { get; set; } = string.Empty;
    [BindProperty]
    public bool Disabled { get; set; }
    [BindProperty]
    public string SubmitText { get; set; } = "Send";
    public List<ChatRequestMessage> ConversationHistory { get; set; } = new();
    public List<SelectListItem> ChatCategoryOptions { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        string categoryId = SelectedCategoryId ?? "0";
        ConversationHistory = await _chatSessionService.GetHistory(categoryId);

        if (string.IsNullOrWhiteSpace(UserPrompt))
        {
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
        return Page();
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await GetCategories();
        ConversationHistory = await _chatSessionService.GetHistory("0");
        return Page();
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