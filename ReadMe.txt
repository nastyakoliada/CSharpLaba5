При разработке решения для лабоработрной работы номер 5 использовались
материалы лобораторной работы номер 4. 

Так как контекст и модель данных, используемых для работы с SQLite, теперь используется 
в нескольких проектах, то они вынесены в отдельные проекты

Решение содержит следующие проекты

1. MusicCatalog.Context - контекст подлкючения к бд SQLite
2. MusicCatalog.EntityModel - модели данных для взаимодействия с контекстом бд
3. MusicCatalog.WebService - реализация REST сервиса взаимодействия с контекстом MusicCatalog.Context, 
	используя модель данных MusicCatalog.EntityModel.
	Описание сервиса - https://localhost:5010/swagger/
4. MusicCatalog.Laba5 - реализация взаимодействия с пользователем для работы с музыкальным каталогом
	и сериализацией через консоль
	- в файлы json, xml, 
	- в бд SQLite (используется контекст MusicCatalog.Context и модель MusicCatalog.EntityModel),
	- через REST Service MusicCatalog.WebService
5. MusicCatalog.Test.XUnit - расширены тесты из ЛР3. 
	Добавлен интегрированный тест музыкального каталога в реализации 
	через WebService (MusicCatalogLaba4Testing.TestWebServiceSerialization). При выполнении теста
	WebService должен быть запущен.
	А также модульные тесты работы контроллера сервиса - RestApiTesting не требующие запуска сервиса


После доработок лабораторной работы номер 4
1. Интерфейсы ISerializer и IMusicCatalog содержат асинхронные методы
2. Класс работы с музыкальным каталогом, которые реализовывали IMusicCatalog и ISerializer, это - MusicCatalog, классы сериализации MCSerializerXml и MCSerializerJSon, 
классы MusicСatalogRestClient, MusicCatalogSQLite изменены для поддержки асинхронной работы методов этих интерфейсов
3. Переписаны модульные тесты для тестирования асинхронных вызовов.

После выполнения лабораторной работы номер 5
1. Добавлены проекты MusicCatalog.UI - Avalonia UI кросcплатформенное приложение для взаимодействия с музыкальными каталогами, реализованными в лабораторной работе 4
2. Добавлен проект MusicCatalog.UI.Dsktop для запуска приложения MusicCatalog.UI  в среде Windows, как Desktop приложения
3. Расширен интерфейс IMusicCatalog для удовлетворения требований к лабораторной работе номер 5. Добавлены методы
	- Task<IEnumerable<Composition>> Search(int stype, string query) - выполняет фильтр с указанием типа фильтра
	- Task<bool> Update(Composition composition) - изменяет существующую композицию в каталоге
	- Task<bool> Delete(int id) - удаляет существующую композицию в каталоге
4. Новые методы интерфейса реализованы во всех классах, которые поддерживают ведение каталога в разных хранилищах
	- MucisCatalog.Laba5.MusicCatalog - класс для ведения каталога в файлах XML и JSON
	- MucisCatalog.Laba5.MusicCatalogRestClient - класс для ведения каталога через WebService
	- MucisCatalog.Laba5.MusicCatalogSQLite - класс для ведения каталога в SQLite
5. Для поддержания работоы с новыми методами в WebSerivice  добавлены новые методы контроллера MusicCatalog.WebService.MusicCatalogController
	- public async Task<IEnumerable<Composition>> GetCompositions(int? stype, string? query) - для применения при 
	  поиске композиций разных типов фильтров
	- public async Task<IActionResult> UpdateComposition([FromBody] Composition c) - для изменения композиции
	- public async Task<IActionResult> DeleteComposition(int? id) - для удаления композиции
6. Для поддержания работы методов п 5. добавлены соответствующие методы в интерфейс репозитория MusicCatalog.WebService.Repositories.IMusicCatalogRepository
	- Task<IEnumerable<Composition>> SearchAsync(int stype,string query) - Метод возвращает перечень композиций, удовлетфоряющих условиям поиска
	- Task<bool> UpdateCompositionAsync(Composition composition) - Изменяет композицию в хранилище
	- Task<bool> DeleteCompositionAync(int id) - Удаляет композицию из каталога
7. Эти доавленные методы реализованы в классе репозитория MusicCatalog.WebService.Repositories.MusicCatalogRepository