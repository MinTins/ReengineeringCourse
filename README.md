# Лабораторні з реінжинірингу (8×)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=MinTins_ReengineeringCourse&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=MinTins_ReengineeringCourse)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=MinTins_ReengineeringCourse&metric=coverage)](https://sonarcloud.io/summary/new_code?id=MinTins_ReengineeringCourse)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=MinTins_ReengineeringCourse&metric=bugs)](https://sonarcloud.io/summary/new_code?id=MinTins_ReengineeringCourse)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=MinTins_ReengineeringCourse&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=MinTins_ReengineeringCourse)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=MinTins_ReengineeringCourse&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=MinTins_ReengineeringCourse)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=MinTins_ReengineeringCourse&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=MinTins_ReengineeringCourse)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=MinTins_ReengineeringCourse&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=MinTins_ReengineeringCourse)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=MinTins_ReengineeringCourse&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=MinTins_ReengineeringCourse)


Цей репозиторій використовується для курсу **реінжиніринг ПЗ**. 
Мета — провести комплексний реінжиніринг спадкового коду NetSdrClient, включаючи рефакторинг архітектури, покращення якості коду, впровадження сучасних практик розробки та автоматизацію процесів контролю якості через CI/CD пайплайни.

---

## Структура 8 лабораторних

  Кожна робота — **через Pull Request або окремий commit**. Додати короткий опис: *що змінено / як перевірити* + звіт про хід виконання в Classroom.

### Лаба 1 — Підключення SonarCloud і CI

**Мета:** створити проект у SonarCloud, підключити GitHub Actions, запустити перший аналіз.

**Необхідно:**
- .NET 8 SDK
- Публічний GitHub-репозиторій
- Обліковка SonarCloud (організація прив'язана до GitHub)

**1) Підключити SonarCloud**
- На SonarCloud створити проект з цього репозиторію (*Analyze new project*).
- Згенерувати **user token** і додати в репозиторій як секрет **`SONAR_TOKEN`** (*Settings → Secrets and variables → Actions*).
- Додати/перевірити `.github/workflows/sonarcloud.yml` з тригерами на PR і push у основну гілку.
- **Вимкнути Automatic Analysis** в проєкті.
- Перевірити **PR-декорацію** (вкладка *Checks* у PR).

**Здати:** посилання на PR чи commit, скрін Quality Gate, скрін бейджів у README.

---

### Лаба 2 — Code Smells через PR + "gated merge"

**Мета:** виправити **5–10** зауважень Sonar (bugs/smells) без зміни поведінки.

**Кроки:**
- Дрібними комітами виправити знайдені Sonar-проблеми у `NetSdrClientApp`.

**Здати:** скріни змін метрик у Sonar.

---

### Лаба 3 — Тести та покриття

**Мета:** підняти покриття коду юніт-тестами в модулі.

**Кроки:**
- Підключити генерацію покриття (`coverlet.msbuild`, формат opencover).
- Додати 4–6 юніт-тестів.

**Здати:** PR із новими тестами, скрін Coverage у Sonar.

---

### Лаба 4 — Дублікати через SonarCloud

**Мета:** зменшити дублікати коду.

**Кроки:**
- Переглянути **Measures → Duplications** у Sonar і **Checks → SonarCloud** у PR.
- Прибрати **1–2** найбільші дубльовані фрагменти (рефакторинг/винесення спільного коду).
- Перезапустити CI, перевірити, що *Duplications on New Code* ≤ порога (типово 3%).

**Здати:** PR з скрінами "до/після".

---

### Лаба 5 — Архітектурні правила (NetArchTest)

**Мета:** дослідження архітектурних правил залежностей.

**Кроки:**
- Додати кілька архітектурних правил залежностей.
- Переконатися, що порушення **ламає збірку** (червоний PR), а фікс — зеленить.

**Здати:** PR із тестами правил, скрін невдалого прогону (до фіксу) і зеленого (після).

---

### Лаба 6 — Безпечний рефакторинг під тести

**Мета:** рефакторинг коду.

**Кроки:**
- Додати проект з юніт тестами для `EchoServer`.
- Реалізувати необхідні зміни в `EchoServer` для покращення його придатності до тестування.
- Покрити код юніт-тестами.

**Здати:** PR + коротка таблиця метрик "до/після".

---

### Лаба 7 — Оновлення залежностей

**Мета:** навчитись виявляти й виправляти уразливі залежності, користуватись інструментами GitHub Security.

**Кроки:**
- `dotnet list NetSdrClient.sln package --outdated --include-transitive`
- Увімкнути GitHub Security (Dependency graph + Dependabot alerts).
- Додати `.github/dependabot.yml`.
- Оновити обрані пакети, прогнати тест/сонар.

**Здати:** PR з оновленням, скрін push-рану після мерджу, нотатки про ризики.

---

### Лаба 8 — Чистий проєкт і gated build

**Мета:** Домогтися зеленого Quality Gate у SonarCloud. Увімкнути gated merge у GitHub.

**Кроки:**
- Довести SonarCloud до "зеленого" (Coverage ≥ 80%, Bugs = 0, Duplications ≤ 3%).
- Увімкнути gated merge: Settings → Branches → Add rule для master.

**Здати:** скрін *Branches → master* з зеленим Gate.

---

## Норми здачі та оцінювання (єдині для всіх лаб)

**Подання:** через **Pull Request** чи **commit**.  
**Опис:** що зроблено, як перевірити, ризики/зворотна сумісність.  
**Артефакти:** скріни/посилання на Sonar, логи CI.

---

## Типові граблі → що робити

- **"You are running CI analysis while Automatic Analysis is enabled"**  
  Вимкнути *Automatic Analysis* у SonarCloud (використовуємо CI).
- **"Project not found"**  
  Перевірити `sonar.organization`/`sonar.projectKey` точно як у UI; токен має доступ до org.
- **Покриття не генерується**  
  Додати `coverlet.msbuild`; використовувати формат **opencover**; у Sonar — `sonar.cs.opencover.reportsPaths`.
- **PR зелений, push червоний**  
  Перевірити **New Code Definition** і довести покриття/дублікації на "new code".
