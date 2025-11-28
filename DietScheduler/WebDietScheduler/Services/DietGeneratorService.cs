using System;
using System.Collections.Generic;
using System.Linq;
using WebDietScheduler.Models;

namespace WebDietScheduler.Services;

public class DietGeneratorService
{
    private readonly Random _random = new Random();

    public List<DailyDiet> GenerateDiet(DietRequest request, List<FoodItem> foodDatabase, List<string> previousMenus)
    {
        var plan = new List<DailyDiet>();
        var currentDate = request.StartDate;

        // 대상에 맞는 음식만 1차 필터링
        var candidates = foodDatabase.Where(f => 
            string.IsNullOrEmpty(f.Target) || 
            f.Target == "전체" || 
            f.Target.Contains(request.TargetAudience)
        ).ToList();

        // 계절 필터링
        string currentSeason = GetSeason(currentDate.Month);
        var seasonalCandidates = candidates.Where(f => 
            string.IsNullOrEmpty(f.Season) || 
            f.Season == "사계절" || 
            f.Season.Contains(currentSeason)
        ).ToList();
        
        if (seasonalCandidates.Count < 5) seasonalCandidates = candidates;

        for (int i = 0; i < request.DurationDays; i++)
        {
            var dailyDiet = new DailyDiet { Date = currentDate };

            // 하루 동안 사용된 메뉴 카운트 (중복 방지용)
            var dailyUsage = new Dictionary<string, int>();

            dailyDiet.Breakfast = PickMeal(seasonalCandidates, request.BreakfastBudget, request.BreakfastCalories, dailyUsage);
            dailyDiet.Lunch = PickMeal(seasonalCandidates, request.LunchBudget, request.LunchCalories, dailyUsage);
            dailyDiet.Dinner = PickMeal(seasonalCandidates, request.DinnerBudget, request.DinnerCalories, dailyUsage);

            plan.Add(dailyDiet);
            currentDate = currentDate.AddDays(1);
        }

        return plan;
    }

    private List<FoodItem> PickMeal(List<FoodItem> allCandidates, int budgetLimit, int calorieLimit, Dictionary<string, int> dailyUsage)
    {
        // 1. 하루 빈도 제한 체크
        var candidates = allCandidates.Where(c => 
        {
            int usedCount = dailyUsage.ContainsKey(c.Name) ? dailyUsage[c.Name] : 0;
            return usedCount < c.MaxFrequencyPerDay;
        }).ToList();

        var meal = new List<FoodItem>();
        if (!candidates.Any()) return meal;

        // 2. 카테고리별 분류 (밥, 메인, 국, 반찬)
        var rices = candidates.Where(c => c.Category.Contains("밥")).ToList();
        var soups = candidates.Where(c => c.Category.Contains("국") || c.Category.Contains("찌개")).ToList();
        var mains = candidates.Where(c => c.Category.Contains("메인")).ToList();
        var sides = candidates.Where(c => !c.Category.Contains("밥") && !c.Category.Contains("메인") && !c.Category.Contains("국") && !c.Category.Contains("찌개")).ToList();

        // 만약 '밥' 카테고리가 없으면 '메인' 중 밥류(볶음밥 등)가 있을 수 있으니 mains에서 가져오거나, 정 없으면 전체에서 찾음
        if (!rices.Any()) rices = mains.Where(m => m.Name.Contains("밥")).ToList();

        int currentPrice = 0;
        int currentCal = 0;

        // [필수 1] 밥 추가 (무조건)
        if (rices.Any())
        {
            var rice = rices[_random.Next(rices.Count)];
            AddItemToMeal(meal, rice, dailyUsage);
            currentPrice += rice.Price;
            currentCal += rice.Calories;
        }

        // [필수 2] 국 추가 (무조건)
        if (soups.Any())
        {
            var soup = soups[_random.Next(soups.Count)];
            AddItemToMeal(meal, soup, dailyUsage);
            currentPrice += soup.Price;
            currentCal += soup.Calories;
        }
        
        // [필수 3] 메인 반찬 1개 추가 (메인 우선, 없으면 일반 반찬)
        var mainDishCandidates = mains.Any() ? mains : sides;
        if (mainDishCandidates.Any())
        {
            // 이미 밥/국을 추가했으므로 중복 체크
            var availableMains = mainDishCandidates.Where(m => !meal.Contains(m)).ToList();
            if (availableMains.Any())
            {
                var mainDish = availableMains[_random.Next(availableMains.Count)];
                AddItemToMeal(meal, mainDish, dailyUsage);
                currentPrice += mainDish.Price;
                currentCal += mainDish.Calories;
            }
        }

        // [선택] 추가 반찬 (예산/칼로리 여유 있을 때만)
        if (sides.Any())
        {
            // 최대 2개 더 시도
            for (int k = 0; k < 2; k++)
            {
                var availableSides = sides.Where(s => !meal.Contains(s)).ToList();
                if (!availableSides.Any()) break;

                var side = availableSides[_random.Next(availableSides.Count)];
                
                // 예산 체크 (필수 요소들은 이미 들어갔으므로, 추가 반찬은 엄격하게 체크)
                if (CanAdd(currentPrice, currentCal, side, budgetLimit, calorieLimit))
                {
                    AddItemToMeal(meal, side, dailyUsage);
                    currentPrice += side.Price;
                    currentCal += side.Calories;
                }
            }
        }
        
        return meal;
    }

    private void AddItemToMeal(List<FoodItem> meal, FoodItem item, Dictionary<string, int> dailyUsage)
    {
        meal.Add(item);
        if (!dailyUsage.ContainsKey(item.Name)) dailyUsage[item.Name] = 0;
        dailyUsage[item.Name]++;
    }

    private bool CanAdd(int currentPrice, int currentCal, FoodItem item, int budgetLimit, int calorieLimit)
    {
        return (currentPrice + item.Price <= budgetLimit * 1.1) && (currentCal + item.Calories <= calorieLimit * 1.2); 
    }

    private string GetSeason(int month)
    {
        return month switch
        {
            12 or 1 or 2 => "겨울",
            3 or 4 or 5 => "봄",
            6 or 7 or 8 => "여름",
            9 or 10 or 11 => "가을",
            _ => "사계절"
        };
    }
}
