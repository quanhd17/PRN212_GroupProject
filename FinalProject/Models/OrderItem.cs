using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FinalProject.Models;

public partial class OrderItem : INotifyPropertyChanged
{
    public int OrderId { get; set; }

    public int ItemId { get; set; }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set { if (_quantity != value) { _quantity = value; OnPropertyChanged(); OnPropertyChanged(nameof(Total)); } }
    }

    private decimal _itemPrice;
    public decimal ItemPrice
    {
        get => _itemPrice;
        set { if (_itemPrice != value) { _itemPrice = value; OnPropertyChanged(); OnPropertyChanged(nameof(Total)); } }
    }

    public decimal Total => Quantity * ItemPrice;

    public virtual MenuItem Item { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
