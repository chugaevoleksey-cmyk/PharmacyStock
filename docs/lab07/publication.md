# Публікація ЛР7

Код і тести перенесено з перевіреної локальної гілки lab07-tests без зміни вмісту файлів. Для кожного з дев'яти кроків перевірено точний збіг Git tree SHA до і після перенесення.

Підключений GitHub створив нові метадані комітів, тому їхні SHA відрізняються. SHA у звіті та журналах — початкові локальні коміти. Повну початкову історію, теги lab7, lab7-red, lab7-green, lab7-refactor та попередні лабораторні збережено у [original-history.bundle](original-history.bundle).

Для відкриття саме початкової історії після завантаження bundle:

```powershell
git clone original-history.bundle PharmacyStock-original
cd PharmacyStock-original
git switch lab07-tests
```

## Відповідність комітів

| Початковий SHA | SHA на GitHub |
|---|---|
| 618ffaf3bd9add13db0e987ecd5b533af2ba57d8 | 42983100c467a977ccef91f546ef69c8ffc39a8c |
| b4f907485bca057ccf170081895159bbede09880 | b097d6bc97fb098c4907ab4c9b818c4443323a26 |
| 3335a921a2f27aea805ea4ab53537ebb4e8e7386 | 614420c1b685522cb8c89be693399bb67b09aaa6 |
| 23323a829d3469faeab1c241ff2c41b0e19c6375 | e3f15abb837a9bbfb1d4ed09b0a8706b9b8fc24f |
| 65c6ef5952d40449ab27a50aaffe7df86edbda8d | f6011b20e08a5445ceb10b57f4a3de61869fc400 |
| 35f9296fd1ad5be87a3445cfd63284a04371a11f | 502f27cef1e0cc9badb2950cba2353b99cb07a6f |
| 94d99ffd7954b9dc1ad2818814be5308d7081f78 | fa2be30f0f467c690c369c3b0989a20c036e1f76 |
| 58c3ac9201df28762eddcbff9318a0bc665ab6b2 | 4ed5989cca56275c37af76fed6ece41a671cabd5 |
| 6871190e7a178bfd0f6c3386d3ea50806b123d84 | 6da87bb7398845e6a611d9b4b07ceb783ac8647a |

Перевірена збірка Release: 0 попереджень, 0 помилок. Тести: 80 основних + 30 Legacy, усі успішні. Покриття всього Core: 427/504 рядки (84,72%), 81/90 напрямків гілок (90%). Повторного запуску GitHub Actions ці показники не означають.

[HTML-звіт покриття](coverage/index.html) потрібно завантажити разом із сусідніми файлами та відкрити локально. Це власне відображення Cobertura, не ReportGenerator.
