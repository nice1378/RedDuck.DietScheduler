using System;
using System.Collections.Generic;

namespace WebDietScheduler.Models;

public class DailyDiet
{
    public DateTime Date { get; set; }
    public List<FoodItem> Breakfast { get; set; } = new();
    public List<FoodItem> Lunch { get; set; } = new();
    public List<FoodItem> Dinner { get; set; } = new();

    public int TotalCalories => GetCalories(Breakfast) + GetCalories(Lunch) + GetCalories(Dinner);
    public int TotalPrice => GetPrice(Breakfast) + GetPrice(Lunch) + GetPrice(Dinner);

    private int GetCalories(List<FoodItem> meal)
    {
        int sum = 0;
        foreach (var item in meal) sum += item.Calories;
        return sum;
    }

    private int GetPrice(List<FoodItem> meal)
    {
        int sum = 0;
        foreach (var item in meal) sum += item.Price;
        return sum;
    }
}

