using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class RevenueReport
{
    public int ReportId { get; set; }

    public DateOnly ReportDate { get; set; }

    public int? TotalOrders { get; set; }

    public decimal? TotalRevenue { get; set; }

    public DateTime? CreatedAt { get; set; }
}
