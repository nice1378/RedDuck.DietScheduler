using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using WebDietScheduler.Models;

namespace WebDietScheduler.Services;

public class CsvService
{
    // FoodItem CSV 파싱 (파일 업로드)
    public async Task<List<FoodItem>> ParseFoodItemsAsync(IBrowserFile file)
    {
        // 최대 5MB 제한
        using var stream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
        using var reader = new StreamReader(stream);
        return await ParseFoodItemsFromReaderAsync(reader);
    }

    // FoodItem CSV 파싱 (문자열 스트림)
    public async Task<List<FoodItem>> ParseFoodItemsFromStringAsync(string csvContent)
    {
        using var reader = new StringReader(csvContent);
        return await ParseFoodItemsFromReaderAsync(reader);
    }

    private async Task<List<FoodItem>> ParseFoodItemsFromReaderAsync(TextReader reader)
    {
        var items = new List<FoodItem>();
        // 헤더 건너뛰기 여부는 상황에 따라 다르지만, 보통 첫 줄은 헤더로 가정
        string? header = await reader.ReadLineAsync(); 
        
        while (reader.Peek() != -1)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',');
            if (parts.Length < 6) continue; // 최소 필드 개수 체크

            try
            {
                var item = new FoodItem
                {
                    Name = parts[0].Trim(),
                    Category = parts[1].Trim(),
                    Calories = int.Parse(parts[2].Trim()),
                    Price = int.Parse(parts[3].Trim()),
                    Season = parts[4].Trim(),
                    Target = parts[5].Trim()
                };
                
                // 7번째 컬럼(빈도)이 있으면 파싱, 없으면 기본값 1 유지
                if (parts.Length > 6 && int.TryParse(parts[6].Trim(), out int freq))
                {
                    item.MaxFrequencyPerDay = freq;
                }
                // 밥이나 김치 같은 기본 메뉴는 명시되지 않아도 빈도를 높여줌 (임시 편의 로직)
                else if (item.Category.Contains("밥") || item.Name.Contains("김치"))
                {
                    item.MaxFrequencyPerDay = 3; 
                }

                items.Add(item);
            }
            catch
            {
                // 파싱 에러 라인은 무시하거나 로그 처리
            }
        }
        return items;
    }

    // 결과 CSV 생성
    public string GenerateDietCsv(List<DailyDiet> dietPlan)
    {
        var csv = new System.Text.StringBuilder();
        csv.AppendLine("Date,Meal,Menu,Calories,Price");

        foreach (var day in dietPlan)
        {
            // 아침
            csv.AppendLine($"{day.Date:yyyy-MM-dd},Breakfast,\"{GetMenuString(day.Breakfast)}\",{GetCalories(day.Breakfast)},{GetPrice(day.Breakfast)}");
            // 점심
            csv.AppendLine($"{day.Date:yyyy-MM-dd},Lunch,\"{GetMenuString(day.Lunch)}\",{GetCalories(day.Lunch)},{GetPrice(day.Lunch)}");
            // 저녁
            csv.AppendLine($"{day.Date:yyyy-MM-dd},Dinner,\"{GetMenuString(day.Dinner)}\",{GetCalories(day.Dinner)},{GetPrice(day.Dinner)}");
        }
        return csv.ToString();
    }

    private string GetMenuString(List<FoodItem> items) => string.Join(" + ", items.Select(x => x.Name));
    private int GetCalories(List<FoodItem> items) => items.Sum(x => x.Calories);
    private int GetPrice(List<FoodItem> items) => items.Sum(x => x.Price);
}
