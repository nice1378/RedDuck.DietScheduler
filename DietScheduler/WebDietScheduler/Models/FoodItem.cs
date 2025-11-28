namespace WebDietScheduler.Models;

public class FoodItem
{
    public string Name { get; set; } = string.Empty;
    public int Calories { get; set; }
    public int Price { get; set; }
    public string Category { get; set; } = string.Empty; // 밥, 국, 반찬, etc.
    public string Season { get; set; } = string.Empty;   // 봄, 여름, 가을, 겨울, 사계절
    public string Target { get; set; } = string.Empty;   // 어린이, 성인, 노인, 전체
    
    // 하루 최대 혀용 횟수 (0: 연속 불가능 로직용, 1: 하루 1회, 3: 매끼 가능 등)
    // 기본값은 1로 설정하여 다양성 확보
    public int MaxFrequencyPerDay { get; set; } = 1;

    // CSV 파싱 편의를 위해 매개변수 없는 생성자
    public FoodItem() { }
}

