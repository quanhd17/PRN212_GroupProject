using System;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FinalProject.Models;
using System.Collections.ObjectModel;

namespace FinalProject.ViewModels
{
    public class StaffStatisticsViewModel : INotifyPropertyChanged
    {
        private readonly Prn212DbContext _dbContext;

        public int TotalOrdersToday { get; set; }
        public decimal TotalRevenueToday { get; set; }
        public int TotalOrdersMonth { get; set; }
        public decimal TotalRevenueMonth { get; set; }
        public string MostPopularItemToday { get; set; }
        public string MostPopularItemMonth { get; set; }
        public ObservableCollection<DailyStat> DailyStats { get; set; }

        public StaffStatisticsViewModel()
        {
            _dbContext = new Prn212DbContext();
            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);

            // Orders today
            var ordersToday = _dbContext.Orders.Where(o => o.OrderDate >= today && o.Status == Models.Enum.OrderStatusEnum.Completed);
            TotalOrdersToday = ordersToday.Count();
            TotalRevenueToday = ordersToday.Sum(o => o.FinalAmount ?? 0);

            // Orders this month
            var ordersMonth = _dbContext.Orders.Where(o => o.OrderDate >= monthStart && o.Status == Models.Enum.OrderStatusEnum.Completed);
            TotalOrdersMonth = ordersMonth.Count();
            TotalRevenueMonth = ordersMonth.Sum(o => o.FinalAmount ?? 0);

            // Most popular item today
            MostPopularItemToday = ordersToday
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => oi.Item.ItemName)
                .OrderByDescending(g => g.Sum(oi => oi.Quantity))
                .Select(g => g.Key)
                .FirstOrDefault() ?? "N/A";

            // Most popular item this month
            MostPopularItemMonth = ordersMonth
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => oi.Item.ItemName)
                .OrderByDescending(g => g.Sum(oi => oi.Quantity))
                .Select(g => g.Key)
                .FirstOrDefault() ?? "N/A";

            // Daily stats for current month
            DailyStats = new ObservableCollection<DailyStat>();
            for (var date = monthStart; date <= today; date = date.AddDays(1))
            {
                var orders = _dbContext.Orders.Where(o => o.OrderDate >= date && o.OrderDate < date.AddDays(1) && o.Status == Models.Enum.OrderStatusEnum.Completed);
                var orderCount = orders.Count();
                var revenue = orders.Sum(o => o.FinalAmount ?? 0);
                var mostPopular = orders
                    .SelectMany(o => o.OrderItems)
                    .GroupBy(oi => oi.Item.ItemName)
                    .OrderByDescending(g => g.Sum(oi => oi.Quantity))
                    .Select(g => g.Key)
                    .FirstOrDefault() ?? "N/A";
                DailyStats.Add(new DailyStat
                {
                    Date = date,
                    Orders = orderCount,
                    Revenue = revenue,
                    MostPopularItem = mostPopular
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class DailyStat
    {
        public DateTime Date { get; set; }
        public int Orders { get; set; }
        public decimal Revenue { get; set; }
        public string MostPopularItem { get; set; }
    }
} 