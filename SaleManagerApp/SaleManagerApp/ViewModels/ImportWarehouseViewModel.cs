using SaleManagerApp.Helpers;
using SaleManagerApp.Models;
using SaleManagerApp.Services;
using SaleManagerApp.ViewModels;
using System.Windows.Input;
using System;

public class ImportIngredientViewModel : BaseViewModel
{
    private readonly WarehouseService _service = new WarehouseService();

    public string IngredientId { get; private set; }
    public string IngredientName { get; private set; }
    public string Unit { get; private set; }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set
        {
            _quantity = value;
            OnPropertyChanged();
        }
    }

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    public Action CloseAction { get; set; }
    public Action ReloadAction { get; set; }

    public ImportIngredientViewModel()
    {
        ConfirmCommand = new RelayCommand(Import);
        CancelCommand = new RelayCommand(_ => CloseAction?.Invoke());
    }

    // ⭐⭐ THÊM HÀM NÀY ⭐⭐
    public void SetIngredient(Ingredient ingredient)
    {
        IngredientId = ingredient.ingredientId;
        IngredientName = ingredient.ingredientName;
        Unit = ingredient.unit;

        OnPropertyChanged(nameof(IngredientName));
        OnPropertyChanged(nameof(Unit));
    }

    private void Import(object obj)
    {
        if (Quantity <= 0)
        {
            ToastService.ShowError("Số lượng nhập không hợp lệ");
            return;
        }

        var result = _service.ImportIngredient(IngredientId, Quantity);
        if (result.Success)
        {
            ReloadAction?.Invoke();
            ToastService.Show(result.SuccessMessage);
            CloseAction?.Invoke();
        }
        else
        {
            ToastService.ShowError(result.ErrorMessage);
        }
    }
}
