using System.Net;
using System.Net.Http.Json;
using MudBlazor;
using Shared.DTOs;

namespace Client.Pages.Components;

public partial class ChangePasswordForm
{
    private PasswordChangeModel passwordModel = new();
    private bool isSubmitting = false;
    
    private InputType currentPasswordInput = InputType.Password;
    private string currentPasswordIcon = Icons.Material.Filled.VisibilityOff;
    private InputType newPasswordInput = InputType.Password;
    private string newPasswordIcon = Icons.Material.Filled.VisibilityOff;

    private async Task HandlePasswordChange()
    {
        isSubmitting = true;

        try
        {
            var response = await Http.PostAsJsonAsync("api/auth/change-password", passwordModel);

            if (response.IsSuccessStatusCode)
            {
                Snackbar.Add("The password has been changed!", Severity.Success);
                passwordModel = new(); // Form ürítése
            }
            else
            {
                Snackbar.Add("There was an error.", Severity.Error);
            }
        }
        catch (Exception)
        {
            Snackbar.Add("Network error.", Severity.Error);
        }
        finally
        {
            isSubmitting = false;
        }
    }

    private void ToggleCurrentPasswordVisibility()
    {
        if (currentPasswordInput == InputType.Password)
        {
            currentPasswordInput = InputType.Text;
            currentPasswordIcon = Icons.Material.Filled.Visibility;
        }
        else
        {
            currentPasswordInput = InputType.Password;
            currentPasswordIcon = Icons.Material.Filled.VisibilityOff;
        }
    }

    private void ToggleNewPasswordVisibility()
    {
        if (newPasswordInput == InputType.Password)
        {
            newPasswordInput = InputType.Text;
            newPasswordIcon = Icons.Material.Filled.Visibility;
        }
        else
        {
            newPasswordInput = InputType.Password;
            newPasswordIcon = Icons.Material.Filled.VisibilityOff;
        }
    }
}