using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


// Интерфейс отправителя уведомлений
public interface INotificationSender
{
    void Send(string message);
}

// Реализация отправки уведомлений по Email
public class EmailNotificationSender : INotificationSender
{
    public void Send(string message)
    {
        Console.WriteLine($"Email sent: {message}");
    }
}

// Реализация отправки уведомлений по SMS
public class SmsNotificationSender : INotificationSender
{
    public void Send(string message)
    {
        Console.WriteLine($"SMS sent: {message}");
    }
}

// Реализация отправки уведомлений через Telegram
public class TelegramNotificationSender : INotificationSender
{
    public void Send(string message)
    {
        Console.WriteLine($"Telegram message sent: {message}");
    }
}

// Сервис отправки уведомлений
public class NotificationService
{
    private readonly INotificationSender _notificationSender;

    public NotificationService(INotificationSender notificationSender)
    {
        _notificationSender = notificationSender;
    }

    public void Notify(string message)
    {
        _notificationSender.Send(message);
    }
}

// Основной класс программы
class Program
{
    static void Main(string[] args)
    {
        // Создаём хост и регистрируем зависимости
        var host = Host.CreateDefaultBuilder(args)
     .ConfigureLogging(logging =>
     {
         logging.ClearProviders(); // Убираем все провайдеры логирования по умолчанию
         logging.AddConsole();     // Добавляем логирование в консоль
     })
     .ConfigureServices((_, services) =>
     {
         services.AddTransient<EmailNotificationSender>();
         services.AddTransient<SmsNotificationSender>();
         services.AddTransient<TelegramNotificationSender>();
         services.AddTransient<NotificationService>();
     })
     .Build();


        // Получаем сервисы из контейнера
        var serviceProvider = host.Services;

        Console.WriteLine("Choose notification method: 1. Email, 2. SMS, 3. Telegram");
        int choice = int.Parse(Console.ReadLine() ?? "1");

        INotificationSender sender = choice switch
        {
            1 => serviceProvider.GetService<EmailNotificationSender>(),
            2 => serviceProvider.GetService<SmsNotificationSender>(),
            3 => serviceProvider.GetService<TelegramNotificationSender>(),
            _ => throw new ArgumentException("Invalid choice")
        };

        if (sender == null)
        {
            Console.WriteLine("Failed to initialize notification sender.");
            return;
        }

        // Создаём сервис уведомлений с внедрением зависимости
        var notificationService = new NotificationService(sender);

        Console.WriteLine("Enter your message:");
        string message = Console.ReadLine();

        notificationService.Notify(message);
    }
}
