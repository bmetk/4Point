using System.Text;
using BlazorBootstrap;
using Common.Extensions;
using Microsoft.AspNetCore.Components;
using WebService.Models;
using WebService.Service;

namespace WebService.Pages;

public partial class Index : ComponentBase
{
    private bool _errorChecked;
    private RemainingTimePredictorService? _remainingTimeService;

    private string AuthTopic => MqttTopics.AuthTopic(Configuration);
    private string ErrorCheckedTopic => MqttTopics.ErrorCheckedTopic(Configuration);

    private readonly List<string> _errorHandlerJobs = new();
    private byte[] AsByteArray(byte data) => Encoding.UTF8.GetBytes(data == 0 ? "0" : "1");
    
    private readonly record struct AlertState(AlertColor Color, string AlertText);
    private static readonly Dictionary<bool, AlertState> AlertModes = new()
    {
        { true, new AlertState(AlertColor.Success, "Sikeresen Authentikálva!") },
        { false, new AlertState(AlertColor.Danger, "Nincs Authentikálva!") }
    };
    
    private void ErrorCheckedTopicHandler()
    {
        var newStatus = Status.ChangeValue(ErrorCheckedTopic, b =>
        {
            byte res = (byte)(b == 0 ? 1 : 0);
            Logger.LogInformation("The value of b is {ErrorCheckedStatus}", res);
            _errorChecked = res == 1;
            return res;
        });

        if (_remainingTimeService is null)
        {
            _remainingTimeService =
                new RemainingTimePredictorService(_goalAmount).WithLogger(Logger);
            _remainingTimeService.AddEventHandler(async remainingTime =>
            {
                if(_goalAmount == 0) return;
                _remainingTime = DateTimeOffset
                    .FromUnixTimeMilliseconds((long)remainingTime.TotalMilliseconds)
                    .ToString("T");
                await InvokeAsync(StateHasChanged);
            });
        }
        
        MqttService.PublishAsync(ErrorCheckedTopic, AsByteArray(newStatus));
    }
    
    private async Task HandleRfidAuthMessage(string? user)
    {
        if (user is null)
        {
            Status.ChangeValue(AuthTopic, _ => 0);
            AlertModes[false].Deconstruct(out AuthAlertColor, out AuthAlertText);
        }
        else
        {
            Status.ChangeValue(AuthTopic, _ => 1);
            AlertModes[true].Deconstruct(out AuthAlertColor, out _);
            AuthAlertText = $"Operátor: {user}";
        }
        await InvokeAsync(StateHasChanged);
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _errorChecked = false;
            AnyExtensions.AsList(AuthTopic, ErrorCheckedTopic).ForEachElement(s => Status.AddTopic(s));
            RfidStatusObserver = new RfidStatusObserver(Logger, HandleRfidAuthMessage);
            RfidAuthMqttService.Subscribe(RfidStatusObserver);
            
            _errorHandlerJobs.Add(FailService.RegisterJob(async () => 
            {
                Status.ChangeValue(ErrorCheckedTopic, _ => 0);
                _errorChecked = false;
                await InvokeAsync(async () =>
                {
                    await ModalService.ShowAsync(new ModalOption()
                    {
                        Title = "Hiba",
                        Message = @"Hiba történt a gyártás során. Kérem ellenőrizze a gyártósort
Amennyiben a hiba elhárult, indítsa újra a gyártást.",
                        Type = ModalType.Danger,
                        IsVerticallyCentered = true
                    });
                    await InvokeAsync(StateHasChanged);
                });
            }));
        }

        return Task.CompletedTask;
    }
    
    public ValueTask DisposeAsync()
    {
        RfidAuthMqttService.Unsubscribe(RfidStatusObserver!);
        
        _errorHandlerJobs.ForEach(job => FailService.RemoveJobById(job));
        
        return ValueTask.CompletedTask;
    }
}