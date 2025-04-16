namespace AspNetCoreStudy.Controllers;

using AspNetCoreStudy.Attribute;
using AspNetCoreStudy.DTO;
using AspNetCoreStudy.Service;
using Microsoft.AspNetCore.Mvc;

[ApiController, Route("[controller]")]
public sealed class AccountController : ControllerBase
{
    private AccountService accountService;
    private SessionService sessionService;

    public AccountController(AccountService accountService,
                             SessionService sessionService)
    {
        this.accountService = accountService;
        this.sessionService = sessionService;
    }

    [HttpPost("Register"), SkipSessionFilter]
    public async Task<IActionResult> TryRegister(AccountDTO dto)
    {
        bool result = await this.accountService.TryRegister(dto);
        return this.Ok(result);
    }

    [HttpPost("Deregister")]
    public async Task<IActionResult> TryDeregister(AccountDTO dto)
    {
        bool result = await this.accountService.TryDeregister(dto);

        if (result == true)
        {
            sessionService.Remove(dto.Id);
        }

        return this.Ok(result);
    }

    [HttpGet("Login"), SkipSessionFilter]
    public async Task<IActionResult> TryLogin(AccountDTO dto)
    {
        bool result = await this.accountService.TryLogin(dto);

        if (result == false)
        {
            return this.Ok(false);
        }

        string sessionId = sessionService.Upsert(dto.Id);
        return this.Ok(sessionId);
    }
}
