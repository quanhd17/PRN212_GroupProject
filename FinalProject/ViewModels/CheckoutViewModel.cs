using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using FinalProject.Models;

namespace FinalProject.ViewModels
{
    public class CheckoutViewModel : INotifyPropertyChanged
    {
        private decimal _paymentAmount;
        private decimal _change;
        private Order _order;

        public CheckoutViewModel(Order order)
        {
            _order = order;
            OrderItems = new ObservableCollection<OrderItem>(order.OrderItems);
            TotalAmount = order.TotalAmount ?? 0;
            Discount = order.Discount ?? 0;
            FinalAmount = order.FinalAmount ?? 0;
        }

        public ObservableCollection<OrderItem> OrderItems { get; }
        public decimal TotalAmount { get; }
        public decimal Discount { get; }
        public decimal FinalAmount { get; }

        public decimal PaymentAmount
        {
            get => _paymentAmount;
            set
            {
                if (_paymentAmount != value)
                {
                    _paymentAmount = value;
                    OnPropertyChanged();
                    UpdateChange();
                }
            }
        }

        public decimal Change
        {
            get => _change;
            set { _change = value; OnPropertyChanged(); }
        }

        private void UpdateChange()
        {
            Change = PaymentAmount - FinalAmount;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 