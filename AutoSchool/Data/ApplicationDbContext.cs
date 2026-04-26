using AutoSchool.Infrastructure;
using AutoSchool.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoSchool.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();
        public DbSet<TestResult> TestResults => Set<TestResult>();
        public DbSet<TestAnswer> TestAnswers => Set<TestAnswer>();
        public DbSet<Topic> Topics => Set<Topic>();
        public DbSet<Instructor> Instructors => Set<Instructor>();
        public DbSet<TheoryCredit> TheoryCredits => Set<TheoryCredit>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\MSSQLLocalDB;Database=AutoSchoolDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Client" }
            );

            modelBuilder.Entity<Ticket>().HasData(
                new Ticket { Id = 1, Title = "Билет 1" },
                new Ticket { Id = 2, Title = "Билет 2" }
            );

            modelBuilder.Entity<User>()
                .HasOne(u => u.Instructor)
                .WithMany(i => i.Users)
                .HasForeignKey(u => u.InstructorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Topic>().HasData(
                new Topic { Id = 1, Name = "Общее" },
                new Topic { Id = 2, Name = "Светофор и регулировщик" },
                new Topic { Id = 3, Name = "Дорожные знаки" },
                new Topic { Id = 4, Name = "Скорость" },
                new Topic { Id = 5, Name = "Обгон и опережение" },
                new Topic { Id = 6, Name = "Остановка и стоянка" },
                new Topic { Id = 7, Name = "Переезды и спецсигналы" }
            );

            modelBuilder.Entity<Question>()
                .Property(q => q.TopicId)
                .HasDefaultValue(1);

            modelBuilder.Entity<Question>().HasData(
                // Ticket 1
                new Question { Id = 1, TicketId = 1, TopicId = 2, Text = "Что означает красный сигнал светофора?", Explanation = "Красный сигнал запрещает движение." },
                new Question { Id = 2, TicketId = 1, TopicId = 5, Text = "Можно ли выполнять обгон на пешеходном переходе?", Explanation = "Обгон на пешеходном переходе запрещен." },

                new Question { Id = 4, TicketId = 1, TopicId = 2, Text = "Какой сигнал светофора разрешает движение?", Explanation = "Движение разрешает зелёный сигнал светофора." },
                new Question { Id = 5, TicketId = 1, TopicId = 2, Text = "Разрешено ли движение на жёлтый сигнал светофора?", Explanation = "Жёлтый сигнал запрещает движение, кроме случаев, когда остановка может быть опасной или невозможной." },
                new Question { Id = 6, TicketId = 1, TopicId = 3, Text = "Что означает знак «Уступите дорогу»?", Explanation = "Водитель обязан уступить дорогу транспортным средствам, движущимся по пересекаемой дороге." },
                new Question { Id = 7, TicketId = 1, TopicId = 4, Text = "Максимальная скорость в населённом пункте, если не указано иное?", Explanation = "Если не установлены иные ограничения, в населённом пункте разрешено 60 км/ч." },
                new Question { Id = 8, TicketId = 1, TopicId = 1, Text = "Должны ли водитель и пассажиры быть пристёгнуты ремнями безопасности?", Explanation = "Да, если конструкцией транспортного средства предусмотрены ремни безопасности." },
                new Question { Id = 9, TicketId = 1, TopicId = 6, Text = "Разрешена ли остановка на пешеходном переходе?", Explanation = "Остановка на пешеходном переходе запрещена." },
                new Question { Id = 10, TicketId = 1, TopicId = 6, Text = "Разрешена ли остановка ближе 5 метров перед пешеходным переходом?", Explanation = "Остановка ближе 5 метров перед переходом запрещена." },
                new Question { Id = 11, TicketId = 1, TopicId = 2, Text = "Где нужно остановиться перед стоп-линией при запрещающем сигнале?", Explanation = "Остановиться нужно перед стоп-линией, не заезжая на неё." },

                // Ticket 2
                new Question { Id = 3, TicketId = 2, TopicId = 3, Text = "Что означает знак 'Главная дорога'?", Explanation = "Он указывает, что водитель находится на главной дороге." },

                new Question { Id = 12, TicketId = 2, TopicId = 3, Text = "Что означает знак «Въезд запрещён»?", Explanation = "Знак запрещает въезд транспортных средств в данном направлении." },
                new Question { Id = 13, TicketId = 2, TopicId = 1, Text = "Кто имеет преимущество на перекрёстке равнозначных дорог?", Explanation = "Преимущество имеет транспортное средство, приближающееся справа (правило «помехи справа»)." },
                new Question { Id = 14, TicketId = 2, TopicId = 1, Text = "Разрешено ли пользоваться телефоном без гарнитуры во время движения?", Explanation = "Пользоваться телефоном без устройства hands-free запрещено." },
                new Question { Id = 15, TicketId = 2, TopicId = 1, Text = "Какой документ обязан иметь водитель при управлении транспортным средством?", Explanation = "Водитель обязан иметь водительское удостоверение соответствующей категории." },
                new Question { Id = 16, TicketId = 2, TopicId = 1, Text = "Что должно быть включено днём для обозначения движущегося транспортного средства?", Explanation = "Днём должны быть включены дневные ходовые огни или ближний свет фар." },
                new Question { Id = 17, TicketId = 2, TopicId = 7, Text = "Разрешён ли разворот на железнодорожном переезде?", Explanation = "Разворот на железнодорожном переезде запрещён." },
                new Question { Id = 18, TicketId = 2, TopicId = 7, Text = "Как поступить при приближении автомобиля спецслужб с маячком и сиреной?", Explanation = "Нужно уступить дорогу и обеспечить беспрепятственный проезд." },
                new Question { Id = 19, TicketId = 2, TopicId = 1, Text = "Можно ли пересекать сплошную линию разметки 1.1?", Explanation = "Пересечение сплошной линии 1.1 запрещено." },
                new Question { Id = 20, TicketId = 2, TopicId = 5, Text = "Разрешён ли обгон в тоннеле?", Explanation = "Обгон в тоннеле запрещён." }
            );

            modelBuilder.Entity<AnswerOption>().HasData(
                // ===== Старые варианты (как было) =====
                new AnswerOption { Id = 1, QuestionId = 1, Text = "Движение запрещено", IsCorrect = true },
                new AnswerOption { Id = 2, QuestionId = 1, Text = "Можно ехать осторожно", IsCorrect = false },
                new AnswerOption { Id = 3, QuestionId = 1, Text = "Можно ехать только направо", IsCorrect = false },

                new AnswerOption { Id = 4, QuestionId = 2, Text = "Да, если нет пешеходов", IsCorrect = false },
                new AnswerOption { Id = 5, QuestionId = 2, Text = "Нет, запрещено", IsCorrect = true },
                new AnswerOption { Id = 6, QuestionId = 2, Text = "Да, если включен поворотник", IsCorrect = false },

                new AnswerOption { Id = 7, QuestionId = 3, Text = "Конец населенного пункта", IsCorrect = false },
                new AnswerOption { Id = 8, QuestionId = 3, Text = "Приоритет на перекрестках", IsCorrect = true },
                new AnswerOption { Id = 9, QuestionId = 3, Text = "Запрет остановки", IsCorrect = false },

                // ===== Новые варианты =====
                // Q4
                new AnswerOption { Id = 10, QuestionId = 4, Text = "Зелёный", IsCorrect = true },
                new AnswerOption { Id = 11, QuestionId = 4, Text = "Красный", IsCorrect = false },
                new AnswerOption { Id = 12, QuestionId = 4, Text = "Жёлтый", IsCorrect = false },

                // Q5
                new AnswerOption { Id = 13, QuestionId = 5, Text = "Запрещено, кроме случаев когда остановка невозможна", IsCorrect = true },
                new AnswerOption { Id = 14, QuestionId = 5, Text = "Разрешено всегда", IsCorrect = false },
                new AnswerOption { Id = 15, QuestionId = 5, Text = "Разрешено только направо", IsCorrect = false },

                // Q6
                new AnswerOption { Id = 16, QuestionId = 6, Text = "Нужно уступить транспортным средствам на пересекаемой дороге", IsCorrect = true },
                new AnswerOption { Id = 17, QuestionId = 6, Text = "Движение запрещено", IsCorrect = false },
                new AnswerOption { Id = 18, QuestionId = 6, Text = "Вы на главной дороге", IsCorrect = false },

                // Q7
                new AnswerOption { Id = 19, QuestionId = 7, Text = "60 км/ч", IsCorrect = true },
                new AnswerOption { Id = 20, QuestionId = 7, Text = "90 км/ч", IsCorrect = false },
                new AnswerOption { Id = 21, QuestionId = 7, Text = "40 км/ч", IsCorrect = false },

                // Q8
                new AnswerOption { Id = 22, QuestionId = 8, Text = "Да, если предусмотрены конструкцией", IsCorrect = true },
                new AnswerOption { Id = 23, QuestionId = 8, Text = "Только водитель", IsCorrect = false },
                new AnswerOption { Id = 24, QuestionId = 8, Text = "Только на трассе", IsCorrect = false },

                // Q9
                new AnswerOption { Id = 25, QuestionId = 9, Text = "Нет, запрещена", IsCorrect = true },
                new AnswerOption { Id = 26, QuestionId = 9, Text = "Да, если нет пешеходов", IsCorrect = false },
                new AnswerOption { Id = 27, QuestionId = 9, Text = "Да, не более 1 минуты", IsCorrect = false },

                // Q10
                new AnswerOption { Id = 28, QuestionId = 10, Text = "Да, запрещена", IsCorrect = true },
                new AnswerOption { Id = 29, QuestionId = 10, Text = "Нет, разрешена", IsCorrect = false },
                new AnswerOption { Id = 30, QuestionId = 10, Text = "Запрещена только после перехода", IsCorrect = false },

                // Q11
                new AnswerOption { Id = 31, QuestionId = 11, Text = "Перед стоп-линией", IsCorrect = true },
                new AnswerOption { Id = 32, QuestionId = 11, Text = "На стоп-линии", IsCorrect = false },
                new AnswerOption { Id = 33, QuestionId = 11, Text = "После стоп-линии", IsCorrect = false },

                // Q12
                new AnswerOption { Id = 34, QuestionId = 12, Text = "Въезд запрещён в данном направлении", IsCorrect = true },
                new AnswerOption { Id = 35, QuestionId = 12, Text = "Движение запрещено для всех", IsCorrect = false },
                new AnswerOption { Id = 36, QuestionId = 12, Text = "Остановка запрещена", IsCorrect = false },

                // Q13
                new AnswerOption { Id = 37, QuestionId = 13, Text = "Транспортное средство справа", IsCorrect = true },
                new AnswerOption { Id = 38, QuestionId = 13, Text = "Транспортное средство слева", IsCorrect = false },
                new AnswerOption { Id = 39, QuestionId = 13, Text = "Тот, кто едет быстрее", IsCorrect = false },

                // Q14
                new AnswerOption { Id = 40, QuestionId = 14, Text = "Запрещено", IsCorrect = true },
                new AnswerOption { Id = 41, QuestionId = 14, Text = "Разрешено", IsCorrect = false },
                new AnswerOption { Id = 42, QuestionId = 14, Text = "Разрешено только в пробке", IsCorrect = false },

                // Q15
                new AnswerOption { Id = 43, QuestionId = 15, Text = "Водительское удостоверение соответствующей категории", IsCorrect = true },
                new AnswerOption { Id = 44, QuestionId = 15, Text = "Паспорт", IsCorrect = false },
                new AnswerOption { Id = 45, QuestionId = 15, Text = "Свидетельство о рождении", IsCorrect = false },

                // Q16
                new AnswerOption { Id = 46, QuestionId = 16, Text = "ДХО или ближний свет фар", IsCorrect = true },
                new AnswerOption { Id = 47, QuestionId = 16, Text = "Только дальний свет", IsCorrect = false },
                new AnswerOption { Id = 48, QuestionId = 16, Text = "Только противотуманные фары", IsCorrect = false },

                // Q17
                new AnswerOption { Id = 49, QuestionId = 17, Text = "Нет, запрещён", IsCorrect = true },
                new AnswerOption { Id = 50, QuestionId = 17, Text = "Да, если нет поезда", IsCorrect = false },
                new AnswerOption { Id = 51, QuestionId = 17, Text = "Да, если включены аварийные огни", IsCorrect = false },

                // Q18
                new AnswerOption { Id = 52, QuestionId = 18, Text = "Уступить дорогу и обеспечить проезд", IsCorrect = true },
                new AnswerOption { Id = 53, QuestionId = 18, Text = "Продолжать движение без изменений", IsCorrect = false },
                new AnswerOption { Id = 54, QuestionId = 18, Text = "Остановиться посреди полосы", IsCorrect = false },

                // Q19
                new AnswerOption { Id = 55, QuestionId = 19, Text = "Нет, нельзя", IsCorrect = true },
                new AnswerOption { Id = 56, QuestionId = 19, Text = "Да, если нет встречных", IsCorrect = false },
                new AnswerOption { Id = 57, QuestionId = 19, Text = "Да, если спешите", IsCorrect = false },

                // Q20
                new AnswerOption { Id = 58, QuestionId = 20, Text = "Нет, запрещён", IsCorrect = true },
                new AnswerOption { Id = 59, QuestionId = 20, Text = "Да, разрешён всегда", IsCorrect = false },
                new AnswerOption { Id = 60, QuestionId = 20, Text = "Разрешён только днём", IsCorrect = false }
            );

            modelBuilder.Entity<Instructor>().HasData(
                new Instructor
                {
                    Id = 1,
                    FirstName = "Иван",
                    LastName = "Петров",
                    Phone = "+7 900 111-22-33",
                    Email = "petrov@autoschool.local",
                    VehicleBrand = "Lada",
                    VehicleModel = "Vesta",
                    Transmission = TransmissionType.Manual,
                    VehicleCategory = "B"
                },
                new Instructor
                {
                    Id = 2,
                    FirstName = "Анна",
                    LastName = "Смирнова",
                    Phone = "+7 900 222-33-44",
                    Email = "smirnova@autoschool.local",
                    VehicleBrand = "Kia",
                    VehicleModel = "Rio",
                    Transmission = TransmissionType.Automatic,
                    VehicleCategory = "B"
                },
                new Instructor
                {
                    Id = 3,
                    FirstName = "Сергей",
                    LastName = "Кузнецов",
                    Phone = "+7 900 333-44-55",
                    Email = "kuznetsov@autoschool.local",
                    VehicleBrand = "Yamaha",
                    VehicleModel = "MT-07",
                    Transmission = TransmissionType.Manual,
                    VehicleCategory = "A"
                }
            );

            modelBuilder.Entity<TestAnswer>()
                .HasOne(a => a.TestResult)
                .WithMany(r => r.Answers)
                .HasForeignKey(a => a.TestResultId);

            modelBuilder.Entity<TestAnswer>()
                .HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TestAnswer>()
                .HasOne(a => a.SelectedOption)
                .WithMany()
                .HasForeignKey(a => a.SelectedOptionId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<TheoryCredit>()
                .HasOne(c => c.User)
                .WithMany(u => u.TheoryCredits)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TheoryCredit>()
                .HasOne(c => c.Topic)
                .WithMany()
                .HasForeignKey(c => c.TopicId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1000,
                FirstName = "Admin",
                LastName = "System",
                Email = "admin@autoschool.local",
                PasswordHash = PasswordHasher.Hash("Admin123!"),
                RoleId = 1
            });
        }
    }
}