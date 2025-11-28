using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WebDietScheduler.Models;
using WebDietScheduler.Services;

namespace WebDietScheduler.Utils;

public class SampleDataGenerator
{
    public static string GenerateOneYearHistory(DietGeneratorService service)
    {
        // 1. 기본 식품 데이터 (FoodPriceList.csv 와 동일한 내용으로 하드코딩 혹은 로드)
        var foods = new List<FoodItem>
        {
            new() { Name="현미밥", Category="밥", Calories=300, Price=1000, Season="사계절", Target="전체" },
            new() { Name="미역국", Category="국", Calories=80, Price=1500, Season="사계절", Target="전체" },
            new() { Name="김치", Category="반찬", Calories=20, Price=500, Season="사계절", Target="전체" },
            new() { Name="불고기", Category="메인", Calories=400, Price=5000, Season="사계절", Target="전체" },
            new() { Name="시금치나물", Category="반찬", Calories=40, Price=800, Season="봄", Target="전체" },
            // ... 더 많은 데이터를 넣으면 좋지만, 로직 테스트용으로 위 PriceList 내용 일부 활용
            new() { Name="된장국", Category="국", Calories=90, Price=1200, Season="사계절", Target="전체" },
            new() { Name="콩나물무침", Category="반찬", Calories=35, Price=600, Season="사계절", Target="전체" },
            new() { Name="계란말이", Category="반찬", Calories=100, Price=1500, Season="사계절", Target="전체" },
            new() { Name="삼계탕", Category="메인", Calories=600, Price=8000, Season="여름", Target="전체" },
            new() { Name="떡국", Category="메인", Calories=400, Price=3000, Season="겨울", Target="전체" },
            new() { Name="고등어구이", Category="메인", Calories=350, Price=4000, Season="가을", Target="전체" }
        };

        var request = new DietRequest
        {
            StartDate = new DateTime(2024, 1, 1),
            DurationDays = 365,
            TargetAudience = "전체"
        };

        // 서비스 로직을 이용해 1년치 생성
        var plan = service.GenerateDiet(request, foods, new List<string>());
        
        // CSV 변환
        var csv = new StringBuilder();
        csv.AppendLine("Date,Meal,Menu,Calories,Price");
        foreach (var day in plan)
        {
            csv.AppendLine($"{day.Date:yyyy-MM-dd},Breakfast,\"{string.Join("+", GetNames(day.Breakfast))}\",{GetCal(day.Breakfast)},{GetPri(day.Breakfast)}");
            csv.AppendLine($"{day.Date:yyyy-MM-dd},Lunch,\"{string.Join("+", GetNames(day.Lunch))}\",{GetCal(day.Lunch)},{GetPri(day.Lunch)}");
            csv.AppendLine($"{day.Date:yyyy-MM-dd},Dinner,\"{string.Join("+", GetNames(day.Dinner))}\",{GetCal(day.Dinner)},{GetPri(day.Dinner)}");
        }
        return csv.ToString();
    }

    private static IEnumerable<string> GetNames(List<FoodItem> l) => l.ConvertAll(x => x.Name);
    private static int GetCal(List<FoodItem> l) { int s=0; foreach(var i in l) s+=i.Calories; return s; }
    private static int GetPri(List<FoodItem> l) { int s=0; foreach(var i in l) s+=i.Price; return s; }
}

