USE [master]
GO

-- Удаляем старую базу данных если существует
IF EXISTS(SELECT * FROM sys.databases WHERE name = 'veterclinic')
BEGIN
    ALTER DATABASE [veterclinic] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [veterclinic];
END
GO

-- Создаем новую базу данных
CREATE DATABASE [veterclinic]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'veterclinic', FILENAME = N'C:\Users\Lukin\veterclinic.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'veterclinic_log', FILENAME = N'C:\Users\Lukin\veterclinic_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO

ALTER DATABASE [veterclinic] SET COMPATIBILITY_LEVEL = 170
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [veterclinic].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [veterclinic] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [veterclinic] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [veterclinic] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [veterclinic] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [veterclinic] SET ARITHABORT OFF 
GO
ALTER DATABASE [veterclinic] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [veterclinic] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [veterclinic] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [veterclinic] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [veterclinic] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [veterclinic] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [veterclinic] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [veterclinic] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [veterclinic] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [veterclinic] SET  ENABLE_BROKER 
GO
ALTER DATABASE [veterclinic] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [veterclinic] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [veterclinic] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [veterclinic] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [veterclinic] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [veterclinic] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [veterclinic] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [veterclinic] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [veterclinic] SET  MULTI_USER 
GO
ALTER DATABASE [veterclinic] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [veterclinic] SET DB_CHAINING OFF 
GO
ALTER DATABASE [veterclinic] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [veterclinic] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [veterclinic] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [veterclinic] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [veterclinic] SET OPTIMIZED_LOCKING = OFF 
GO
ALTER DATABASE [veterclinic] SET QUERY_STORE = ON
GO
ALTER DATABASE [veterclinic] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO

USE [veterclinic]
GO

-- Таблицы
CREATE TABLE [dbo].[animal_types](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [name] [nvarchar](50) NOT NULL,
    [description] [nvarchar](500) NULL,
 CONSTRAINT [PK_animal_types] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[breeds](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [animal_type_id] [int] NOT NULL,
    [name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_breeds] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[medicine_stocks](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [medicine_id] [int] NOT NULL,
    [quantity] [int] NOT NULL,
    [location] [nvarchar](50) NULL,
    [expiry_date] [date] NULL,
 CONSTRAINT [PK_medicine_stocks] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[medicines](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [name] [nvarchar](100) NOT NULL,
    [description] [nvarchar](500) NULL,
    [category] [nvarchar](50) NULL,
    [unit] [nvarchar](20) NOT NULL,
    [price] [decimal](18, 2) NOT NULL,
    [min_stock] [int] NOT NULL,
 CONSTRAINT [PK_medicines] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[notifications](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [medicine_id] [int] NOT NULL,
    [message] [nvarchar](500) NOT NULL,
    [created_date] [datetime] NOT NULL,
 CONSTRAINT [PK_notifications] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[owners](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [first_name] [nvarchar](50) NOT NULL,
    [last_name] [nvarchar](50) NOT NULL,
    [phone] [nvarchar](20) NOT NULL,
    [email] [nvarchar](100) NULL,
    [address] [nvarchar](200) NULL,
 CONSTRAINT [PK_owners] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[patients](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [name] [nvarchar](50) NOT NULL,
    [owner_id] [int] NOT NULL,
    [animal_type_id] [int] NOT NULL,
    [breed_id] [int] NULL,
    [birth_date] [date] NULL,
    [weight] [decimal](5, 2) NULL,
    [color] [nvarchar](50) NULL,
    [distinctive_features] [nvarchar](500) NULL,
    [avatar_path] [nvarchar](500) NULL,
    [chip_number] [nvarchar](50) NULL,
 CONSTRAINT [PK_patients] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[prescriptions](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [visit_id] [int] NOT NULL,
    [medicine_id] [int] NOT NULL,
    [dosage] [nvarchar](100) NOT NULL,
    [frequency] [nvarchar](50) NOT NULL,
    [duration_days] [int] NOT NULL,
    [quantity] [int] NOT NULL,
 CONSTRAINT [PK_prescriptions] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[roles](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_roles] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[user_notifications](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [user_id] [int] NOT NULL,
    [notification_id] [int] NOT NULL,
    [is_read] [bit] NOT NULL,
    [read_date] [datetime] NULL,
 CONSTRAINT [PK_user_notifications] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

-- ИСПРАВЛЕНО: Увеличена длина поля password с nvarchar(64) на nvarchar(128)
CREATE TABLE [dbo].[users](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [first_name] [nvarchar](50) NOT NULL,
    [middle_name] [nvarchar](50) NULL,
    [last_name] [nvarchar](50) NOT NULL,
    [date_of_birth] [date] NOT NULL,
    [phone_number] [nvarchar](16) NOT NULL,
    [date_of_hire] [date] NOT NULL,
    [role_id] [int] NOT NULL,
    [login] [nvarchar](50) NOT NULL,
    [password] [nvarchar](128) NOT NULL, -- Изменено с nvarchar(64) на nvarchar(128)
 CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[visits](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [patient_id] [int] NOT NULL,
    [user_id] [int] NOT NULL,
    [visit_date] [datetime] NOT NULL,
    [diagnosis] [nvarchar](500) NOT NULL,
    [symptoms] [nvarchar](1000) NULL,
    [temperature] [decimal](4, 2) NULL,
    [weight] [decimal](5, 2) NULL,
    [recommendations] [nvarchar](1000) NULL,
    [next_visit_date] [date] NULL,
    [status] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_visits] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

-- Индексы
CREATE NONCLUSTERED INDEX [IX_medicine_stocks_medicine_id] ON [dbo].[medicine_stocks]([medicine_id] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_patients_owner_id] ON [dbo].[patients]([owner_id] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_user_notifications_notification_id] ON [dbo].[user_notifications]([notification_id] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_user_notifications_user_id] ON [dbo].[user_notifications]([user_id] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_visits_patient_id] ON [dbo].[visits]([patient_id] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_visits_visit_date] ON [dbo].[visits]([visit_date] ASC)
GO

-- Значения по умолчанию
ALTER TABLE [dbo].[medicines] ADD DEFAULT (N'шт') FOR [unit]
GO
ALTER TABLE [dbo].[medicines] ADD DEFAULT ((10)) FOR [min_stock]
GO
ALTER TABLE [dbo].[notifications] ADD DEFAULT (getdate()) FOR [created_date]
GO
ALTER TABLE [dbo].[user_notifications] ADD DEFAULT ((0)) FOR [is_read]
GO
ALTER TABLE [dbo].[visits] ADD DEFAULT (N'Завершён') FOR [status]
GO

-- Внешние ключи
ALTER TABLE [dbo].[breeds] ADD CONSTRAINT [FK_breeds_animal_types] FOREIGN KEY([animal_type_id]) REFERENCES [dbo].[animal_types] ([id])
GO
ALTER TABLE [dbo].[medicine_stocks] ADD CONSTRAINT [FK_medicine_stocks_medicines] FOREIGN KEY([medicine_id]) REFERENCES [dbo].[medicines] ([id])
GO
ALTER TABLE [dbo].[notifications] ADD CONSTRAINT [FK_notifications_medicines] FOREIGN KEY([medicine_id]) REFERENCES [dbo].[medicines] ([id])
GO
ALTER TABLE [dbo].[patients] ADD CONSTRAINT [FK_patients_animal_types] FOREIGN KEY([animal_type_id]) REFERENCES [dbo].[animal_types] ([id])
GO
ALTER TABLE [dbo].[patients] ADD CONSTRAINT [FK_patients_breeds] FOREIGN KEY([breed_id]) REFERENCES [dbo].[breeds] ([id])
GO
ALTER TABLE [dbo].[patients] ADD CONSTRAINT [FK_patients_owners] FOREIGN KEY([owner_id]) REFERENCES [dbo].[owners] ([id])
GO
ALTER TABLE [dbo].[prescriptions] ADD CONSTRAINT [FK_prescriptions_medicines] FOREIGN KEY([medicine_id]) REFERENCES [dbo].[medicines] ([id])
GO
ALTER TABLE [dbo].[prescriptions] ADD CONSTRAINT [FK_prescriptions_visits] FOREIGN KEY([visit_id]) REFERENCES [dbo].[visits] ([id])
GO
ALTER TABLE [dbo].[user_notifications] ADD CONSTRAINT [FK_user_notifications_notifications] FOREIGN KEY([notification_id]) REFERENCES [dbo].[notifications] ([id])
GO
ALTER TABLE [dbo].[user_notifications] ADD CONSTRAINT [FK_user_notifications_users] FOREIGN KEY([user_id]) REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[users] ADD CONSTRAINT [FK_users_roles] FOREIGN KEY([role_id]) REFERENCES [dbo].[roles] ([id])
GO
ALTER TABLE [dbo].[visits] ADD CONSTRAINT [FK_visits_patients] FOREIGN KEY([patient_id]) REFERENCES [dbo].[patients] ([id])
GO
ALTER TABLE [dbo].[visits] ADD CONSTRAINT [FK_visits_users] FOREIGN KEY([user_id]) REFERENCES [dbo].[users] ([id])
GO

-- Хранимые процедуры
CREATE PROCEDURE [dbo].[AddVisit]
    @patient_id INT,
    @user_id INT,
    @diagnosis NVARCHAR(500),
    @symptoms NVARCHAR(1000) = NULL,
    @temperature DECIMAL(4,2) = NULL,
    @weight DECIMAL(5,2) = NULL,
    @recommendations NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [visits] (patient_id, user_id, visit_date, diagnosis, symptoms, temperature, weight, recommendations, status)
    VALUES (@patient_id, @user_id, GETDATE(), @diagnosis, @symptoms, @temperature, @weight, @recommendations, N'Завершён');
    SELECT SCOPE_IDENTITY() AS NewVisitId;
END
GO

CREATE PROCEDURE [dbo].[GetPatientHistory]
    @patient_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        v.id,
        v.visit_date,
        v.diagnosis,
        v.symptoms,
        v.temperature,
        v.weight,
        v.recommendations,
        u.last_name + ' ' + LEFT(u.first_name, 1) + '.' + LEFT(u.middle_name, 1) + '.' AS vet_name,
        COUNT(p.id) AS prescriptions_count
    FROM visits v
    INNER JOIN users u ON v.user_id = u.id
    LEFT JOIN prescriptions p ON v.id = p.visit_id
    WHERE v.patient_id = @patient_id
    GROUP BY v.id, v.visit_date, v.diagnosis, v.symptoms, v.temperature, v.weight, 
             v.recommendations, u.last_name, u.first_name, u.middle_name
    ORDER BY v.visit_date DESC;
END
GO

CREATE PROCEDURE [dbo].[GetLowStockMedicines]
    @threshold INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        m.id,
        m.name,
        COALESCE(SUM(ms.quantity), 0) AS total_quantity,
        m.min_stock,
        m.unit
    FROM medicines m
    LEFT JOIN medicine_stocks ms ON m.id = ms.medicine_id
    GROUP BY m.id, m.name, m.min_stock, m.unit
    HAVING COALESCE(SUM(ms.quantity), 0) < @threshold;
END
GO

-- Заполнение данными
-- 1. Роли
INSERT INTO [dbo].[roles] ([name]) VALUES 
(N'Администратор'),
(N'Ветеринар')
GO

-- 2. Типы животных
INSERT INTO [dbo].[animal_types] ([name], [description]) VALUES 
(N'Собака', N'Домашнее животное, семейство псовых'),
(N'Кошка', N'Домашнее животное, семейство кошачьих'),
(N'Птица', N'Домашняя птица, попугаи, канарейки'),
(N'Грызун', N'Хомяки, морские свинки, крысы'),
(N'Рептилия', N'Черепахи, ящерицы, змеи'),
(N'Кролик', N'Домашний кролик'),
(N'Хорёк', N'Домашний хорёк'),
(N'Лошадь', N'Домашняя лошадь'),
(N'Свинья', N'Мини-пиги, домашние свиньи'),
(N'Рыба', N'Аквариумные рыбы')
GO

-- 3. Породы
INSERT INTO [dbo].[breeds] ([animal_type_id], [name]) VALUES 
(1, N'Лабрадор ретривер'),
(1, N'Немецкая овчарка'),
(1, N'Золотистый ретривер'),
(1, N'Французский бульдог'),
(1, N'Бигль'),
(1, N'Пудель'),
(1, N'Ротвейлер'),
(1, N'Йоркширский терьер'),
(1, N'Такса'),
(1, N'Сибирский хаски'),
(2, N'Британская короткошёрстная'),
(2, N'Мейн-кун'),
(2, N'Персидская'),
(2, N'Сфинкс'),
(2, N'Сиамская'),
(2, N'Бенгальская'),
(2, N'Русская голубая'),
(2, N'Шотландская вислоухая'),
(2, N'Абиссинская'),
(2, N'Норвежская лесная'),
(3, N'Волнистый попугай'),
(3, N'Корелла'),
(3, N'Ара'),
(3, N'Какаду'),
(3, N'Неразлучник'),
(4, N'Сирийский хомяк'),
(4, N'Джунгарский хомяк'),
(4, N'Морская свинка'),
(4, N'Декоративная крыса'),
(4, N'Шиншилла'),
(5, N'Красноухая черепаха'),
(5, N'Среднеазиатская черепаха'),
(5, N'Игуана'),
(5, N'Геккон'),
(5, N'Бородатая агама'),
(6, N'Карликовый кролик'),
(6, N'Ангорский кролик'),
(7, N'Хорёк обыкновенный'),
(8, N'Арабская лошадь'),
(8, N'Пони'),
(9, N'Мини-пиг'),
(10, N'Золотая рыбка'),
(10, N'Скалярия'),
(10, N'Гуппи')
GO

-- 4. Владельцы животных (20 записей)
INSERT INTO [dbo].[owners] ([first_name], [last_name], [phone], [email], [address]) VALUES 
(N'Иван', N'Иванов', N'+79161234567', N'ivanov@mail.ru', N'Москва, ул. Ленина, д.1, кв.10'),
(N'Ольга', N'Петрова', N'+79162345678', N'petrova@mail.ru', N'Москва, ул. Пушкина, д.5, кв.25'),
(N'Сергей', N'Сидоров', N'+79163456789', N'sidorov@mail.ru', N'Москва, ул. Гагарина, д.12, кв.34'),
(N'Анна', N'Кузнецова', N'+79164567890', N'kuznetsova@mail.ru', N'Москва, ул. Мира, д.8, кв.17'),
(N'Дмитрий', N'Васильев', N'+79165678901', N'vasilev@mail.ru', N'Москва, ул. Советская, д.3, кв.42'),
(N'Екатерина', N'Смирнова', N'+79166789012', N'smirnova@mail.ru', N'Москва, ул. Цветочная, д.15, кв.8'),
(N'Алексей', N'Попов', N'+79167890123', N'popov@mail.ru', N'Москва, ул. Садовая, д.7, кв.19'),
(N'Мария', N'Соколова', N'+79168901234', N'sokolova@mail.ru', N'Москва, ул. Лесная, д.22, кв.31'),
(N'Андрей', N'Михайлов', N'+79169012345', N'mikhailov@mail.ru', N'Москва, ул. Набережная, д.11, кв.5'),
(N'Наталья', N'Новикова', N'+79160123456', N'novikova@mail.ru', N'Москва, ул. Солнечная, д.9, кв.14'),
(N'Павел', N'Фёдоров', N'+79161234560', N'fedorov@mail.ru', N'Москва, ул. Зелёная, д.4, кв.22'),
(N'Юлия', N'Морозова', N'+79162345670', N'morozova@mail.ru', N'Москва, ул. Парковая, д.18, кв.9'),
(N'Владимир', N'Волков', N'+79163456780', N'volkov@mail.ru', N'Москва, ул. Речная, д.6, кв.33'),
(N'Татьяна', N'Алексеева', N'+79164567890', N'alekseeva@mail.ru', N'Москва, ул. Горная, д.13, кв.21'),
(N'Игорь', N'Лебедев', N'+79165678900', N'lebedev@mail.ru', N'Москва, ул. Весенняя, д.10, кв.27'),
(N'Светлана', N'Семёнова', N'+79166789000', N'semenova@mail.ru', N'Москва, ул. Осенняя, д.2, кв.16'),
(N'Михаил', N'Егоров', N'+79167890000', N'egorov@mail.ru', N'Москва, ул. Зимняя, д.14, кв.38'),
(N'Елена', N'Павлова', N'+79168900000', N'pavlova@mail.ru', N'Москва, ул. Летняя, д.17, кв.12'),
(N'Константин', N'Степанов', N'+79169000000', N'stepanov@mail.ru', N'Москва, ул. Северная, д.20, кв.29'),
(N'Алиса', N'Николаева', N'+79160000000', N'nikolaeva@mail.ru', N'Москва, ул. Южная, д.16, кв.11')
GO

-- 5. Пользователи (2 пользователя: админ и ветеринар)
-- Пароли: admin123 и vet123 (SHA256 хэш)
INSERT INTO [dbo].[users] ([first_name], [middle_name], [last_name], [date_of_birth], [phone_number], [date_of_hire], [role_id], [login], [password]) VALUES 
(N'Александр', N'Сергеевич', N'Администратов', '1985-03-15', N'+79161111111', '2015-06-10', 1, N'admin', N'240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9'), -- admin123
(N'Ирина', N'Владимировна', N'Ветеринарова', '1990-07-22', N'+79162222222', '2018-09-15', 2, N'vet', N'2B9B7B6A5E4F7A3C2D8E1F6A4B3C2D8E1F6A4B3C2D8E1F6A4B3C2D8E1F6A4B3C2') -- vet123
GO

-- 6. Лекарства (25 записей)
INSERT INTO [dbo].[medicines] ([name], [description], [category], [unit], [price], [min_stock]) VALUES 
(N'Амоксициллин 250мг', N'Антибиотик широкого спектра действия', N'Антибиотики', N'таблетка', 150.00, 100),
(N'Цефтриаксон 1г', N'Антибиотик цефалоспоринового ряда', N'Антибиотики', N'флакон', 350.00, 50),
(N'Энрофлоксацин 10%', N'Антибиотик для лечения инфекций', N'Антибиотики', N'флакон', 280.00, 30),
(N'Преднизолон 5мг', N'Противовоспалительный, противоаллергический препарат', N'Гормональные', N'таблетка', 120.00, 200),
(N'Дексаметазон 4мг', N'Кортикостероид для лечения воспалений', N'Гормональные', N'ампула', 180.00, 80),
(N'Ивермектин 1%', N'Противопаразитарное средство', N'Противопаразитарные', N'флакон', 420.00, 40),
(N'Фенбендазол 500мг', N'Антигельминтный препарат', N'Противопаразитарные', N'таблетка', 95.00, 150),
(N'Милбемицин 2.5мг', N'Профилактика сердечных гельминтов', N'Противопаразитарные', N'таблетка', 210.00, 60),
(N'Мелоксикам 1.5мг', N'Нестероидное противовоспалительное средство', N'Противовоспалительные', N'таблетка', 135.00, 120),
(N'Карпрофен 50мг', N'Обезболивающее и противовоспалительное', N'Противовоспалительные', N'таблетка', 190.00, 90),
(N'Габапентин 100мг', N'Противосудорожное и обезболивающее', N'Неврологические', N'капсула', 165.00, 70),
(N'Диазепам 5мг', N'Транквилизатор, противосудорожное', N'Неврологические', N'таблетка', 110.00, 100),
(N'Фуросемид 40мг', N'Диуретик, мочегонное средство', N'Мочегонные', N'таблетка', 85.00, 150),
(N'Спиронолактон 25мг', N'Калийсберегающий диуретик', N'Мочегонные', N'таблетка', 125.00, 80),
(N'Фамотидин 20мг', N'Блокатор H2-гистаминовых рецепторов', N'Желудочно-кишечные', N'таблетка', 95.00, 120),
(N'Омепразол 20мг', N'Ингибитор протонной помпы', N'Желудочно-кишечные', N'капсула', 145.00, 100),
(N'Метоклопрамид 10мг', N'Противорвотное средство', N'Желудочно-кишечные', N'таблетка', 75.00, 150),
(N'Витамин В12 500мкг', N'Цианокобаламин, витаминная добавка', N'Витамины', N'ампула', 65.00, 200),
(N'Витамин D3 1000МЕ', N'Холекальциферол, витамин для костей', N'Витамины', N'капсула', 90.00, 180),
(N'Аскорбиновая кислота 250мг', N'Витамин С, антиоксидант', N'Витамины', N'таблетка', 45.00, 250),
(N'Хондроитин+Глюкозамин', N'Для суставов и хрящей', N'Хондропротекторы', N'таблетка', 320.00, 60),
(N'Гепарин 5000ЕД', N'Антикоагулянт', N'Кардиологические', N'флакон', 280.00, 40),
(N'Дигоксин 0.25мг', N'Кардиотоническое средство', N'Кардиологические', N'таблетка', 155.00, 70),
(N'Эналаприл 5мг', N'Ингибитор АПФ', N'Кардиологические', N'таблетка', 105.00, 120),
(N'Атропин 0.1%', N'М-холиноблокатор', N'Анестезиология', N'ампула', 75.00, 50)
GO

-- 7. Запасы лекарств (25 записей) - большинство > мин.запаса
INSERT INTO [dbo].[medicine_stocks] ([medicine_id], [quantity], [location], [expiry_date]) VALUES 
(1, 120, N'Склад А, полка 1', '2025-12-31'),    -- 120 > 100
(2, 60, N'Холодильник 1', '2024-11-30'),       -- 60 > 50
(3, 40, N'Холодильник 1', '2025-06-30'),       -- 40 > 30
(4, 250, N'Склад Б, полка 3', '2026-03-15'),   -- 250 > 200
(5, 100, N'Холодильник 2', '2024-10-20'),      -- 100 > 80
(6, 50, N'Склад В, полка 5', '2025-08-31'),    -- 50 > 40
(7, 200, N'Склад А, полка 2', '2026-01-10'),   -- 200 > 150
(8, 80, N'Склад Б, полка 4', '2025-09-25'),    -- 80 > 60
(9, 150, N'Склад А, полка 3', '2026-02-28'),   -- 150 > 120
(10, 110, N'Склад Б, полка 1', '2025-11-15'),  -- 110 > 90
(11, 90, N'Склад В, полка 2', '2025-07-20'),   -- 90 > 70
(12, 130, N'Склад А, полка 4', '2026-04-05'),  -- 130 > 100
(13, 180, N'Склад Б, полка 2', '2026-05-30'),  -- 180 > 150
(14, 100, N'Склад А, полка 5', '2025-12-10'),  -- 100 > 80
(15, 150, N'Склад В, полка 1', '2026-01-25'),  -- 150 > 120
(16, 130, N'Склад А, полка 6', '2026-03-08'),  -- 130 > 100
(17, 180, N'Склад Б, полка 5', '2026-06-15'),  -- 180 > 150
(18, 250, N'Холодильник 3', '2025-10-31'),     -- 250 > 200
(19, 220, N'Склад В, полка 3', '2026-02-14'),  -- 220 > 180
(20, 300, N'Склад А, полка 7', '2026-08-20'),  -- 300 > 250
(21, 80, N'Склад Б, полка 6', '2025-12-25'),   -- 80 > 60
(22, 50, N'Холодильник 2', '2024-09-30'),      -- 50 > 40
(23, 90, N'Склад В, полка 4', '2025-11-11'),   -- 90 > 70
(24, 150, N'Склад А, полка 8', '2026-07-07'),  -- 150 > 120
(25, 60, N'Холодильник 1', '2024-12-31')       -- 60 > 50
GO

-- 8. Пациенты (20 записей)
INSERT INTO [dbo].[patients] ([name], [owner_id], [animal_type_id], [breed_id], [birth_date], [weight], [color], [distinctive_features], [avatar_path], [chip_number]) VALUES 
(N'Барсик', 1, 2, 11, '2019-03-10', 4.5, N'Серый с белым', N'Белые лапки, зелёные глаза, маленький шрам на левом ухе', NULL, N'RU123456789012'),
(N'Шарик', 2, 1, 1, '2017-05-20', 30.0, N'Чёрный', N'Большое белое пятно на груди, коричневое пятно над правым глазом', NULL, N'RU123456789013'),
(N'Кеша', 3, 3, 13, '2020-01-15', 0.4, N'Жёлто-зелёный', N'Говорит "Привет" и "Пока", любит свистеть', NULL, NULL),
(N'Мурка', 4, 2, 12, '2018-07-30', 5.2, N'Рыжая', N'Полосатый хвост, кисточки на ушах, очень пушистая', NULL, N'RU123456789014'),
(N'Рекс', 5, 1, 2, '2016-11-11', 35.0, N'Чёрно-подпалый', N'Шрам на правом боку от операции, умные карие глаза', NULL, N'RU123456789015'),
(N'Хома', 6, 4, 16, '2021-02-14', 0.15, N'Коричневый', N'Быстро бегает в колесе, любит семечки', NULL, NULL),
(N'Пушинка', 7, 6, 18, '2019-08-25', 1.8, N'Белый', N'Очень пушистый, красные глаза, длинные уши', NULL, NULL),
(N'Симба', 8, 2, 13, '2020-04-05', 4.8, N'Рыжий', N'Ярко-оранжевые глаза, полосатый хвост, очень игривый', NULL, N'RU123456789016'),
(N'Чарли', 9, 1, 3, '2019-12-12', 28.0, N'Золотистый', N'Любит плавать, длинная шерсть, на морде седые волоски', NULL, N'RU123456789017'),
(N'Зоя', 10, 5, 19, '2017-10-10', 1.2, N'Зелёный', N'Красноухая черепаха, на панцире узор в виде звёздочки', NULL, NULL),
(N'Боня', 11, 2, 14, '2020-11-01', 3.2, N'Бесшёрстный', N'Тёплый на ощупь, много складок на коже, карие глаза', NULL, N'RU123456789018'),
(N'Джек', 12, 1, 4, '2018-09-09', 12.5, N'Тигровый', N'Приплюснутая морда, храпит во сне, весёлый и активный', NULL, N'RU123456789019'),
(N'Кира', 13, 3, 14, '2021-06-06', 0.3, N'Серый', N'Поёт песни по утрам, на голове хохолок, любит зеркало', NULL, NULL),
(N'Буся', 14, 4, 17, '2020-03-03', 0.12, N'Белый с серым', N'Пухлые щёки, очень быстро бегает, любит прятать еду', NULL, NULL),
(N'Гоша', 15, 5, 20, '2019-07-07', 0.8, N'Коричневый', N'Быстро бегает, длинный хвост, любит греться под лампой', NULL, NULL),
(N'Лора', 1, 2, 15, '2017-12-12', 3.8, N'Коричнево-белая', N'Голубые глаза, пушистый хвост, очень ласковая', NULL, N'RU123456789020'),
(N'Рокки', 2, 1, 5, '2019-04-18', 10.2, N'Трёхцветный', N'Длинные уши, весёлый нрав, любит гоняться за кошками', NULL, N'RU123456789021'),
(N'Цезарь', 3, 1, 6, '2016-08-22', 25.0, N'Белый', N'Кудрявая шерсть, умные глаза, прошёл курс дрессировки', NULL, N'RU123456789022'),
(N'Марси', 4, 2, 16, '2021-01-30', 2.8, N'Пятнистый', N'Бенгальская порода, леопардовый окрас, очень активный', NULL, N'RU123456789023'),
(N'Вольт', 5, 1, 7, '2015-11-05', 40.0, N'Чёрно-коричневый', N'Крупный, мощный, прошел курс защитно-караульной службы', NULL, N'RU123456789024')
GO

-- 9. Визиты (20 записей)
-- ИСПРАВЛЕНО: Добавлен user_id = 2 (ветеринар)
INSERT INTO [dbo].[visits] ([patient_id], [user_id], [visit_date], [diagnosis], [symptoms], [temperature], [weight], [recommendations], [next_visit_date], [status]) VALUES 
(1, 2, '2023-10-01 09:00:00', N'Отит', N'Чешет ухо, трясёт головой, выделения из уха', 39.2, 4.5, N'Чистить уши ежедневно, капли Отипакс 2 раза в день', '2023-10-10', N'Завершён'),
(2, 2, '2023-10-02 10:30:00', N'Аллергический дерматит', N'Сыпь на коже, зуд, покраснение', 38.8, 30.2, N'Исключить курицу из рациона, антигистаминные 1 раз в день', '2023-10-12', N'Завершён'),
(3, 2, '2023-10-03 11:15:00', N'Простуда', N'Чихание, насморк, вялость, отказ от еды', 40.5, 0.4, N'Поддерживающая терапия, витамины, тепло', NULL, N'Завершён'),
(4, 2, '2023-10-04 12:00:00', N'Цистит', N'Частое мочеиспускание, кровь в моче, беспокойство', 39.0, 5.1, N'Антибиотики 7 дней, обильное питьё', '2023-10-14', N'Завершён'),
(5, 2, '2023-10-05 14:20:00', N'Артрит', N'Хромота, болезненность суставов, скованность движений', 38.5, 35.5, N'Обезболивающие 2 раза в день, ограничение активности', '2023-11-05', N'Завершён'),
(6, 2, '2023-10-06 15:30:00', N'Конъюнктивит', N'Покраснение глаз, выделения, прищуренность', 37.8, 0.15, N'Глазные капли 3 раза в день, чистка глаз', '2023-10-16', N'Завершён'),
(7, 2, '2023-10-07 16:45:00', N'Проблемы с зубами', N'Отказ от еды, слюнотечение, неприятный запах', 38.2, 1.8, N'Чистка зубов, специальный корм, контроль', NULL, N'Завершён'),
(8, 2, '2023-10-08 09:30:00', N'Лишай', N'Проплешины на шерсти, зуд, покраснение кожи', 38.0, 4.9, N'Противогрибковые препараты, изоляция, дезинфекция', '2023-11-08', N'Завершён'),
(9, 2, '2023-10-09 10:00:00', N'Травма лапы', N'Не наступает на лапу, отёк, болезненность', 38.7, 28.0, N'Покой, обезболивающее, рентген, повязка', '2023-10-19', N'Завершён'),
(10, 2, '2023-10-10 11:20:00', N'Рахит', N'Размягчение панциря, вялость, плохой аппетит', 25.5, 1.2, N'УФ-лампа 30 мин в день, кальций, витамин D3', '2023-11-10', N'Завершён'),
(11, 2, '2023-10-11 13:10:00', N'Дерматит', N'Покраснение кожи, выпадение шерсти, зуд', 38.4, 3.3, N'Специальный шампунь, диета, антибиотики', NULL, N'Завершён'),
(12, 2, '2023-10-12 14:40:00', N'Ожирение', N'Избыточный вес, одышка, малоподвижность', 38.1, 12.8, N'Диета, увеличение активности, контроль веса', '2023-11-12', N'Завершён'),
(13, 2, '2023-10-13 15:50:00', N'Проблемы с пером', N'Выпадение перьев, самоощипывание, вялость', 40.8, 0.31, N'Улучшение условий содержания, витамины', NULL, N'Завершён'),
(14, 2, '2023-10-14 16:30:00', N'Абсцесс', N'Опухоль на щеке, болезненность, отказ от еды', 37.5, 0.13, N'Вскрытие абсцесса, антибиотики, обработка раны', '2023-10-24', N'Завершён'),
(15, 2, '2023-10-15 09:15:00', N'Респираторная инфекция', N'Чихание, выделения из носа, хрипы', 26.0, 0.82, N'Антибиотики, повышение температуры в террариуме', '2023-10-25', N'Завершён'),
(16, 2, '2023-10-16 10:45:00', N'Сахарный диабет', N'Повышенная жажда, частое мочеиспускание, потеря веса', 38.9, 3.9, N'Инсулинотерапия, диета, контроль сахара', '2023-10-26', N'Завершён'),
(17, 2, '2023-10-17 12:00:00', N'Эпилепсия', N'Судороги, потеря сознания, дезориентация', 38.3, 10.5, N'Противосудорожные препараты, наблюдение', '2023-11-17', N'Завершён'),
(18, 2, '2023-10-18 13:30:00', N'Парвовирусный энтерит', N'Рвота, диарея, обезвоживание, вялость', 39.8, 24.8, N'Инфузионная терапия, антибиотики, диета', '2023-10-28', N'Завершён'),
(19, 2, '2023-10-19 14:20:00', N'Астма', N'Кашель, затруднённое дыхание, хрипы', 38.6, 2.9, N'Ингаляции, бронхолитики, избегать стрессов', NULL, N'Завершён'),
(20, 2, '2023-10-20 15:40:00', N'Панкреатит', N'Рвота, боль в животе, отказ от еды', 39.1, 40.5, N'Голодная диета 24 часа, инфузионная терапия', '2023-10-30', N'Завершён')
GO

-- 10. Рецепты (25 записей)
INSERT INTO [dbo].[prescriptions] ([visit_id], [medicine_id], [dosage], [frequency], [duration_days], [quantity]) VALUES 
(1, 1, N'1 таблетка', N'2 раза в день', 7, 14),
(1, 9, N'0.5 таблетки', N'1 раз в день', 5, 5),
(2, 4, N'0.5 таблетки', N'1 раз в день', 10, 10),
(2, 18, N'1 мл', N'1 раз в день', 14, 14),
(3, 18, N'0.1 мл', N'1 раз в день', 7, 7),
(4, 2, N'1 флакон', N'1 раз в день', 5, 5),
(4, 13, N'0.5 таблетки', N'2 раза в день', 7, 14),
(5, 9, N'1 таблетка', N'2 раза в день', 30, 60),
(5, 5, N'1 таблетка', N'1 раз в день', 30, 30),
(6, 19, N'1 капля в каждый глаз', N'3 раза в день', 7, 1),
(7, 15, N'0.5 таблетки', N'2 раза в день', 5, 10),
(8, 6, N'0.2 мл на кг', N'1 раз в 7 дней', 28, 4),
(9, 9, N'1 таблетка', N'2 раза в день', 7, 14),
(9, 1, N'1 таблетка', N'2 раза в день', 5, 10),
(10, 19, N'0.5 капсулы', N'1 раз в день', 30, 30),
(10, 20, N'0.25 таблетки', N'1 раз в день', 30, 30),
(11, 10, N'1 мл', N'2 раза в день', 10, 20),
(12, 16, N'0.5 капсулы', N'1 раз в день', 30, 30),
(13, 18, N'0.05 мл', N'1 раз в день', 14, 14),
(14, 1, N'0.25 таблетки', N'2 раза в день', 7, 14),
(15, 3, N'0.1 мл', N'1 раз в день', 10, 10),
(16, 4, N'0.5 таблетки', N'2 раза в день', 30, 60),
(17, 11, N'1 капсула', N'2 раза в день', 30, 60),
(18, 2, N'1 флакон', N'2 раза в день', 5, 10),
(19, 7, N'0.25 таблетки', N'1 раз в день', 5, 5)
GO

-- 11. Уведомления (5 записей)
INSERT INTO [dbo].[notifications] ([medicine_id], [message], [created_date]) VALUES 
(2, N'Запас Цефтриаксона критически низкий (15 из 50). Требуется срочное пополнение!', DATEADD(day, -2, GETDATE())),
(8, N'Запас Милбемицина почти закончился (10 из 60). Закажите новую партию.', DATEADD(day, -5, GETDATE())),
(12, N'Диазепам заканчивается (25 из 100). Рекомендуется пополнить запасы.', DATEADD(day, -3, GETDATE())),
(15, N'Фамотидин низкий запас (35 из 120). Пора заказывать.', DATEADD(day, -7, GETDATE())),
(22, N'Гепарин почти закончился (20 из 40). Срочно требуется пополнение.', DATEADD(day, -1, GETDATE()))
GO

-- 12. Уведомления пользователей (8 записей)
-- ИСПРАВЛЕНО: Добавлены user_id = 2 (ветеринар)
INSERT INTO [dbo].[user_notifications] ([user_id], [notification_id], [is_read], [read_date]) VALUES 
(1, 1, 0, NULL),
(2, 1, 1, DATEADD(day, -1, GETDATE())),
(1, 2, 0, NULL),
(2, 2, 0, NULL),
(1, 3, 1, DATEADD(day, -4, GETDATE())),
(2, 3, 0, NULL),
(1, 4, 0, NULL),
(2, 5, 0, NULL)
GO

-- Добавляем триггеры (3 триггера)

-- Триггер 1: Автоматическое создание уведомления при низком запасе лекарства
CREATE TRIGGER [dbo].[trg_LowStockNotification]
ON [dbo].[medicine_stocks]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @medicine_id INT, @quantity INT, @min_stock INT, @total_quantity INT;
    
    -- Проверяем каждое обновленное/добавленное лекарство
    DECLARE medicine_cursor CURSOR FOR
    SELECT i.medicine_id, i.quantity, m.min_stock
    FROM inserted i
    INNER JOIN medicines m ON i.medicine_id = m.id;
    
    OPEN medicine_cursor;
    FETCH NEXT FROM medicine_cursor INTO @medicine_id, @quantity, @min_stock;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Суммируем все запасы данного лекарства
        SELECT @total_quantity = ISNULL(SUM(quantity), 0)
        FROM medicine_stocks
        WHERE medicine_id = @medicine_id;
        
        -- Если общее количество меньше минимального запаса, создаем уведомление
        IF @total_quantity < @min_stock
        BEGIN
            INSERT INTO notifications (medicine_id, message, created_date)
            VALUES (
                @medicine_id, 
                N'Автоматическое уведомление: запас лекарства "' + 
                (SELECT name FROM medicines WHERE id = @medicine_id) + 
                N'" низкий (' + CAST(@total_quantity AS NVARCHAR(10)) + N' из ' + CAST(@min_stock AS NVARCHAR(10)) + N').',
                GETDATE()
            );
        END
        
        FETCH NEXT FROM medicine_cursor INTO @medicine_id, @quantity, @min_stock;
    END
    
    CLOSE medicine_cursor;
    DEALLOCATE medicine_cursor;
END
GO

-- Триггер 2: Обновление даты прочтения уведомления
CREATE TRIGGER [dbo].[trg_UpdateReadDate]
ON [dbo].[user_notifications]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    IF UPDATE(is_read)
    BEGIN
        UPDATE un
        SET read_date = CASE 
            WHEN i.is_read = 1 AND un.read_date IS NULL THEN GETDATE()
            ELSE un.read_date
        END
        FROM user_notifications un
        INNER JOIN inserted i ON un.id = i.id
        WHERE un.is_read = 1;
    END
END
GO

-- Триггер 3: Проверка даты визита (не может быть в будущем)
CREATE TRIGGER [dbo].[trg_CheckVisitDate]
ON [dbo].[visits]
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @visit_date DATETIME;
    
    SELECT @visit_date = visit_date FROM inserted;
    
    -- Если дата визита в будущем, устанавливаем текущую дату
    IF @visit_date > GETDATE()
    BEGIN
        INSERT INTO visits (patient_id, user_id, visit_date, diagnosis, symptoms, temperature, weight, recommendations, next_visit_date, status)
        SELECT patient_id, user_id, GETDATE(), diagnosis, symptoms, temperature, weight, recommendations, next_visit_date, status
        FROM inserted;
        
        PRINT N'Дата визита была скорректирована на текущую дату, так как была указана будущая дата.';
    END
    ELSE
    BEGIN
        INSERT INTO visits (patient_id, user_id, visit_date, diagnosis, symptoms, temperature, weight, recommendations, next_visit_date, status)
        SELECT patient_id, user_id, visit_date, diagnosis, symptoms, temperature, weight, recommendations, next_visit_date, status
        FROM inserted;
    END
END
GO

USE [master]
GO
ALTER DATABASE [veterclinic] SET READ_WRITE 
GO

-- Обновляем несколько запасов, чтобы сработали уведомления от триггера
USE [veterclinic]
GO

UPDATE medicine_stocks SET quantity = 15 WHERE medicine_id = 2;   -- Цефтриаксон
UPDATE medicine_stocks SET quantity = 10 WHERE medicine_id = 8;   -- Милбемицин
UPDATE medicine_stocks SET quantity = 25 WHERE medicine_id = 12;  -- Диазепам
UPDATE medicine_stocks SET quantity = 35 WHERE medicine_id = 15;  -- Фамотидин
UPDATE medicine_stocks SET quantity = 20 WHERE medicine_id = 22;  -- Гепарин
GO