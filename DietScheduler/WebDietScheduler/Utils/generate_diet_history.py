import csv
import random
from datetime import date, timedelta
import os

# 저장 경로 설정 (프로젝트 루트 기준 상대 경로 or 실행 위치 기준)
# 이 스크립트는 DietScheduler/WebDietScheduler/Utils/ 에 위치한다고 가정
# 목표 CSV 위치: DietScheduler/WebDietScheduler/wwwroot/CSVs/DietHistory.csv
output_path = "../wwwroot/CSVs/DietHistory.csv"

# 사실적인 메뉴 세트 (밥, 국, 메인/반찬 조합)
menu_sets = [
    ('현미밥', '미역국', '불고기', '시금치나물', '배추김치'),
    ('잡곡밥', '된장찌개', '제육볶음', '콩나물무침', '깍두기'),
    ('쌀밥', '소고기무국', '고등어구이', '계란말이', '열무김치'),
    ('보리밥', '청국장', '보쌈', '무생채', '상추쌈'),
    ('흑미밥', '김치찌개', '계란찜', '멸치볶음', '오이소박이'),
    ('쌀밥', '순두부찌개', '오징어볶음', '잡채', '백김치'),
    ('현미밥', '북엇국', '두부조림', '어묵볶음', '깍두기'),
    ('잡곡밥', '육개장', '동그랑땡', '호박볶음', '배추김치'),
    ('곤드레밥', '콩나물국', '떡갈비', '도라지무침', '물김치'),
    ('쌀밥', '갈비탕', '오이무침', '진미채볶음', '석박지'),
    ('카레라이스', '유부장국', '돈가스', '양배추샐러드', '단무지'),
    ('짜장밥', '짬뽕국물', '탕수육', '짜사이', '단무지'),
    ('하이라이스', '크림스프', '함박스테이크', '마카로니', '피클'),
    ('참치마요덮밥', '미소장국', '타코야끼', '락교', '초생강'),
    ('비빔밥', '콩나물국', '약고추장', '계란후라이', '백김치'),
    ('현미밥', '동태찌개', '감자채볶음', '가지무침', '총각김치'),
    ('쌀밥', '감자국', '코다리조림', '고사리나물', '파김치'),
    ('잡곡밥', '사골국', '소세지야채볶음', '연근조림', '배추김치'),
    ('보리밥', '강된장', '수육', '깻잎장아찌', '열무김치'),
    ('흑미밥', '매운탕', '수제비', '콩자반', '갓김치')
]

start_date = date(2024, 1, 1)
duration = 365

def generate():
    # 경로가 존재하는지 확인하고 없으면 생성
    os.makedirs(os.path.dirname(output_path), exist_ok=True)

    with open(output_path, 'w', encoding='utf-8', newline='') as f:
        writer = csv.writer(f)
        writer.writerow(['Date', 'Meal', 'Menu', 'Calories', 'Price'])

        for i in range(duration):
            current_date = start_date + timedelta(days=i)
            date_str = current_date.strftime('%Y-%m-%d')
            
            # 아침
            b_menu = list(random.choice(menu_sets))
            b_cal = random.randint(400, 600)
            b_price = random.randint(4000, 7000)
            writer.writerow([date_str, 'Breakfast', '+'.join(b_menu), b_cal, b_price])

            # 점심
            l_menu = list(random.choice(menu_sets))
            while l_menu == b_menu:
                l_menu = list(random.choice(menu_sets))
            l_cal = random.randint(700, 900)
            l_price = random.randint(7000, 10000)
            writer.writerow([date_str, 'Lunch', '+'.join(l_menu), l_cal, l_price])

            # 저녁
            d_menu = list(random.choice(menu_sets))
            while d_menu == b_menu or d_menu == l_menu:
                d_menu = list(random.choice(menu_sets))
            d_cal = random.randint(600, 800)
            d_price = random.randint(6000, 9000)
            writer.writerow([date_str, 'Dinner', '+'.join(d_menu), d_cal, d_price])
    
    print(f"Successfully generated diet history at {output_path}")

if __name__ == "__main__":
    generate()

