using System;

namespace WebDietScheduler.Models;

public class DietRequest
{
    public DateTime StartDate { get; set; } = DateTime.Today;
    public int DurationDays { get; set; } = 7;
    public string TargetAudience { get; set; } = "Adult"; // Child, Adult, Senior
    
    // 예산 (끼니당)
    public int BreakfastBudget { get; set; } = 5000;
    public int LunchBudget { get; set; } = 8000;
    public int DinnerBudget { get; set; } = 8000;

    // 칼로리 (끼니당)
    public int BreakfastCalories { get; set; } = 500;
    public int LunchCalories { get; set; } = 800;
    public int DinnerCalories { get; set; } = 700;
}

